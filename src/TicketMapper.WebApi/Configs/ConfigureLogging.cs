using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class LoggingConfigure
{
    public static LoggerConfiguration ConfigureLogging(this IServiceCollection collection, IConfiguration configuration)
    {
        return new LoggerConfiguration().ReadFrom.Configuration(configuration);
    }
}