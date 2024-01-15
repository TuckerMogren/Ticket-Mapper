using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using TicketMapper.WebApi.Configs;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Example of calling ConfigureLogging correctly
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();


// Set up Serilog here before any logging is done
Log.Logger = builder.Services.ConfigureLogging(configuration).CreateLogger();
builder.Host.UseSerilog();


Log.Logger.Information($"Application is starting...");


// Continue with your service and application setup
builder.Services.ConfigureMediatR();
//builder.Services.DependencyConfiguration();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var appSettings = configuration.AppSettingsConfiguration();
builder.Services.AddGoogleDriveConfiguration(appSettings);
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Document API", Version = "v1" });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Log.Logger.Information($"You are in: Dev.");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    Log.Logger.Information($"You are in: Prod.");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();