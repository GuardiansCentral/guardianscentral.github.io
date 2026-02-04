import pandas as pd
import mssql_python
import os
from dotenv import load_dotenv


# Load Config file
config = load_dotenv("REDACTED")

# Extract DB connection details
db_host = os.getenv("DB_HOST")
db_username = os.getenv("DB_USERNAME")
db_password = os.getenv("DB_PASSWORD")
db_database = os.getenv("DB_PROD")

# Define the connection string to your SQL Server database
connection_string = (
    f"Server={db_host};"
    f"Database={db_database};"
    f"Uid={db_username};"
    f"Pwd={db_password};"
    "Encrypt=Yes;"
    "TrustServerCertificate=Yes;"
)

connection = mssql_python.connect(connection_string)
cursor = connection.cursor()

# Path to your CSV file
csv_file_path = r"C:\Users\Wesley\Desktop\rotatorschedule.csv"

# Read the CSV file into a DataFrame
df = pd.read_csv(csv_file_path)

# SQL Insert Query (same parameter style)
insert_query = """
INSERT INTO RotatorSchedule (Hash, RotatorType, Sequence, Name)
VALUES (?, ?, ?, ?)
"""

# Connect to the SQL Server database
try:
    connection = mssql_python.connect(connection_string)
    cursor = connection.cursor()

    # Iterate over the DataFrame rows and insert them into the database
    for index, row in df.iterrows():
        cursor.execute(
            insert_query,
            row["Hash"],
            row["RotatorType"],
            row["Sequence"],
            row["Name"]
        )

    connection.commit()
    print("Data inserted successfully.")

except Exception as e:
    print(f"Error: {e}")

finally:
    try:
        cursor.close()
        connection.close()
    except Exception:
        pass
