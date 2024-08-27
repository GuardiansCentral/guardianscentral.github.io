from airflow import DAG
from airflow.operators.python import PythonOperator
from datetime import datetime
import pandas as pd
import pyodbc
import shutil
from minio import Minio

minio_client = Minio(
    'minio:9000',
    access_key = "ACCESS",
    secret_key = 'SECRET',
    secure=False
)

def extract_data():
    try:
        connection_string = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};'
                            'SERVER=SERVERIP;'
                            'DATABASE=DBNAME;'
                            'UID=USERNAME;'
                            'PWD=PASSWORD')
        sql_query = "SELECT * FROM WeeklyRotatorsTable"
        dataframe = pd.read_sql(sql_query, connection_string)
        print(dataframe)
        dataframe.to_parquet(r'/opt/airflow/temp/weekly_rotators_table.parquet')
    except Exception as e:
        print(f"An error occurred: {e}")

def load_data():
    shutil.move('/opt/airflow/temp/weekly_rotators_table.parquet', '/opt/airflow/parquet-files/weekly_rotators_table.parquet')


def upload_to_minio():
    try:
        minio_client.fput_object(
            "weekly-rotators-table-bucket",
            "weekly_rotators_table.parquet",
            r"/opt/airflow/parquet-files/weekly_rotators_table.parquet"
        )
        print("File uploaded successfully")
    except Exception as e:
        print(f"An error occured {e}")



default_args = {
    'owner': 'airflow',
    'start_date': datetime(2024, 8, 14),
    'retries': 1,
}

dag = DAG(
    'weekly_rotators_dag',
    default_args=default_args,
    description='A Pipeline to backup Server Data',
    schedule_interval='@weekly',
    catchup=False,
)

extract_task = PythonOperator(
    task_id='extract_data',
    python_callable=extract_data,
    dag=dag,
)

load_task = PythonOperator(
    task_id='load_data',
    python_callable=load_data,
    dag=dag,
)

upload_task = PythonOperator(
    task_id='upload_to_minio',
    python_callable=upload_to_minio,
    dag=dag,
)

extract_task >> load_task >> upload_task