using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Same registrations as App.xaml.cs
    })
    .Build();

Console.WriteLine("Workspace Guardian CLI");