#!/usr/bin/env node

// MCP Inspector test script
// This script can be used to test the MCP server directly
// Install MCP Inspector: npm install -g @modelcontextprotocol/inspector

const { spawn } = require('child_process');
const path = require('path');

console.log('Starting MCP Server for testing...');

// Path to your MCP server
const mcpServerPath = path.join(__dirname, 'MCPServer.csproj');

// Start the MCP server
const mcpServer = spawn('dotnet', ['run', '--project', mcpServerPath], {
  stdio: ['pipe', 'pipe', 'pipe'],
  env: {
    ...process.env,
    ASPNETCORE_ENVIRONMENT: 'Development'
  }
});

mcpServer.stdout.on('data', (data) => {
  console.log('MCP Server:', data.toString());
});

mcpServer.stderr.on('data', (data) => {
  console.error('MCP Server Error:', data.toString());
});

mcpServer.on('close', (code) => {
  console.log(`MCP Server exited with code ${code}`);
});

// Send test commands
setTimeout(() => {
  console.log('Sending test echo command...');
  const testMessage = JSON.stringify({
    jsonrpc: '2.0',
    id: 1,
    method: 'tools/call',
    params: {
      name: 'Echo',
      arguments: {
        message: 'Hello MCP Server!'
      }
    }
  }) + '\n';
  
  mcpServer.stdin.write(testMessage);
}, 2000);

// Keep the process running
process.on('SIGINT', () => {
  console.log('Shutting down...');
  mcpServer.kill();
  process.exit();
});
