using System.Diagnostics.CodeAnalysis;
using DocumentFormat.OpenXml.Spreadsheet;
using TicketMapper.Domain.Interfaces.Settings;
using TicketMapper.WebApi.Settings;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class ConfigureApplicationSettings
{
    public static IApplicationSettings AppSettingsConfiguration(this IConfiguration config)
    {
        if(config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        var applicationSettings = new ApplicationSettings();

        var test = applicationSettings;

        config.Bind(applicationSettings);


        return applicationSettings;
    }
}