import pandas as pd
import pyodbc

# Define the connection string to your SQL Server database
connection_string = (
    'DRIVER={ODBC Driver 17 for SQL Server};'
    'SERVER=REDACTED;'
    'DATABASE=REDACTED;'
    'UID=REDACTED;'
    'PWD=REDACTED;'
)

# Path to your CSV file
csv_file_path = r'C:\Users\Wesley\Desktop\Testing\rotatorschedule.csv'

# Read the CSV file into a DataFrame
df = pd.read_csv(csv_file_path)

# SQL Insert Query
insert_query = """
INSERT INTO RotatorSchedule (Hash, RotatorType, Sequence, Name)
VALUES (?, ?, ?, ?)
"""

# Connect to the SQL Server database
try:
    with pyodbc.connect(connection_string) as conn:
        with conn.cursor() as cursor:
            # Iterate over the DataFrame rows and insert them into the database
            for index, row in df.iterrows():
                cursor.execute(insert_query, row['Hash'], row['RotatorType'], row['Sequence'], row['Name'])
            conn.commit()
            print("Data inserted successfully.")
except pyodbc.Error as e:
    print(f"Error: {e}")
