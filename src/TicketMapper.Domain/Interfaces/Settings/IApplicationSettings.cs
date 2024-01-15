namespace TicketMapper.Domain.Interfaces.Settings;

public interface IApplicationSettings
{
    public IGoogleDriveSettings GoogleDriveSettings { get; set; }
}