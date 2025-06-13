import csv
import os
from azure.cosmos import CosmosClient, PartitionKey
from azure.identity import DefaultAzureCredential

# Set your Cosmos DB endpoint
COSMOS_ENDPOINT = "https://cdb-conversational.documents.azure.com:443/"
DATABASE_NAME = "mockdata"
CONTAINER_NAME = "WorkOrders"

# Path to your CSV file
CSV_FILE_PATH = "workorders.csv"

# Initialize Cosmos client with AAD
def get_container():
    credential = DefaultAzureCredential()
    client = CosmosClient(COSMOS_ENDPOINT, credential=credential)
    db = client.create_database_if_not_exists(DATABASE_NAME)
    container = db.create_container_if_not_exists(
        id=CONTAINER_NAME,
        partition_key=PartitionKey(path="/id"),
        offer_throughput=400
    )
    return container

def import_csv_to_cosmos(csv_path):
    container = get_container()
    with open(csv_path, newline='', encoding='utf-8') as csvfile:
        reader = csv.DictReader(csvfile)
        for row in reader:
            # Ensure each document has a unique 'id' field
            if 'id' not in row or not row['id']:
                row['id'] = str(hash(str(row)))
            container.upsert_item(row)
    print(f"Imported records from {csv_path} to Cosmos DB container '{CONTAINER_NAME}'.")

if __name__ == "__main__":
    if not os.path.exists(CSV_FILE_PATH):
        print(f"File not found: {CSV_FILE_PATH}")
    else:
        import_csv_to_cosmos(CSV_FILE_PATH)
