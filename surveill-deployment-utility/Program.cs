using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Surveill.DeploymentUtility.App;
using Surveill.DeploymentUtility.App.Views;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Reflect.Extensions;
using Terminal.Gui.Reflect.Interfaces;
using Terminal.Gui.Views;

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddReflectServices()
           .AddMemoryCache()
           .AddSingleton<IGitCliService, GitCliService>()
           .AddSingleton<IPipelineStatusService, PipelineStatusService>()
           .AddSingleton<AppSettingsManager>();

hostBuilder.Services.AddSingleton(Application.Create().Init());

using var host = hostBuilder.Build();

await host.StartAsync();

var app                   = host.Services.GetRequiredService<IApplication>();
var viewControllerFactory = host.Services.GetRequiredService<IViewControllerFactory>();
var window = new Window
{
    //Title = "Surveill Deployment Utility"
    BorderStyle = LineStyle.None,
};

window.Add(viewControllerFactory.Create<DashboardView>());

app.Run(window);

host.WaitForShutdown();
