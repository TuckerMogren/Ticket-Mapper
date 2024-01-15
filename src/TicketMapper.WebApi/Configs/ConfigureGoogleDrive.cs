using Google.Apis.Drive.v3;
namespace TicketMapper.WebApi.Configs;

public static class ConfigureGoogleDrive
{
    public static DriveService GoogleDriveConfiguration(this ServiceCollection services)
    {
        services.AddSingleton<DriveService>(provider =>
        {
            return new DriveService();
        });
    }
}