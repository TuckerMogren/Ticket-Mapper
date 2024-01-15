using TicketMapper.Domain.Interfaces.Settings;

namespace TicketMapper.WebApi.Settings;

public class GoogleDriveSettings()
    : IGoogleDriveSettings
{
    public string ClientId { get; set; }
    public string ProjectId { get; set; }
    public string AuthUrl { get; set; }
    public string TokenUri { get; set; }
    public string AuthProviderCertUrl { get; set; }
    public string ClientSecret { get; set; }
    public string ApplicationName { get; set; }
}