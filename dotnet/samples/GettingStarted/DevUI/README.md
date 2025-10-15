# DevUI Samples

This folder contains samples demonstrating how to use the Microsoft Agents AI DevUI in ASP.NET Core applications.

## Overview

The DevUI provides an interactive web interface for testing and debugging AI agents built with the Microsoft Agent Framework. It includes:

- **Agent View** - Interactive chat interface for conversing with agents
- **Workflow View** - Visual workflow designer and executor
- **Gallery View** - Browse and select from sample agents and workflows
- **Debug Panel** - Inspect agent execution details and traces
- **Theme Support** - Light and dark mode

## Samples

### [DevUI_Step01_BasicUsage](./DevUI_Step01_BasicUsage)

**Demonstrates:**
- Mapping the DevUI endpoint to your application
- Accessing the DevUI in a web browser

**Run the sample:**
```bash
cd DevUI_Step01_BasicUsage
dotnet run
```
Then navigate to: http://localhost:5000

## Requirements

- .NET 8.0 or later
- ASP.NET Core

## Key API

### MapDevUI

Registers the DevUI endpoint in your ASP.NET Core application:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Map the DevUI endpoint
app.MapDevUI();

app.Run();
```

The DevUI assets are served from embedded resources within the assembly. Simply call `MapDevUI()` to set up everything needed.

## Learn More

- [DevUI Package Documentation](../../../../src/Microsoft.Agents.AI.DevUI/README.md)
