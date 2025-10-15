// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Agents.AI.DevUI;

namespace DevUI_Step01_BasicUsage;

/// <summary>
/// Sample demonstrating basic usage of the DevUI in an ASP.NET Core application.
/// </summary>
/// <remarks>
/// This sample shows how to:
/// 1. Map the DevUI endpoint which automatically configures the middleware
/// 2. Access the DevUI in a web browser
///
/// The DevUI provides an interactive web interface for testing and debugging AI agents.
/// DevUI assets are served from embedded resources within the assembly.
/// Simply call MapDevUI() to set up everything needed.
/// </remarks>
internal static class Program
{
    /// <summary>
    /// Entry point that starts an ASP.NET Core web server with the DevUI.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        // Step 1: Map the DevUI endpoint - this automatically configures the middleware
        // Once the app is running, navigate to: https://localhost:64704/devui
        app.MapDevUI();

        Console.WriteLine("DevUI is available at: https://localhost:64704/devui");
        Console.WriteLine("Press Ctrl+C to stop the server.");

        app.Run();
    }
}
