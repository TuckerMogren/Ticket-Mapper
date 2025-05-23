using Microsoft.OpenApi.Models;
using TicketMapper.WebApi.Configs;
using TicketMapper.WebApi.Settings; // Added for GoogleDriveSettings
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Example of calling ConfigureLogging correctly
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Set up Serilog here before any logging is done
Log.Logger = builder.Services.ConfigureLogging(configuration).CreateLogger();
builder.Host.UseSerilog();

// Continue with your service and application setup
builder.Services.ConfigureMediatR();
builder.Services.DependencyConfiguration();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Document API", Version = "v1" });
});

// Configure GoogleDriveSettings
builder.Services.Configure<GoogleDriveSettings>(builder.Configuration.GetSection("GoogleDriveSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();