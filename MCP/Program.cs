using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using MCPServer.Services;
using MCPServer.Tools;
using MCPServer.Resources;
using MCPServer.Prompts;
using System.ComponentModel;
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();
if (OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog();
}

// Add MCP Server with SSE (Server-Sent Events) transport for HTTP
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<WorkOrderTools>()
    .WithTools<InspectionStatusTools>()
    .WithTools<ODataMetadataTools>()
    .WithTools<EchoTool>()
    .WithResources<ODataApiResources>()
    .WithPrompts<ODataApiPrompts>();

// Add HTTP client factory for API calls
builder.Services.AddHttpClient();

// Add our OData API service
builder.Services.AddSingleton<ODataApiService>();

// Add CORS for web access
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Build the web application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

// Add a health check endpoint
app.MapGet("/health", () => new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow, 
    server = "Records Mock Data MCP Server",
    version = "1.0.0"
});

// Add an info endpoint
app.MapGet("/info", () => new
{
    name = "Records Mock Data MCP Server",
    description = "MCP server providing access to work orders and inspection status data through OData endpoints",
    version = "1.0.0",
    endpoints = new
    {
        mcp = "/sse",
        health = "/health",
        info = "/info"
    },
    tools = new
    {
        workOrders = 8,
        inspectionStatus = 6,
        metadata = 3,
        testing = 2
    }
});

// MCP Server endpoint for SSE
app.MapMcp();

// Start the server
var port = builder.Configuration.GetValue<int>("Port", 3001);
app.Urls.Add($"http://localhost:{port}");

app.Logger.LogInformation("Starting MCP Server on http://localhost:{Port}", port);
app.Logger.LogInformation("MCP SSE endpoint available at: http://localhost:{Port}/sse", port);
app.Logger.LogInformation("Health check available at: http://localhost:{Port}/health", port);

await app.RunAsync();

/// <summary>
/// Echo tool for testing MCP connectivity
/// </summary>
[McpServerToolType]
public class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client for testing connectivity.")]
    public static string Echo(string message) => $"Echo from OData MCP Server: {message}";

    [McpServerTool, Description("Returns the current server time and status.")]
    public static string GetServerStatus()
    {
        return $"OData MCP Server is running. Current time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
    }
}
