using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using MCPServer.Services;

namespace MCPServer.Tools;

/// <summary>
/// MCP tools for OData API metadata and utility functions
/// </summary>
[McpServerToolType]
public sealed class ODataMetadataTools
{
    private readonly ODataApiService odataApiService;

    public ODataMetadataTools(ODataApiService odataApiService)
    {
        this.odataApiService = odataApiService;
    }

    [McpServerTool, Description("Get the OData metadata for the API")]
    public async Task<string> GetODataMetadata()
    {
        var metadata = await odataApiService.GetMetadataAsync();
        if (string.IsNullOrEmpty(metadata))
        {
            return JsonSerializer.Serialize(new { error = "Failed to retrieve OData metadata" });
        }
        
        return JsonSerializer.Serialize(new { metadata = metadata });
    }

    [McpServerTool, Description("Clear all cached data from the OData service")]
    public string ClearCache()
    {
        odataApiService.ClearCache();
        return JsonSerializer.Serialize(new { message = "Cache cleared successfully" });
    }

    [McpServerTool, Description("Get API information and available endpoints")]
    public string GetApiInfo()
    {
        var apiInfo = new
        {
            Name = "Records Mock Data OData API",
            Version = "1.0",
            Description = "MCP server providing access to work orders and inspection status data through OData endpoints",
            Endpoints = new
            {
                WorkOrders = "/odata/WorkOrders",
                InspectionStatus = "/odata/inspectionstatus",
                Metadata = "/odata/$metadata"
            },
            AvailableTools = new
            {
                WorkOrderTools = new[]
                {
                    "GetWorkOrders - Get all work orders",
                    "GetWorkOrder - Get a specific work order by ID",
                    "SearchWorkOrders - Search work orders by title/description",
                    "GetWorkOrdersByStatus - Filter work orders by status",
                    "GetWorkOrdersByAssignee - Filter work orders by assignee",
                    "GetWorkOrdersByPriority - Filter work orders by priority",
                    "GetWorkOrdersByDateRange - Get work orders within date range",
                    "GetWorkOrdersSummary - Get work orders summary statistics"
                },
                InspectionStatusTools = new[]
                {
                    "GetInspectionStatuses - Get all inspection statuses",
                    "GetInspectionStatus - Get a specific inspection status by ID",
                    "GetActiveInspectionStatuses - Get only active inspection statuses",
                    "SearchInspectionStatuses - Search inspection statuses by name/description",
                    "GetInspectionStatusesOrdered - Get inspection statuses ordered by sort order",
                    "GetInspectionStatusesSummary - Get inspection statuses summary"
                },
                MetadataTools = new[]
                {
                    "GetODataMetadata - Get OData metadata",
                    "ClearCache - Clear cached data",
                    "GetApiInfo - Get this API information"
                }
            },
            Features = new[]
            {
                "Caching with 5-minute expiry",
                "Comprehensive error handling",
                "Structured logging",
                "Configurable base URL",
                "OData v4 support"
            }
        };
        
        return JsonSerializer.Serialize(apiInfo);
    }
}
