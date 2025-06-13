using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using MCPServer.Services;
using MCPServer.Models;

namespace MCPServer.Tools;

/// <summary>
/// MCP tools for accessing InspectionStatus data from the OData API
/// </summary>
[McpServerToolType]
public sealed class InspectionStatusTools
{
    private readonly ODataApiService odataApiService;

    public InspectionStatusTools(ODataApiService odataApiService)
    {
        this.odataApiService = odataApiService;
    }

    [McpServerTool, Description("Get a list of all inspection statuses from the OData API")]
    public async Task<string> GetInspectionStatuses()
    {
        var inspectionStatuses = await odataApiService.GetInspectionStatusesAsync();
        return JsonSerializer.Serialize(inspectionStatuses, ApiModelsContext.Default.ListInspectionStatus);
    }

    [McpServerTool, Description("Get a specific inspection status by ID")]
    public async Task<string> GetInspectionStatus(
        [Description("The ID of the inspection status to retrieve")] int id)
    {
        var inspectionStatus = await odataApiService.GetInspectionStatusByIdAsync(id);
        if (inspectionStatus == null)
        {
            return JsonSerializer.Serialize(new { error = $"Inspection status with ID {id} not found" });
        }
        return JsonSerializer.Serialize(inspectionStatus, ApiModelsContext.Default.InspectionStatus);
    }

    [McpServerTool, Description("Get only active inspection statuses")]
    public async Task<string> GetActiveInspectionStatuses()
    {
        var allStatuses = await odataApiService.GetInspectionStatusesAsync();
        var activeStatuses = allStatuses.Where(status => status.IsActive).ToList();
        
        return JsonSerializer.Serialize(activeStatuses, ApiModelsContext.Default.ListInspectionStatus);
    }

    [McpServerTool, Description("Search inspection statuses by name or description")]
    public async Task<string> SearchInspectionStatuses(
        [Description("The search term to look for in inspection status names and descriptions")] string searchTerm)
    {
        var allStatuses = await odataApiService.GetInspectionStatusesAsync();
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return JsonSerializer.Serialize(allStatuses, ApiModelsContext.Default.ListInspectionStatus);
        }

        var filteredStatuses = allStatuses.Where(status => 
            (status.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
            (status.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true)
        ).ToList();
        
        return JsonSerializer.Serialize(filteredStatuses, ApiModelsContext.Default.ListInspectionStatus);
    }

    [McpServerTool, Description("Get inspection statuses ordered by their sort order")]
    public async Task<string> GetInspectionStatusesOrdered()
    {
        var allStatuses = await odataApiService.GetInspectionStatusesAsync();
        var orderedStatuses = allStatuses
            .OrderBy(status => status.SortOrder ?? int.MaxValue)
            .ThenBy(status => status.Name)
            .ToList();
        
        return JsonSerializer.Serialize(orderedStatuses, ApiModelsContext.Default.ListInspectionStatus);
    }

    [McpServerTool, Description("Get a summary of inspection statuses")]
    public async Task<string> GetInspectionStatusesSummary()
    {
        var allStatuses = await odataApiService.GetInspectionStatusesAsync();
        
        var summary = new
        {
            Total = allStatuses.Count,
            Active = allStatuses.Count(status => status.IsActive),
            Inactive = allStatuses.Count(status => !status.IsActive),
            WithColors = allStatuses.Count(status => !string.IsNullOrEmpty(status.Color)),
            LatestCreated = allStatuses
                .Where(status => status.CreatedDate.HasValue)
                .OrderByDescending(status => status.CreatedDate)
                .FirstOrDefault()?.Name ?? "None",
            RecentlyModified = allStatuses
                .Where(status => status.ModifiedDate.HasValue)
                .OrderByDescending(status => status.ModifiedDate)
                .FirstOrDefault()?.Name ?? "None"
        };
        
        return JsonSerializer.Serialize(summary);
    }
}
