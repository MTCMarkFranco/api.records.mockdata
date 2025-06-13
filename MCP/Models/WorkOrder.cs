using System.Text.Json.Serialization;

namespace MCPServer.Models;

/// <summary>
/// Represents a Work Order entity from the OData API
/// </summary>
public partial class WorkOrder
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Priority { get; set; }
    public string? Location { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
}

/// <summary>
/// Represents an Inspection Status entity from the OData API
/// </summary>
public partial class InspectionStatus
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public int? SortOrder { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

/// <summary>
/// JSON serialization context for the models
/// </summary>
[JsonSerializable(typeof(List<WorkOrder>))]
[JsonSerializable(typeof(WorkOrder))]
[JsonSerializable(typeof(List<InspectionStatus>))]
[JsonSerializable(typeof(InspectionStatus))]
internal sealed partial class ApiModelsContext : JsonSerializerContext
{
}
