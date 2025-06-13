using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCPServer.Prompts;

/// <summary>
/// MCP prompts for common work order and inspection status operations
/// </summary>
[McpServerPromptType]
public class ODataApiPrompts
{
    [McpServerPrompt, Description("Get a comprehensive overview of all work orders.")]
    public static string GetWorkOrdersOverviewPrompt()
    {
        return "Please provide a comprehensive overview of all work orders including their statuses, priorities, and assignees. Organize the information in a table format and include summary statistics.";
    }

    [McpServerPrompt, Description("Analyze work order performance and metrics.")]
    public static string AnalyzeWorkOrderPerformancePrompt()
    {
        return "Please analyze the work order performance including completion rates, estimated vs actual hours, and identify any bottlenecks or trends. Present the analysis with charts or graphs if possible.";
    }

    [McpServerPrompt, Description("Get details for a specific work order.")]
    public static string GetWorkOrderDetailsPrompt([Description("The ID of the work order to get details for")] int workOrderId)
    {
        return $"Please provide comprehensive details for work order ID {workOrderId}, including all available information about status, assignee, timeline, and any related data.";
    }

    [McpServerPrompt, Description("Get current status of all inspection statuses.")]
    public static string GetInspectionStatusOverviewPrompt()
    {
        return "Please provide an overview of all inspection statuses, highlighting which ones are active, their sort order, and any recent changes. Format the information clearly and include summary statistics.";
    }

    [McpServerPrompt, Description("Find work orders that need attention.")]
    public static string FindUrgentWorkOrdersPrompt()
    {
        return "Please identify work orders that require urgent attention, such as high priority items, overdue tasks, or items with significant variance between estimated and actual hours. Prioritize the list by urgency.";
    }

    [McpServerPrompt, Description("Compare work order metrics by assignee.")]
    public static string CompareAssigneePerformancePrompt()
    {
        return "Please analyze and compare work order performance across different assignees, including completion rates, average time to completion, and workload distribution. Present the comparison in a clear, tabular format.";
    }

    [McpServerPrompt, Description("Generate a work order status report.")]
    public static string GenerateWorkOrderStatusReportPrompt([Description("The status to focus on (e.g., 'Open', 'In Progress', 'Completed')")] string status)
    {
        return $"Please generate a detailed report for all work orders with status '{status}', including trends, aging analysis, and actionable insights for management.";
    }

    [McpServerPrompt, Description("Analyze work order trends over time.")]
    public static string AnalyzeWorkOrderTrendsPrompt([Description("The number of days to look back")] int days = 30)
    {
        return $"Please analyze work order trends over the past {days} days, including creation patterns, completion rates, and any seasonal or cyclical trends. Include visualizations if possible.";
    }
}
