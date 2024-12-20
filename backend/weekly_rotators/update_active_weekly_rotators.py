from sql_functions import build_active_weekly_rotator_table, does_item_exists_in_column, execute_query, get_max_sequence_by_rotator_type
from backend.weekly_rotators.sql_functions import fetch_one

def update_active_weekly_rotators(config,logger):
    """Adds active weekly rotators to the database"""
    # Check if active weekly Rotators Table Exist and build it if it does not
    build_active_weekly_rotator_table(config=config, logger=logger)

    if not does_item_exists_in_column(config=config, table_name='ActiveWeeklyRotatorsTable', column_name="NAME", name_to_check="ActiveWeeklyRotators", logger=logger):
        logger.info('Active weekly rotators list is not populated. Setting to default values.')
        populate_active_weekly_rotator_table_query = f"INSERT INTO ActiveWeeklyRotatorsTable (NAME, RotatorList) VALUES ('ActiveWeeklyRotators', '[2122313384,1042180643,2823159265,1262462921,2668737148]');"
        execute_query(query=populate_active_weekly_rotator_table_query, config=config, params=None, logger=logger)
        return
    else:
        logger.info("Active weekly rotators list already exists inside the database")

    current_active_weekly_rotators_row = fetch_one(query=f"SELECT RotatorList FROM ActiveWeeklyRotatorsTable WHERE NAME = 'ActiveWeeklyRotators'",config=config,params=None, logger=logger)
    current_active_weekly_rotators_string = current_active_weekly_rotators_row[0]
    rotators_string_list = current_active_weekly_rotators_string.strip('[]').split(',')
    current_active_weekly_rotators_list = [int(x.strip()) for x in rotators_string_list]
    logger.info(f"This is the old active weekly rotators {current_active_weekly_rotators_list}")

    if not current_active_weekly_rotators_list:
        logger.info("No active weekly rotators found or the list is empty")
        logger.info("Creating new active weekly rotators list")
        populate_active_weekly_rotator_table_query = f"INSERT INTO ActiveWeeklyRotatorsTable (NAME, RotatorList) VALUES ('ActiveWeeklyRotators', '[2122313384,1042180643,2823159265,1262462921,2668737148]');"
        execute_query(query=populate_active_weekly_rotator_table_query, config=config, params=None, logger=logger)
        return
    else:
        logger.info("Active weekly rotators list exists")

    raid_one_found = False
    raid_two_found = False
    dungeon_one_found = False
    dungeon_two_found = False
    exotic_one_found = False
    new_active_weekly_rotators_list = []
    for activity in current_active_weekly_rotators_list:
        logger.info(f"Checking activity {activity} inside the RotatorSchedule")
        get_activity_query = f"SELECT Sequence,RotatorType,Hash, Name FROM RotatorSchedule WHERE Hash = {activity}"
        activity_row = fetch_one(query=get_activity_query,config=config,params=None, logger=logger)
        logger.info(f"Activity row for {activity} found")
        logger.info(activity_row)

        sequence = activity_row[0]
        rotator_type = activity_row[1]
        new_activity_sequence = None

        max_sequence = get_max_sequence_by_rotator_type(config=config, activity_type=rotator_type, logger=logger)
        logger.info(max_sequence)
        if rotator_type == 'Raid' or rotator_type == 'Dungeon':
            if sequence >= max_sequence or sequence + 2 > max_sequence:
                if raid_one_found == False and raid_two_found == False and rotator_type == 'Raid':
                    raid_one_found = True
                    new_activity_sequence = 1
                elif raid_two_found == False and raid_one_found == True and rotator_type == 'Raid':
                    raid_two_found = True
                    new_activity_sequence = 2
                elif dungeon_one_found == False and dungeon_two_found == False and rotator_type == 'Dungeon':
                    dungeon_one_found = True
                    new_activity_sequence = 1
                elif dungeon_two_found == False and dungeon_one_found == True and rotator_type == 'Dungeon':
                    dungeon_two_found = True
                    new_activity_sequence = 2
            else:
                if raid_one_found == False and raid_two_found == False and rotator_type == 'Raid':
                    raid_one_found = True
                    new_activity_sequence = sequence + 2
                elif raid_two_found == False and raid_one_found == True and rotator_type == 'Raid':
                    raid_two_found = True
                    new_activity_sequence = sequence + 2
                elif dungeon_one_found == False and dungeon_two_found == False and rotator_type == 'Dungeon':
                    dungeon_one_found = True
                    new_activity_sequence = sequence + 2
                elif dungeon_two_found == False and dungeon_one_found == True and rotator_type == 'Dungeon':
                    dungeon_two_found = True
                    new_activity_sequence = sequence + 2
        elif rotator_type == 'Exotic Mission':
            if sequence >= max_sequence:
                exotic_one_found = True
                new_activity_sequence = 1
            else:
                exotic_one_found = True
                new_activity_sequence = sequence + 1
        if new_activity_sequence is None:
            print('stop')
        new_activity_row = fetch_one(query=f"SELECT Hash, RotatorType, Sequence, Name FROM RotatorSchedule WHERE Sequence = '{new_activity_sequence}' AND RotatorType = '{rotator_type}'",config=config,params=None, logger=logger)
        new_activity_hash = new_activity_row[0]

        new_active_weekly_rotators_list.append(new_activity_hash)
    logger.info(f"This is the current active weekly rotators {str(new_active_weekly_rotators_list)}")

    if raid_one_found == True and raid_two_found == True and dungeon_one_found == True and dungeon_two_found == True and exotic_one_found == True:
        try:
            execute_query(query=f"UPDATE ActiveWeeklyRotatorsTable SET RotatorList = '{new_active_weekly_rotators_list}' WHERE NAME = 'ActiveWeeklyRotators'", config=config, params=None, logger=logger)
            logger.info("Successfully updated active weekly rotators list")
        except Exception as e:
            logger.error("Failed to update active weekly rotators list")
            logger.error(e)
    else:
        logger.error("Did not find one of the new active weekly rotators")