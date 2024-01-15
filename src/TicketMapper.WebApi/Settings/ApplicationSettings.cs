using TicketMapper.Domain.Interfaces.Settings;

namespace TicketMapper.WebApi.Settings;

public class ApplicationSettings : IApplicationSettings
{
    public IGoogleDriveSettings GoogleDriveSettings { get; set; } = new GoogleDriveSettings();
}