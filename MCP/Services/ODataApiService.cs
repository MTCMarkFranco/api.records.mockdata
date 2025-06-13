using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MCPServer.Models;

namespace MCPServer.Services;

/// <summary>
/// Service for interacting with the OData API endpoints
/// Provides cached access to WorkOrders and InspectionStatus data
/// </summary>
public class ODataApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<ODataApiService> logger;
    private readonly string baseUrl;
    
    // Cached data
    private List<WorkOrder>? workOrdersCache;
    private List<InspectionStatus>? inspectionStatusCache;
    private DateTime? workOrdersCacheTime;
    private DateTime? inspectionStatusCacheTime;
    
    // Cache expiry duration (5 minutes)
    private readonly TimeSpan cacheExpiry = TimeSpan.FromMinutes(5);

    public ODataApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ODataApiService> logger)
    {
        this.httpClient = httpClientFactory.CreateClient();
        this.logger = logger;
        
        // Get the base URL from configuration, default to localhost if not specified
        this.baseUrl = configuration.GetValue<string>("ODataApi:BaseUrl") ?? "https://localhost:5001";
        
        // Configure HttpClient with appropriate headers for OData
        this.httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        this.httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
        this.httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
    }

    /// <summary>
    /// Gets all work orders from the OData API with caching
    /// </summary>
    /// <returns>List of work orders</returns>
    public async Task<List<WorkOrder>> GetWorkOrdersAsync()
    {
        try
        {
            // Check if cache is valid
            if (workOrdersCache != null && workOrdersCacheTime.HasValue && 
                DateTime.UtcNow - workOrdersCacheTime.Value < cacheExpiry)
            {
                logger.LogDebug("Returning cached work orders");
                return workOrdersCache;
            }

            logger.LogInformation("Fetching work orders from OData API");
            
            var url = $"{baseUrl}/odata/WorkOrders";
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                
                // Parse OData response (assuming it returns a value array)
                using var document = JsonDocument.Parse(jsonResponse);
                var valueElement = document.RootElement.GetProperty("value");
                
                workOrdersCache = JsonSerializer.Deserialize(valueElement.GetRawText(), ApiModelsContext.Default.ListWorkOrder) ?? new List<WorkOrder>();
                workOrdersCacheTime = DateTime.UtcNow;
                
                logger.LogInformation("Successfully fetched {Count} work orders", workOrdersCache.Count);
                return workOrdersCache;
            }
            else
            {
                logger.LogWarning("Failed to fetch work orders. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return workOrdersCache ?? new List<WorkOrder>();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching work orders from OData API");
            return workOrdersCache ?? new List<WorkOrder>();
        }
    }

    /// <summary>
    /// Gets a specific work order by ID
    /// </summary>
    /// <param name="id">The work order ID</param>
    /// <returns>Work order if found, null otherwise</returns>
    public async Task<WorkOrder?> GetWorkOrderByIdAsync(int id)
    {
        try
        {
            logger.LogInformation("Fetching work order with ID: {Id}", id);
            
            var url = $"{baseUrl}/odata/WorkOrders({id})";
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var workOrder = await response.Content.ReadFromJsonAsync(ApiModelsContext.Default.WorkOrder);
                logger.LogInformation("Successfully fetched work order with ID: {Id}", id);
                return workOrder;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogInformation("Work order with ID {Id} not found", id);
                return null;
            }
            else
            {
                logger.LogWarning("Failed to fetch work order {Id}. Status: {StatusCode}", id, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching work order with ID: {Id}", id);
            return null;
        }
    }

    /// <summary>
    /// Searches work orders by title or description
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <returns>Filtered list of work orders</returns>
    public async Task<List<WorkOrder>> SearchWorkOrdersAsync(string searchTerm)
    {
        try
        {
            var workOrders = await GetWorkOrdersAsync();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
                return workOrders;

            var filteredWorkOrders = workOrders.Where(wo => 
                (wo.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                (wo.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true)
            ).ToList();

            logger.LogInformation("Found {Count} work orders matching search term '{SearchTerm}'", 
                filteredWorkOrders.Count, searchTerm);
                
            return filteredWorkOrders;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching work orders with term: {SearchTerm}", searchTerm);
            return new List<WorkOrder>();
        }
    }

    /// <summary>
    /// Gets all inspection statuses from the OData API with caching
    /// </summary>
    /// <returns>List of inspection statuses</returns>
    public async Task<List<InspectionStatus>> GetInspectionStatusesAsync()
    {
        try
        {
            // Check if cache is valid
            if (inspectionStatusCache != null && inspectionStatusCacheTime.HasValue && 
                DateTime.UtcNow - inspectionStatusCacheTime.Value < cacheExpiry)
            {
                logger.LogDebug("Returning cached inspection statuses");
                return inspectionStatusCache;
            }

            logger.LogInformation("Fetching inspection statuses from OData API");
            
            var url = $"{baseUrl}/odata/inspectionstatus";
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                
                // Parse OData response (assuming it returns a value array)
                using var document = JsonDocument.Parse(jsonResponse);
                var valueElement = document.RootElement.GetProperty("value");
                
                inspectionStatusCache = JsonSerializer.Deserialize(valueElement.GetRawText(), ApiModelsContext.Default.ListInspectionStatus) ?? new List<InspectionStatus>();
                inspectionStatusCacheTime = DateTime.UtcNow;
                
                logger.LogInformation("Successfully fetched {Count} inspection statuses", inspectionStatusCache.Count);
                return inspectionStatusCache;
            }
            else
            {
                logger.LogWarning("Failed to fetch inspection statuses. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return inspectionStatusCache ?? new List<InspectionStatus>();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching inspection statuses from OData API");
            return inspectionStatusCache ?? new List<InspectionStatus>();
        }
    }

    /// <summary>
    /// Gets a specific inspection status by ID
    /// </summary>
    /// <param name="id">The inspection status ID</param>
    /// <returns>Inspection status if found, null otherwise</returns>
    public async Task<InspectionStatus?> GetInspectionStatusByIdAsync(int id)
    {
        try
        {
            logger.LogInformation("Fetching inspection status with ID: {Id}", id);
            
            var url = $"{baseUrl}/odata/inspectionstatus({id})";
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var inspectionStatus = await response.Content.ReadFromJsonAsync(ApiModelsContext.Default.InspectionStatus);
                logger.LogInformation("Successfully fetched inspection status with ID: {Id}", id);
                return inspectionStatus;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogInformation("Inspection status with ID {Id} not found", id);
                return null;
            }
            else
            {
                logger.LogWarning("Failed to fetch inspection status {Id}. Status: {StatusCode}", id, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching inspection status with ID: {Id}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets OData metadata
    /// </summary>
    /// <returns>Metadata XML as string</returns>
    public async Task<string> GetMetadataAsync()
    {
        try
        {
            logger.LogInformation("Fetching OData metadata");
            
            var url = $"{baseUrl}/odata/$metadata";
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var metadata = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Successfully fetched OData metadata");
                return metadata;
            }
            else
            {
                logger.LogWarning("Failed to fetch metadata. Status: {StatusCode}", response.StatusCode);
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching OData metadata");
            return string.Empty;
        }
    }

    /// <summary>
    /// Clears all cached data
    /// </summary>
    public void ClearCache()
    {
        workOrdersCache = null;
        inspectionStatusCache = null;
        workOrdersCacheTime = null;
        inspectionStatusCacheTime = null;
        logger.LogInformation("Cleared all cached data");
    }
}
