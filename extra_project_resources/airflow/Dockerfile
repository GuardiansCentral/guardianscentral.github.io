GNU nano 7.2                                         Dockerfile                                                   
FROM apache/airflow:2.5.2

# Install dependencies
USER root
RUN apt-get update && \
    apt-get install -y \
    unixodbc \
    unixodbc-dev \
    && rm -rf /var/lib/apt/lists/*

# Install ODBC Driver for SQL Server
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list | tee /etc/apt/sources.list.d/mssql-releas>
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y msodbcsql17 \
    && rm -rf /var/lib/apt/lists/*

# Install Python dependencies
USER airflow
COPY requirements.txt /
RUN pip install --no-cache-dir -r /requirements.txt

# Make necessary directories
RUN mkdir -p /opt/airflow/temp /opt/airflow/parquet-files
