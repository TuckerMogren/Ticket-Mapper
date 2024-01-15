namespace TicketMapper.Domain.Interfaces.Settings;

public interface IGoogleDriveSettings 
{ 
    string ClientId { get; set; }
    string ProjectId { get; set; }
    string AuthUrl { get; set; }
    string TokenUri { get; set; }
    string AuthProviderCertUrl { get; set; }
    string ClientSecret { get; set; }
}