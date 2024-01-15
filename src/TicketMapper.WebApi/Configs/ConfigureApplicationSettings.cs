using System.Diagnostics.CodeAnalysis;
using TicketMapper.Domain.Interfaces.Settings;
using TicketMapper.WebApi.Settings;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class ConfigureApplicationSettings
{
    public static IApplicationSettings AppSettingsConfiguration(this IServiceCollection services)
    {
        // Build the configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return new ApplicationSettings();
    }
}