
using Domain.Models;
using Relatorio;
using Relatorio.Extensions;

var builder = Host.CreateApplicationBuilder(args);

var basePath = AppContext.BaseDirectory;


builder.Services.AddDependencyJob(builder);
builder.Configuration
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


builder.Services.AddHostedService<RelatorioJob>();


var host = builder.Build();
host.Run();
