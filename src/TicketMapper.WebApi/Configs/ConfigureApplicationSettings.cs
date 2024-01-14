using System.Diagnostics.CodeAnalysis;
using TicketMapper.WebApi.Settings;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class ConfigureApplicationSettings
{
    public static void AppSettingsConfiguration()
    {
        // Build the configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}