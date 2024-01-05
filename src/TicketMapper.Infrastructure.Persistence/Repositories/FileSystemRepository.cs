using System.IO.Abstractions;

namespace TicketMapper.Infrastructure.Persistence.Repositories;

public class FileSystemRepository : IFileSystem
{
    public FileSystemRepository(IDirectory directory, IDirectoryInfoFactory directoryInfo, IDriveInfoFactory driveInfo, IFile file, IFileInfoFactory fileInfo, IFileStreamFactory fileStream, IFileSystemWatcherFactory fileSystemWatcher, IPath path)
    {
        Directory = directory;
        DirectoryInfo = directoryInfo;
        DriveInfo = driveInfo;
        File = file;
        FileInfo = fileInfo;
        FileStream = fileStream;
        FileSystemWatcher = fileSystemWatcher;
        Path = path;
    }

    public IDirectory Directory { get; }
    public IDirectoryInfoFactory DirectoryInfo { get; }
    public IDriveInfoFactory DriveInfo { get; }
    public IFile File { get; }
    public IFileInfoFactory FileInfo { get; }
    public IFileStreamFactory FileStream { get; }
    public IFileSystemWatcherFactory FileSystemWatcher { get; }
    public IPath Path { get; }
}