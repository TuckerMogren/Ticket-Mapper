using Google.Apis.Drive.v3;
namespace TicketMapper.WebApi.Configs;

public static class ConfigureGoogleDrive
{
    public static DriveService GoogleDriveConfiguration(this ServiceCollection services)
    {
        services.AddSingleton<DriveService>(provider => 
        {
            // Create and configure your DriveService instance here
            UserCredential credential = // ... obtain credentials ...
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveAPI",
            });
            
        });
    }
}