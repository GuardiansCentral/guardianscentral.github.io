import pyodbc

def establish_connection(config,logger):
    connection_string = (
        f"Driver=ODBC Driver 17 for SQL Server;"
        f"Server={config.get('Server')};"
        f"Database={config.get('Database')};"
        f"UID={config.get('UserId')};"
        f"PWD={config.get('Password')};"
    )
    try:
        return pyodbc.connect(connection_string)
    except pyodbc.Error as e:
        logger.error(e)
        logger.error("Error connecting to database.")
        raise

def execute_query(query, config, logger, params=None):
    with establish_connection(config=config,logger=logger) as connection:
        try:
            cursor = connection.cursor()
            if params:
                cursor.execute(query, params)
            else:
                cursor.execute(query)
            connection.commit()
        except Exception as e:
            logger.error(f"An error occurred while executing query: {e}")
            raise

def fetch_one(query, config, logger, params=None):
    """Fetches one result from the database"""
    try:
        with establish_connection(config=config,logger=logger) as connection:
            with connection.cursor() as cursor:
                cursor.execute(query, params or [])
                return cursor.fetchone()
    except Exception as e:
        logger.error(f"An error occurred while fetching the result: {e}")
        raise

def fetch_all(query, config, logger, params=None):
    """Fetches all results from the database"""
    try:
        with establish_connection(config=config,logger=logger) as connection:
            with connection.cursor() as cursor:
                cursor.execute(query, params or [])
                return cursor.fetchall()
    except Exception as e:
        logger.error(f"An error occurred while fetching the results: {e}")
        raise

def table_exists(table_name, config, logger):
    """Returns True if the table exists in the database"""
    try:
        query = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ?"
        return fetch_one(query=query, config=config, params=table_name, logger=logger) is not None
    except Exception as e:
        logger.error(f"An error occurred while executing table exist query for table {table_name}: {e}")


def build_weekly_rotator_table(config,logger):
    """Builds the WeeklyRotatorsTable if it doesn't exist"""
    table_name = 'WeeklyRotatorsTable'
    try:
        if table_exists(table_name=table_name, config=config, logger=logger):
            logger.info(f"The table {table_name} exists.")
        else:
            logger.info(f"The table {table_name} does not exist. Creating it.")
            query = "CREATE TABLE WeeklyRotatorsTable (Hash BIGINT, Json VARCHAR(MAX))"
            execute_query(query=query, config=config,logger=logger)
    except Exception as e:
        logger.error(f"Error while creating the Weekly Rotator table: {e}")

def build_active_weekly_rotator_table(config, logger):
    """Builds the ActiveWeeklyRotatorsTable if it doesn't exist"""
    table_name = 'ActiveWeeklyRotatorsTable'
    try:
        if table_exists(table_name=table_name, config=config, logger=logger):
            logger.info(f"The table {table_name} exists.")
        else:
            logger.info(f"The table {table_name} does not exist. Creating it.")
            query = "CREATE TABLE ActiveWeeklyRotatorsTable (Name VARCHAR(MAX), RotatorList VARCHAR(MAX))"
            execute_query(query=query, config=config, params=None)
            populate_active_weekly_rotator_table_query = f"INSERT INTO ActiveWeeklyRotatorsTable (NAME, RotatorList) VALUES ('ActiveWeeklyRotators', '[2122313384,1042180643,2823159265,1262462921,2668737148]');"
            execute_query(query=populate_active_weekly_rotator_table_query, config=config, params=None)
    except Exception as e:
        logger.error(f"Error while creating the table: {e}")

def does_item_exists_in_column(config, table_name, column_name, name_to_check, logger):
    try:
        query = f"SELECT 1 FROM {table_name} WHERE {column_name} = ?"
        fetch_one(query, config, params=name_to_check,logger=logger)
        return fetch_one(query, config, params=name_to_check,logger=logger) is not None
    except Exception as e:
        logger.error(f"An error occurred while checking if {name_to_check} exist in {column_name} and {table_name}: {e}")


def get_max_sequence_by_rotator_type(config, activity_type,logger):
    try:
        query = (
            f"SELECT MAX(Sequence) AS MaxSequence "
            f"FROM RotatorSchedule "
            f"WHERE RotatorType = ?"
        )
        result = fetch_one(query=query, config=config, params=(activity_type,), logger=logger)
        return result[0] if result else None
    except Exception as e:
        logger.error(f"An error occurred while get max rotator sequence: {e}")