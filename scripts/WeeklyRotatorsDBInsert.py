import pyodbc
import pyarrow.parquet as pq

def main():
    # Read the Parquet file into a PyArrow Table
    weekly_rotators_table = pq.read_table('weekly_rotators_table.parquet')
    print(weekly_rotators_table)

    # Convert the PyArrow Table to Pandas DataFrame for easy insertion into SQL Server
    df = weekly_rotators_table.to_pandas()

    # Ensure that the columns have the correct types
    df['MilestoneHash'] = df['MilestoneHash'].astype(int)   # Convert to int
    df['Json'] = df['Json'].astype(str)                     # Convert to str

    # Establish SQL Server connection
    sql_connection = pyodbc.connect(
        'DRIVER={ODBC Driver 17 for SQL Server};'
        'SERVER=REDACTED;'
        'DATABASE=REDACTED;'
        'UID=REDACTED;'
        'PWD=REDACTED'
    )

    cursor = sql_connection.cursor()

    # SQL query template for inserting data
    insert_query = '''
    INSERT INTO WeeklyRotatorsTable (MilestoneHash, Json)
    VALUES (?, ?)
    '''

    # Iterate over the rows in the DataFrame and insert them into the database
    for index, row in df.iterrows():
        cursor.execute(insert_query, row['MilestoneHash'], row['Json'])

    # Commit the transaction
    sql_connection.commit()

    # Close the connection
    cursor.close()
    sql_connection.close()

if __name__ == '__main__':
    main()
