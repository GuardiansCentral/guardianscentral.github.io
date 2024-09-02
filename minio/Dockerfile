# Use the official MinIO image from the registry as the base image
FROM quay.io/minio/minio

# Set environment variables for MinIO root user credentials
ENV MINIO_ROOT_USER=USERNAME
ENV MINIO_ROOT_PASSWORD=PASSWORD

# Define the command to run MinIO with specific arguments
CMD ["server", "/data", "--console-address", ":9001"]
