using Google.Apis.Drive.v3;
using TicketMapper.Infrastructure.DocumentService.Data_Models;
using TicketMapper.Infrastructure.DocumentService.Interfaces;

namespace TicketMapper.Infrastructure.DocumentService;

public class GoogleDrive(DriveService driveService) : IGoogleDrive
{
    private readonly DriveService _driveService = driveService;
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string mimeType, string? folderId = null)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileName,
            Parents = string.IsNullOrEmpty(folderId) ? null : new List<string> { folderId }
        };

        FilesResource.CreateMediaUpload request;
        request = _driveService.Files.Create(fileMetadata, fileStream, mimeType);
        request.Fields = "id";
        await request.UploadAsync();

        var file = request.ResponseBody;
        return file?.Id;
    }

    public async Task<Stream> DownloadFileAsync(string fileId)
    {
        var request = _driveService.Files.Get(fileId);
        var memoryStream = new MemoryStream();
        await request.DownloadAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream position to the beginning
        return memoryStream;
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var request = _driveService.Files.Delete(fileId);
        await request.ExecuteAsync();
    }
    
    public async Task<GoogleDriveFile> GetFileMetadataAsync(string fileId)
    {
        var request = _driveService.Files.Get(fileId);
        request.Fields = "id, name, mimeType";
        var file = await request.ExecuteAsync ();

        return new GoogleDriveFile(file.Id, file.Name, file.MimeType);
    }

    public async Task<IEnumerable<GoogleDriveFile>> ListFilesInFolderAsync(string folderId)
    {
        var request = _driveService.Files.List();
        request.Q = $"mimeType != 'application/vnd.google-apps.folder' and '{folderId}' in parents";
        request.Fields = "files(id, name, mimeType)";

        var result = await request.ExecuteAsync();
        var files = new List<GoogleDriveFile>();

        foreach (var file in result.Files)
        {
            files.Add(new GoogleDriveFile(file.Id, file.Name, file.MimeType));
        }

        return files;
    }
}