import mssql_python
from dotenv import load_dotenv
import os
import pandas as pd
import json

# Load Config file
config = load_dotenv("REDACTED")

# Extract DB connection details
db_host = os.getenv("DB_HOST")
db_username = os.getenv("DB_USERNAME")
db_password = os.getenv("DB_PASSWORD")
db_database = os.getenv("DB_PROD")

# Table we will be updating
active_weekly_rotators_table_name = 'ActiveWeeklyRotatorsTable'

def connect_to_db():
    connection_string = (
        f"Server={db_host};"
        f"Database={db_database};"
        f"Uid={db_username};"
        f"Pwd={db_password};"
        "Encrypt=Yes;"
        "TrustServerCertificate=Yes;"
    )
    print(f"Connecting to {connection_string}")
    connection = mssql_python.connect(connection_string)
    cursor = connection.cursor()
    return connection, cursor

def check_table_exists(cursor, table_name):
    cursor.execute(f"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table_name}'")
    does_table_exist = cursor.fetchone()
    return bool(does_table_exist)

def check_row_exists(cursor, table_name):
    cursor.execute(
        f"SELECT 1 FROM dbo.{table_name} WHERE Name = 'ActiveWeeklyRotators'"
    )
    return bool(cursor.fetchone())


def create_and_populate_active_weekly_rotators_table(connection, cursor, table_name):
    # Drop ActiveWeeklyRotatorsTable if it exists
    print('Dropping ActiveWeeklyRotatorsTable to recreate it')
    cursor.execute(f"DROP TABLE IF EXISTS dbo.{table_name};")
    connection.commit()

    # Get all the activities with sequence 1
    cursor.execute(f"SELECT Hash FROM RotatorSchedule WHERE Sequence = 1")
    rotator_schedule_table = cursor.fetchall()

    # Convert table to df
    rotator_schedule_df = pd.DataFrame( rotator_schedule_table, columns=["Hash"])

    # Extract Hash list from df and convert it to a string
    default_rotator_schedule_hash_list = rotator_schedule_df["Hash"].tolist()
    default_rotator_schedule_hash_string = json.dumps(default_rotator_schedule_hash_list)
    print(default_rotator_schedule_hash_string)

    # Execute the Create Table Query and Populate it
    print('Creating ActiveWeeklyRotatorsTable')
    cursor.execute(f"CREATE TABLE dbo.{table_name} (Name VARCHAR(MAX) , RotatorList VARCHAR(MAX)); INSERT INTO dbo.{table_name} (Name, RotatorList) VALUES ('ActiveWeeklyRotators', '{default_rotator_schedule_hash_string}');")
    connection.commit()

def update_active_weekly_rotators():
    # Connect to DB
    connection, cursor = connect_to_db()

    # Check if Active Weekly Rotators Table Exist and Create a Populate it if it does not
    active_weekly_rotators_table_status = check_table_exists(cursor, active_weekly_rotators_table_name)
    if not active_weekly_rotators_table_status:
        print("No active weekly rotators table found.")
        create_and_populate_active_weekly_rotators_table(connection=connection, cursor=cursor, table_name=active_weekly_rotators_table_name)
    else:
        print("Active weekly rotators table found.")

    # Check if proper columns exist within Active Weekly Rotators Table
    check_active_weekly_rotators_row_status = check_row_exists(cursor=cursor, table_name=active_weekly_rotators_table_name)
    if not check_active_weekly_rotators_row_status:
        print("No active weekly rotators table found.")
        create_and_populate_active_weekly_rotators_table(connection=connection,cursor=cursor, table_name=active_weekly_rotators_table_name)
    else:
        print("ActiveWeeklyRotators row found")

    new_rotator_schedule_hash_list = []
    if active_weekly_rotators_table_status and check_active_weekly_rotators_row_status:
        # Grab list from ActiveWeeklyRotatorsTable
        cursor.execute(f"SELECT * FROM dbo.{active_weekly_rotators_table_name}")
        active_weekly_rotators_table = cursor.fetchall()
        active_weekly_rotators_df = pd.DataFrame(active_weekly_rotators_table, columns=["Name", "RotatorList"])
        rotators_list_string = active_weekly_rotators_df["RotatorList"].iloc[0]
        active_rotators = json.loads(rotators_list_string)

        # Get the Rotator Schedule
        cursor.execute(f"SELECT * FROM RotatorSchedule")
        rotator_schedule_table = cursor.fetchall()
        rotator_schedule_df = pd.DataFrame(rotator_schedule_table, columns=["Hash", "RotatorType", "Sequence", "Name"])
        print(rotator_schedule_df)


        for active_rotator in active_rotators:

            print(active_rotator)
            row = rotator_schedule_df[rotator_schedule_df["Hash"] == active_rotator].iloc[0]
            hash = row["Hash"]
            rotator_type = row["RotatorType"]
            sequence = row["Sequence"]
            name = row["Name"]
            print(name)

            # Get the max Sequence number for the current rotator
            cursor.execute(f"SELECT MAX(Sequence) from RotatorSchedule WHERE RotatorType = '{rotator_type}'")
            max_rotator_sequence = cursor.fetchone()[0]

            if (sequence + 1) > max_rotator_sequence:
                next_sequence = 1
            else:
                next_sequence = sequence + 1

            next_rotator_row = rotator_schedule_df[(rotator_schedule_df["Sequence"] == next_sequence) & (rotator_schedule_df["RotatorType"] == rotator_type)].iloc[0]
            new_rotator_schedule_hash_list.append(int(next_rotator_row["Hash"]))
        print(active_rotators)
        print(new_rotator_schedule_hash_list)
        print(type(new_rotator_schedule_hash_list))

    raid = False
    dungeon = False
    exotic_mission = False

    # Check if one of each type of rotator exist. This is important for the website as it expects a list of index 3 and one for each type
    for new_rotator_schedule_hash in new_rotator_schedule_hash_list:
        row = rotator_schedule_df[rotator_schedule_df["Hash"] == new_rotator_schedule_hash].iloc[0]
        rotator_type = row["RotatorType"]

        if rotator_type.lower() == "dungeon":
            dungeon = True
        elif rotator_type.lower() == "exotic mission":
            exotic_mission = True
        elif rotator_type.lower() == "raid":
            raid = True

    if raid == True and dungeon == True and exotic_mission == True and len(new_rotator_schedule_hash_list) == 3:
        new_rotator_schedule_hash_string = json.dumps(new_rotator_schedule_hash_list)
        print(new_rotator_schedule_hash_string)
        print(type(new_rotator_schedule_hash_string))
        cursor.execute(
            f"UPDATE dbo.{active_weekly_rotators_table_name} SET RotatorList = ? WHERE Name = ?",
            (new_rotator_schedule_hash_string, "ActiveWeeklyRotators")
        )
        connection.commit()







update_active_weekly_rotators()







