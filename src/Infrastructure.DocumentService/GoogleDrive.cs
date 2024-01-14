using Infrastructure.DocumentService.Data_Models;
using Infrastructure.DocumentService.Interfaces;

namespace Infrastructure.DocumentService;

public class GoogleDrive : IGoogleDrive
{
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string mimeType, string folderId = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Stream> DownloadFileAsync(string fileId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteFileAsync(string fileId)
    {
        throw new NotImplementedException();
    }

    public async Task<GoogleDriveFile> GetFileMetadataAsync(string fileId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GoogleDriveFile>> ListFilesInFolderAsync(string folderId)
    {
        throw new NotImplementedException();
    }
}