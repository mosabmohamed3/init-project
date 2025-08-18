using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using REM.Application.Services.Abstraction.FileHandler;

namespace REM.Application.Services.Implementation;

public class FileHandler(IWebHostEnvironment env) : IFileHandler
{
    private readonly IWebHostEnvironment _env = env;

    public async Task<string> SaveFileAsync(
        IFormFile file, string folder,
        CancellationToken cancellationToken = default
    )
    {
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var directory = Path.Combine(_env.WebRootPath, "Files", folder);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var path = Path.Combine(_env.WebRootPath, "Files", folder, fileName);

        using var fileStream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(fileStream, cancellationToken);

        return $"Files/{folder}/{fileName}";
    }

    public bool DeleteFile(string path, string folder)
    {
        var rootpath = $"{_env.WebRootPath}\\Files\\{folder}";
        var files = Directory.GetFiles(rootpath);

        if (files.Length > 0)
        {
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                fileInfo.Delete();
            }
            return true;
        }

        return false;
    }

    public bool DeleteFile(string path)
    {
        var _path = $"{_env.WebRootPath}\\{path}";

        if (!File.Exists(_path))
            return false;

        File.Delete(_path);
        return true;
    }

    public async Task<string?> MoveFileAsync(
        string? filePath,
        string folder,
        CancellationToken cancellationToken = default
    )
    {
        if (
            !string.IsNullOrWhiteSpace(filePath)
            && filePath.Contains("TempFiles")
            && FileExists(filePath)
        )
        {
            var fileName = Path.GetFileName(filePath);
            var rootpath = $"{_env.WebRootPath}";
            var oldPath = $"{rootpath}\\{filePath}";
            var newPath = $"{rootpath}\\Files\\{folder}\\{fileName}";

            if (!Directory.Exists($"{rootpath}\\Files\\{folder}"))
                Directory.CreateDirectory($"{rootpath}\\Files\\{folder}");

            await Task.Factory.StartNew(() => File.Move(oldPath, newPath), cancellationToken);
            return $"Files\\{folder}\\{fileName}";
        }

        return filePath;
    }

    public async Task<IEnumerable<string>> MoveFilesAsync(
        List<string> filePaths, string folder,
        CancellationToken cancellationToken = default
    )
    {
        var newPaths = new List<string>();

        foreach (var filePath in filePaths)
        {
            var newPath = await MoveFileAsync(filePath, folder, cancellationToken);
            if (newPath != null)
                newPaths.Add(newPath);
        }

        return newPaths;
    }

    public async Task<string?> UpdateFileAsync(
        string oldPath, string tempPath, string folder,
        CancellationToken cancellationToken = default
    )
    {
        if (oldPath == tempPath)
            return oldPath;

        var newPath = await MoveFileAsync(tempPath, folder, cancellationToken);
        if (oldPath != null)
            DeleteFile(oldPath);

        return newPath;
    }

    public bool FileExists(string path)
    {
        var _path = $"{_env.WebRootPath}\\{path}";
        return File.Exists(_path);
    }
}
