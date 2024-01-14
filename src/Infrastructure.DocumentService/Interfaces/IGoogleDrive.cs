using Infrastructure.DocumentService.Data_Models;

namespace Infrastructure.DocumentService.Interfaces;

public interface IGoogleDrive
{
    // Uploads a file to Google Drive.
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string mimeType, string folderId = null);

    // Downloads a file from Google Drive.
    Task<Stream> DownloadFileAsync(string fileId);

    // Deletes a file from Google Drive.
    Task DeleteFileAsync(string fileId);

    // Retrieves metadata of a file from Google Drive.
    Task<GoogleDriveFile> GetFileMetadataAsync(string fileId);

    // Lists files in a specified folder.
    Task<IEnumerable<GoogleDriveFile>> ListFilesInFolderAsync(string folderId);
}