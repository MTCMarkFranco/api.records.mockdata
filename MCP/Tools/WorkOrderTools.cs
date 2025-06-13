using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using MCPServer.Services;
using MCPServer.Models;

namespace MCPServer.Tools;

/// <summary>
/// MCP tools for accessing WorkOrder data from the OData API
/// </summary>
[McpServerToolType]
public sealed class WorkOrderTools
{
    private readonly ODataApiService odataApiService;

    public WorkOrderTools(ODataApiService odataApiService)
    {
        this.odataApiService = odataApiService;
    }

    [McpServerTool, Description("Get a list of all work orders from the OData API")]
    public async Task<string> GetWorkOrders()
    {
        var workOrders = await odataApiService.GetWorkOrdersAsync();
        return JsonSerializer.Serialize(workOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerTool, Description("Get a specific work order by ID")]
    public async Task<string> GetWorkOrder(
        [Description("The ID of the work order to retrieve")] int id)
    {
        var workOrder = await odataApiService.GetWorkOrderByIdAsync(id);
        if (workOrder == null)
        {
            return JsonSerializer.Serialize(new { error = $"Work order with ID {id} not found" });
        }
        return JsonSerializer.Serialize(workOrder, ApiModelsContext.Default.WorkOrder);
    }

    [McpServerTool, Description("Search work orders by title or description")]
    public async Task<string> SearchWorkOrders(
        [Description("The search term to look for in work order titles and descriptions")] string searchTerm)
    {
        var workOrders = await odataApiService.SearchWorkOrdersAsync(searchTerm);
        return JsonSerializer.Serialize(workOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerTool, Description("Get work orders filtered by status")]
    public async Task<string> GetWorkOrdersByStatus(
        [Description("The status to filter work orders by (e.g., 'Open', 'In Progress', 'Completed')")] string status)
    {
        var allWorkOrders = await odataApiService.GetWorkOrdersAsync();
        var filteredWorkOrders = allWorkOrders
            .Where(wo => wo.Status?.Equals(status, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
        
        return JsonSerializer.Serialize(filteredWorkOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerTool, Description("Get work orders assigned to a specific person")]
    public async Task<string> GetWorkOrdersByAssignee(
        [Description("The name of the person assigned to the work orders")] string assignee)
    {
        var allWorkOrders = await odataApiService.GetWorkOrdersAsync();
        var filteredWorkOrders = allWorkOrders
            .Where(wo => wo.AssignedTo?.Contains(assignee, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
        
        return JsonSerializer.Serialize(filteredWorkOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerTool, Description("Get work orders by priority level")]
    public async Task<string> GetWorkOrdersByPriority(
        [Description("The priority level to filter by (e.g., 'High', 'Medium', 'Low')")] string priority)
    {
        var allWorkOrders = await odataApiService.GetWorkOrdersAsync();
        var filteredWorkOrders = allWorkOrders
            .Where(wo => wo.Priority?.Equals(priority, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
        
        return JsonSerializer.Serialize(filteredWorkOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerTool, Description("Get work orders created within a date range")]
    public async Task<string> GetWorkOrdersByDateRange(
        [Description("Start date in ISO format (e.g., '2024-01-01')")] string startDate,
        [Description("End date in ISO format (e.g., '2024-12-31')")] string endDate)
    {
        if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
        {
            return JsonSerializer.Serialize(new { error = "Invalid date format. Please use ISO date format (e.g., '2024-01-01')" });
        }

        var allWorkOrders = await odataApiService.GetWorkOrdersAsync();
        var filteredWorkOrders = allWorkOrders
            .Where(wo => wo.CreatedDate.HasValue && 
                        wo.CreatedDate.Value.Date >= start.Date && 
                        wo.CreatedDate.Value.Date <= end.Date)
            .ToList();
        
        return JsonSerializer.Serialize(filteredWorkOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerTool, Description("Get a summary of work orders by status")]
    public async Task<string> GetWorkOrdersSummary()
    {
        var allWorkOrders = await odataApiService.GetWorkOrdersAsync();
        
        var summary = new
        {
            Total = allWorkOrders.Count,
            ByStatus = allWorkOrders
                .GroupBy(wo => wo.Status ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count()),
            ByPriority = allWorkOrders
                .GroupBy(wo => wo.Priority ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count()),
            TotalEstimatedHours = allWorkOrders.Sum(wo => wo.EstimatedHours ?? 0),
            TotalActualHours = allWorkOrders.Sum(wo => wo.ActualHours ?? 0)
        };
        
        return JsonSerializer.Serialize(summary);
    }
}
