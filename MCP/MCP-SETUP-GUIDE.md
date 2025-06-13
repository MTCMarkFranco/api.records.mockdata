# MCP Server Configuration Guide

## VS Code with GitHub Copilot Configuration

To use this MCP server with VS Code and GitHub Copilot, add the following configuration to your VS Code settings:

### Method 1: User Settings (settings.json)
Open VS Code settings (Ctrl+,) and add to your `settings.json`:

```json
{
  "github.copilot.chat.experimental.mcp.servers": {
    "records-mockdata": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "f:\\Projects\\api.records.mockdata\\MCP\\MCPServer.csproj"
      ],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Method 2: Workspace Settings
Create or edit `.vscode/settings.json` in your workspace root:

```json
{
  "github.copilot.chat.experimental.mcp.servers": {
    "records-mockdata": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "./MCP/MCPServer.csproj"
      ],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## Claude Desktop Configuration

For Claude Desktop, add this to your configuration file:

### Windows: `%APPDATA%\Claude\claude_desktop_config.json`
```json
{
  "mcpServers": {
    "records-mockdata": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "f:\\Projects\\api.records.mockdata\\MCP\\MCPServer.csproj"
      ],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### macOS: `~/Library/Application Support/Claude/claude_desktop_config.json`
```json
{
  "mcpServers": {
    "records-mockdata": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/your/api.records.mockdata/MCP/MCPServer.csproj"
      ],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## Environment Configuration

### Required Configuration
Before using the MCP server, ensure your OData API is running and update the base URL in `appsettings.json`:

```json
{
  "ODataApi": {
    "BaseUrl": "https://localhost:7000"
  }
}
```

Or set via environment variable:
```bash
set ODataApi__BaseUrl=https://your-api-url.com
```

### Development vs Production
- **Development**: Uses detailed logging and debug information
- **Production**: Minimal logging for performance

## Testing the Connection

Once configured, you can test the MCP server by asking questions like:

### Basic Connectivity
- "Use the Echo tool to test the MCP connection"
- "Show me the server status"

### Work Orders
- "Get all work orders"
- "Show me work orders with high priority"
- "Find work orders assigned to John Doe"
- "Get work orders created in the last week"
- "Show me a summary of work orders by status"

### Inspection Status
- "Get all inspection statuses"
- "Show me only active inspection statuses"
- "Search for inspection statuses containing 'pending'"

### Analysis and Reporting
- "Analyze work order performance metrics"
- "Generate a work order status report for 'In Progress' items"
- "Compare assignee performance across all work orders"

## Available Tools Summary

### Work Order Tools (8 tools)
1. `GetWorkOrders` - Get all work orders
2. `GetWorkOrder` - Get specific work order by ID
3. `SearchWorkOrders` - Search by title/description
4. `GetWorkOrdersByStatus` - Filter by status
5. `GetWorkOrdersByAssignee` - Filter by assignee
6. `GetWorkOrdersByPriority` - Filter by priority
7. `GetWorkOrdersByDateRange` - Filter by date range
8. `GetWorkOrdersSummary` - Get summary statistics

### Inspection Status Tools (6 tools)
1. `GetInspectionStatuses` - Get all inspection statuses
2. `GetInspectionStatus` - Get specific status by ID
3. `GetActiveInspectionStatuses` - Get only active statuses
4. `SearchInspectionStatuses` - Search by name/description
5. `GetInspectionStatusesOrdered` - Get ordered by sort order
6. `GetInspectionStatusesSummary` - Get summary statistics

### Metadata Tools (3 tools)
1. `GetODataMetadata` - Get OData metadata
2. `ClearCache` - Clear cached data
3. `GetApiInfo` - Get API information

### Testing Tools (2 tools)
1. `Echo` - Echo test message
2. `GetServerStatus` - Get server status

## Troubleshooting

### Common Issues

1. **MCP Server not appearing in tools**
   - Restart VS Code or Claude Desktop after configuration
   - Check that the path to MCPServer.csproj is correct
   - Ensure .NET 9.0 SDK is installed

2. **Connection refused errors**
   - Verify your OData API is running
   - Check the BaseUrl in appsettings.json
   - Ensure firewall allows the connection

3. **Build errors**
   - Run `dotnet restore` in the MCP directory
   - Check that all NuGet packages are properly restored

4. **No data returned**
   - Verify your OData API endpoints are working
   - Check API returns data in expected OData format
   - Use the ClearCache tool to refresh data

### Debug Mode
Enable detailed logging by setting:
```bash
set ASPNETCORE_ENVIRONMENT=Development
```

Or modify the configuration to include:
```json
"env": {
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```

### Logs Location
Logs will appear in:
- Console output when running the server directly
- VS Code integrated terminal when debugging
- Windows Event Log (when running on Windows in production)

## Performance Notes

- Data is cached for 5 minutes to improve performance
- Use `ClearCache` tool if you need fresh data immediately
- The server automatically handles connection pooling and retries
- Concurrent requests are supported for better responsiveness

## Security Considerations

- The MCP server runs locally and connects to your local OData API
- No external network access required
- All communication uses stdio transport (no network ports)
- Configure appropriate authentication if your OData API requires it
