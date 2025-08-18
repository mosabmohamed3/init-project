using Microsoft.AspNetCore.Http;

namespace REM.Application.Services.Abstraction.FileHandler;

public interface IFileHandler
{
    Task<string> SaveFileAsync(
        IFormFile file, string folder,
        CancellationToken cancellationToken = default
    );
    public bool DeleteFile(string path, string folder);
    public bool DeleteFile(string path);
    Task<string?> MoveFileAsync(
        string? filePath, string folder,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<string>> MoveFilesAsync(
        List<string> filePaths, string folder,
        CancellationToken cancellationToken = default
    );
    Task<string?> UpdateFileAsync(
        string oldPath, string tempPath, string folder,
        CancellationToken cancellationToken = default
    );
    bool FileExists(string path);
}
