using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using MCPServer.Services;
using MCPServer.Models;

namespace MCPServer.Resources;

/// <summary>
/// MCP resources providing quick access to commonly requested data
/// </summary>
[McpServerResourceType]
public class ODataApiResources
{
    private readonly ODataApiService odataApiService;

    public ODataApiResources(ODataApiService odataApiService)
    {
        this.odataApiService = odataApiService;
    }

    [McpServerResource, Description("Current work orders summary")]
    public async Task<string> WorkOrdersSummary()
    {
        var workOrders = await odataApiService.GetWorkOrdersAsync();
        
        var summary = new
        {
            TotalWorkOrders = workOrders.Count,
            StatusBreakdown = workOrders.GroupBy(wo => wo.Status ?? "Unknown").ToDictionary(g => g.Key, g => g.Count()),
            PriorityBreakdown = workOrders.GroupBy(wo => wo.Priority ?? "Unknown").ToDictionary(g => g.Key, g => g.Count()),
            RecentlyCreated = workOrders.Where(wo => wo.CreatedDate >= DateTime.UtcNow.AddDays(-7)).Count(),
            TotalEstimatedHours = workOrders.Sum(wo => wo.EstimatedHours ?? 0),
            TotalActualHours = workOrders.Sum(wo => wo.ActualHours ?? 0),
            LastUpdated = DateTime.UtcNow
        };
        
        return JsonSerializer.Serialize(summary);
    }

    [McpServerResource, Description("Active inspection statuses")]
    public async Task<string> ActiveInspectionStatuses()
    {
        var statuses = await odataApiService.GetInspectionStatusesAsync();
        var activeStatuses = statuses.Where(s => s.IsActive).OrderBy(s => s.SortOrder).ToList();
        
        return JsonSerializer.Serialize(activeStatuses, ApiModelsContext.Default.ListInspectionStatus);
    }

    [McpServerResource, Description("High priority work orders")]
    public async Task<string> HighPriorityWorkOrders()
    {
        var workOrders = await odataApiService.GetWorkOrdersAsync();
        var highPriorityOrders = workOrders
            .Where(wo => wo.Priority?.Equals("High", StringComparison.OrdinalIgnoreCase) == true)
            .OrderBy(wo => wo.CreatedDate)
            .ToList();
        
        return JsonSerializer.Serialize(highPriorityOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerResource, Description("Overdue work orders")]
    public async Task<string> OverdueWorkOrders()
    {
        var workOrders = await odataApiService.GetWorkOrdersAsync();
        var overdueOrders = workOrders
            .Where(wo => wo.CreatedDate.HasValue && 
                        wo.CreatedDate.Value.AddDays(30) < DateTime.UtcNow && // Assuming 30 days is the SLA
                        wo.Status != "Completed")
            .OrderBy(wo => wo.CreatedDate)
            .ToList();
        
        return JsonSerializer.Serialize(overdueOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerResource, Description("Recent work orders (last 7 days)")]
    public async Task<string> RecentWorkOrders()
    {
        var workOrders = await odataApiService.GetWorkOrdersAsync();
        var recentOrders = workOrders
            .Where(wo => wo.CreatedDate >= DateTime.UtcNow.AddDays(-7))
            .OrderByDescending(wo => wo.CreatedDate)
            .ToList();
        
        return JsonSerializer.Serialize(recentOrders, ApiModelsContext.Default.ListWorkOrder);
    }

    [McpServerResource, Description("Work orders by location")]
    public async Task<string> WorkOrdersByLocation()
    {
        var workOrders = await odataApiService.GetWorkOrdersAsync();
        var locationGroups = workOrders
            .GroupBy(wo => wo.Location ?? "Unknown")
            .Select(g => new { Location = g.Key, Count = g.Count(), WorkOrders = g.ToList() })
            .OrderByDescending(g => g.Count)
            .ToList();
        
        return JsonSerializer.Serialize(locationGroups);
    }
}
