# Microsoft.Agents.AI.DevUI

Developer UI components for Microsoft Agent Framework. This package provides a ready-to-use web interface for interacting with AI agents built using the Microsoft Agent Framework.

## Installation

```bash
dotnet add package Microsoft.Agents.AI.DevUI
```

## Usage in ASP.NET Minimal API

### Basic Usage

To add DevUI to your application, simply call `MapDevUI()`:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Agents.AI.DevUI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Map the DevUI endpoint at the default path
app.MapDevUI(); // Default path: /devui

app.Run();
```

### Custom Path

```csharp
// Specify a custom path for DevUI
app.MapDevUI("/agent-ui");
```

### Multiple DevUI Instances

You can map DevUI at multiple paths with different configurations:

```csharp
// Map DevUI at different paths with different authorization
app.MapDevUI("/admin/devui").RequireAuthorization("Admin");
app.MapDevUI("/dev/devui").RequireAuthorization("Developer");
```

## Usage in ASP.NET MVC/Razor Pages

In your `Program.cs` or `Startup.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map the DevUI endpoint
app.MapDevUI("/devui");

app.Run();
```

Then in your layout or view:

```html
<!DOCTYPE html>
<html>
  <head>
    <title>Agent DevUI</title>
  </head>
  <body>
    <iframe
      src="/devui"
      style="width:100%; height:100vh; border:none; margin:0; padding:0;"
    >
    </iframe>
  </body>
</html>
```

## Features

- **Agent View** - Interactive chat interface for conversing with agents
- **Workflow View** - Visual workflow designer and executor
- **Gallery View** - Browse and select from sample agents and workflows
- **Debug Panel** - Inspect agent execution details and traces
- **Theme Support** - Light and dark mode

## License

MIT - See LICENSE file for details.
