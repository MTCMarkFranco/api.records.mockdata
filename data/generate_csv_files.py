import csv
import random
from datetime import datetime, timedelta

# Generate random data for WorkOrders table
def generate_workorders_data():
    workorders = []
    workOrderIds = []
    for i in range(1, 11):
        workOrderId = random.randint(1000, 9999)
        workOrderIds.append(workOrderId)
        workorder = {
            "id": workOrderId,
            "requestDate": (datetime.now() - timedelta(days=random.randint(1, 365))).strftime("%Y-%m-%d %H:%M:%S"),
            "requestedBy": random.choice(["Michael Scott", "Pam Beesly", "Jim Halpert", "Dwight Schrute"]),
            "details": random.choice(["Routine maintenance", "Urgent repair", "Upgrade request"])
        }
        workorders.append(workorder)
    return workorders, workOrderIds

# Generate random data for InspectionStatus table
def generate_inspectionstatus_data(workOrderIds):
    inspectionstatus = []
    for i in range(1, 11):
        inspection = {
            "id": i,
            "workOrderId": random.choice(workOrderIds),
            "status": random.choice(["Completed", "Pending", "In Progress"]),
            "inspectionDate": (datetime.now() - timedelta(days=random.randint(1, 365))).strftime("%Y-%m-%d %H:%M:%S"),
            "inspector": random.choice(["John Doe", "Jane Smith", "Alice Johnson", "Bob Brown"]),
            "comments": random.choice(["No issues found", "Minor repairs needed", "Follow-up required"])
        }
        inspectionstatus.append(inspection)
    return inspectionstatus

# Write data to CSV file
def write_to_csv(filename, data, headers):
    with open(filename, mode="w", newline="") as file:
        writer = csv.DictWriter(file, fieldnames=headers)
        writer.writeheader()
        writer.writerows(data)

# Main function
def main():
    workorders_data, workOrderIds = generate_workorders_data()
    inspectionstatus_data = generate_inspectionstatus_data(workOrderIds)

    # Write WorkOrders data to CSV
    workorders_headers = ["id", "requestDate", "requestedBy", "details"]
    write_to_csv("workorders.csv", workorders_data, workorders_headers)

    # Write InspectionStatus data to CSV
    inspectionstatus_headers = ["id", "workOrderId", "status", "inspectionDate", "inspector", "comments"]
    write_to_csv("inspectionstatus.csv", inspectionstatus_data, inspectionstatus_headers)

if __name__ == "__main__":
    main()