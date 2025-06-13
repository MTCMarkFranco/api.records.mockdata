using Azure.Identity;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Controllers
{
    public class mockdataController : ODataController
    {
        private readonly Container _containerWorkOrders;
        private readonly Container _containerInspectionStatus;

        public mockdataController(IConfiguration configuration)
        {
            var accountEndpoint = configuration["CosmosDb:AccountEndpoint"] ?? throw new ArgumentNullException("CosmosDb:AccountEndpoint");
            var databaseName = configuration["CosmosDb:DatabaseName"] ?? throw new ArgumentNullException("CosmosDb:DatabaseName");
            var workOrdersContainerName = configuration["CosmosDb:WorkOrderContainer"] ?? throw new ArgumentNullException("CosmosDb:WorkOrderContainer");
            var inspectionStatusContainer = configuration["CosmosDb:InspectionStatusContainer"] ?? throw new ArgumentNullException("CosmosDb:InspectionStatusContainer");

            var credential = new DefaultAzureCredential();
            var cosmosClient = new CosmosClient(accountEndpoint, credential);
            var database = cosmosClient.GetDatabase(databaseName);
            _containerWorkOrders = database.GetContainer(workOrdersContainerName);
            _containerInspectionStatus = database.GetContainer(inspectionStatusContainer);
        }

       
        // GET odata/workorders
        [EnableQuery]
        [HttpGet("odata/WorkOrders")]
        public async Task<IActionResult> GetWorkOrders()
        {
            var query = _containerWorkOrders.GetItemLinqQueryable<WorkOrder>(true);
            var iterator = query.ToFeedIterator();
            var workOrders = new List<WorkOrder>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                workOrders.AddRange(response);
            }

            // Get all inspections
            var inspectionQuery = _containerInspectionStatus.GetItemLinqQueryable<InspectionStatus>(true);
            var inspectionIterator = inspectionQuery.ToFeedIterator();
            var allInspections = new List<InspectionStatus>();
            while (inspectionIterator.HasMoreResults)
            {
                var response = await inspectionIterator.ReadNextAsync();
                allInspections.AddRange(response);
            }

            // Associate inspections with work orders
            foreach (var workOrder in workOrders)
            {
                workOrder.InspectionStatus = allInspections.Where(i => i.workOrderId == workOrder.id).ToList();
            }

            return Ok(workOrders);
        }

        // GET odata/inspectionstatus
        [EnableQuery]
        [HttpGet("odata/inspectionstatus")]
        public async Task<IActionResult> GetInspectionStatus()
        {
            var query = _containerInspectionStatus.GetItemLinqQueryable<InspectionStatus>(true);
            var iterator = query.ToFeedIterator();
            var inspections = new List<InspectionStatus>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                inspections.AddRange(response);
            }
            return Ok(inspections);
        }
    }
}