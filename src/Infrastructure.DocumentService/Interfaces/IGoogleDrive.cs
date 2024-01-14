using Infrastructure.DocumentService.Data_Models;

namespace Infrastructure.DocumentService.Interfaces;

public interface IGoogleDrive
{
    /// <summary>
    /// Asynchronously uploads a file to Google Drive.
    /// </summary>
    /// <param name="fileStream">The stream of the file to be uploaded.</param>
    /// <param name="fileName">The name of the file to be uploaded.</param>
    /// <param name="mimeType">The MIME type of the file.</param>
    /// <param name="folderId">The ID of the folder to upload the file to. If null, the file will be uploaded to the root directory.</param>
    /// <returns>The ID of the uploaded file on Google Drive.</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string mimeType, string? folderId = null);

    /// <summary>
    /// Asynchronously downloads a file from Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file to be downloaded.</param>
    /// <returns>A stream containing the contents of the file.</returns>
    Task<Stream> DownloadFileAsync(string fileId);

    /// <summary>
    /// Asynchronously deletes a file from Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file to be deleted.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteFileAsync(string fileId);

    /// <summary>
    /// Asynchronously retrieves the metadata of a file from Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file for which metadata is to be retrieved.</param>
    /// <returns>A <see cref="GoogleDriveFile"/> object containing the file's metadata.</returns>
    Task<GoogleDriveFile> GetFileMetadataAsync(string fileId);

    /// <summary>
    /// Asynchronously lists the files in a specified folder on Google Drive.
    /// </summary>
    /// <param name="folderId">The ID of the folder whose files are to be listed. If null, files in the root directory will be listed.</param>
    /// <returns>A collection of <see cref="GoogleDriveFile"/> objects representing the files in the specified folder.</returns>
    Task<IEnumerable<GoogleDriveFile>> ListFilesInFolderAsync(string folderId);
}
