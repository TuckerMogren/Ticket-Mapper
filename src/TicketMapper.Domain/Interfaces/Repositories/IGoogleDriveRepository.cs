using System.IO;
using System.Threading.Tasks;

namespace TicketMapper.Domain.Interfaces.Repositories;

public interface IGoogleDriveRepository
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType);
    Task DeleteFileAsync(string fileId);
}