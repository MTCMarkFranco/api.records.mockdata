# Records Mock Data MCP Server

A Model Context Protocol (MCP) server implementation for exposing OData API endpoints through MCP tools. This server provides access to WorkOrders and InspectionStatus data from your OData API.

## Overview

This MCP server is built with .NET 9.0 and provides a communication protocol for facilitating interactions with your Records Mock Data OData API. It includes comprehensive tools for querying work orders and inspection statuses, with built-in caching, error handling, and logging.

## Features

### Core Components
- **MCP Server**: Built using the ModelContextProtocol library (version 0.2.0-preview.3)
- **Standard I/O Transport**: Uses stdio for communication with MCP clients
- **OData Integration**: Direct integration with your OData API endpoints
- **Caching**: 5-minute cache expiry for improved performance
- **Comprehensive Logging**: Structured logging with configurable levels

### Available Tools

#### Work Order Tools
- **GetWorkOrders**: Returns a JSON list of all work orders
- **GetWorkOrder**: Retrieves a specific work order by ID
- **SearchWorkOrders**: Search work orders by title or description
- **GetWorkOrdersByStatus**: Filter work orders by status
- **GetWorkOrdersByAssignee**: Filter work orders by assigned person
- **GetWorkOrdersByPriority**: Filter work orders by priority level
- **GetWorkOrdersByDateRange**: Get work orders within a specific date range
- **GetWorkOrdersSummary**: Get summary statistics for work orders

#### Inspection Status Tools
- **GetInspectionStatuses**: Returns a JSON list of all inspection statuses
- **GetInspectionStatus**: Retrieves a specific inspection status by ID
- **GetActiveInspectionStatuses**: Get only active inspection statuses
- **SearchInspectionStatuses**: Search inspection statuses by name or description
- **GetInspectionStatusesOrdered**: Get inspection statuses ordered by sort order
- **GetInspectionStatusesSummary**: Get summary statistics for inspection statuses

#### Metadata Tools
- **GetODataMetadata**: Retrieve OData metadata from the API
- **ClearCache**: Clear all cached data
- **GetApiInfo**: Get information about available tools and endpoints

#### Testing Tools
- **Echo**: Echo back a message for testing connectivity
- **GetServerStatus**: Get current server status and time

### Resources
Pre-configured resources for quick access to common data:
- **WorkOrdersSummary**: Current work orders summary
- **ActiveInspectionStatuses**: List of active inspection statuses
- **HighPriorityWorkOrders**: Work orders with high priority
- **OverdueWorkOrders**: Work orders that are overdue
- **RecentWorkOrders**: Work orders created in the last 7 days
- **WorkOrdersByLocation**: Work orders grouped by location

### Prompts
Predefined prompts for common operations:
- Work order overviews and analysis
- Performance metrics and trends
- Status reports and comparisons
- Inspection status management

## Configuration

### Prerequisites
- .NET 9.0 SDK or later
- Access to your OData API
- Basic understanding of the Model Context Protocol (MCP)

### API Configuration
Configure the OData API base URL in `appsettings.json`:

```json
{
  "ODataApi": {
    "BaseUrl": "https://your-api-url.com"
  }
}
```

### Running the Server

1. **Build the project**:
   ```bash
   dotnet build
   ```

2. **Run the server**:
   ```bash
   dotnet run
   ```

3. **Configure with VS Code or other MCP client**:
   ```json
   {
     "servers": {
       "records-mockdata-mcp": {
         "type": "stdio",
         "command": "dotnet",
         "args": [
           "run",
           "--project",
           "path/to/your/MCPServer.csproj"
         ]
       }
     }
   }
   ```

## API Endpoints

The server connects to these OData API endpoints:
- `/odata/WorkOrders` - Work orders data
- `/odata/inspectionstatus` - Inspection status data
- `/odata/$metadata` - OData metadata

## Error Handling

The server includes comprehensive error handling:
- Network connectivity issues
- API endpoint failures
- Invalid data formats
- Missing resources
- Timeout handling

## Caching

Data is cached for 5 minutes to improve performance:
- Work orders cache
- Inspection statuses cache
- Automatic cache invalidation
- Manual cache clearing via tools

## Development

### Project Structure
- `Models/` - Data models for WorkOrder and InspectionStatus
- `Services/` - ODataApiService for API communication
- `Tools/` - MCP tools implementation
- `Resources/` - MCP resources for quick data access
- `Prompts/` - Predefined prompts for common operations
- `Program.cs` - Application entry point and configuration

## Dependencies

- **Microsoft.Extensions.Hosting** (9.0.6): Hosting infrastructure
- **ModelContextProtocol** (0.2.0-preview.3): MCP server implementation
- **Microsoft.AspNetCore.OData** (8.0.18): OData support
- **System.Text.Json**: JSON serialization

## Troubleshooting

### Common Issues

1. **Connection refused**: Check that your OData API is running and accessible
2. **Invalid JSON**: Verify the API returns valid OData JSON responses
3. **Cache issues**: Use the `ClearCache` tool to reset cached data

### Testing Connectivity
Use the Echo tool to test basic MCP connectivity:
- Tool: `Echo`
- Message: "Hello MCP Server"
- `MCPServer.csproj`: Project file for the MCP server
- `Program.cs`: Entry point for the MCP server
- `mcp.yaml`: MCP data model and endpoint definition
