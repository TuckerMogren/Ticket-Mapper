using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using TicketMapper.Domain.Interfaces.Settings;

namespace TicketMapper.WebApi.Configs
{
    public static class ConfigureGoogleDrive
    {
        public static void AddGoogleDriveConfiguration(this IServiceCollection services, IApplicationSettings applicationSettings)
        {
            services.AddSingleton(async provider =>
            {
                var credential = await GetUserCredentialAsync();
                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationSettings.GoogleDriveSettings.ApplicationName,
                });
            });
        }

        private static async Task<UserCredential> GetUserCredentialAsync()
        {
            using var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read);
            string credPath = "token.json"; // Path to store the user token

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
        }
    }
}