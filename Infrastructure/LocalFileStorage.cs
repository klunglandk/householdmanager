using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HouseholdManager.Infrastructure
{
    public class LocalFileStorage : IFileStorage
    {
        public LocalFileStorage(IOptions<FileStorageOptions> options, IHostEnvironment environment)
        {
            _maxFileSizeBytes = options.Value.MaxFileSizeBytes;
            _rootPath = Path.GetFullPath(options.Value.RootPath, environment.ContentRootPath);
            Directory.CreateDirectory(_rootPath);
        }

        private static readonly Regex StoredFileNamePattern = new("^[a-f0-9]{32}\\.(jpg|png|webp|pdf)$", RegexOptions.Compiled);

        private readonly int _maxFileSizeBytes;
        private readonly string _rootPath;

        public async Task<string> SaveAsync(Stream content, AllowedFileKind allowedKind, CancellationToken cancellationToken)
        {
            var buffer = await ReadWithinLimitAsync(content, cancellationToken);

            if (buffer.Length == 0)
            {
                throw new ValidationException("The file is empty.");
            }

            var header = buffer.GetBuffer().AsSpan(0, (int)Math.Min(buffer.Length, 16));
            var fileType = FileTypeDetector.Detect(header);
            if (fileType is null || (allowedKind == AllowedFileKind.Image && !fileType.IsImage))
            {
                var allowed = allowedKind == AllowedFileKind.Image ? "JPEG, PNG or WebP" : "JPEG, PNG, WebP or PDF";
                throw new ValidationException($"Unsupported file type. Allowed types: {allowed}.");
            }

            var storedFileName = Guid.NewGuid().ToString("N") + fileType.Extension;
            buffer.Position = 0;
            await using var file = new FileStream(Path.Combine(_rootPath, storedFileName), FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await buffer.CopyToAsync(file, cancellationToken);
            return storedFileName;
        }

        public Stream? Open(string storedFileName)
        {
            if (!StoredFileNamePattern.IsMatch(storedFileName)) return null;

            var path = Path.Combine(_rootPath, storedFileName);
            if (!File.Exists(path)) return null;

            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        }

        public void Delete(string storedFileName)
        {
            if (!StoredFileNamePattern.IsMatch(storedFileName)) return;

            File.Delete(Path.Combine(_rootPath, storedFileName));
        }

        private async Task<MemoryStream> ReadWithinLimitAsync(Stream content, CancellationToken cancellationToken)
        {
            var buffer = new MemoryStream();
            var chunk = new byte[81920];
            int bytesRead;
            while ((bytesRead = await content.ReadAsync(chunk, cancellationToken)) > 0)
            {
                if (buffer.Length + bytesRead > _maxFileSizeBytes)
                {
                    throw new ValidationException($"The file is too large. Maximum size is {_maxFileSizeBytes / (1024 * 1024)} MB.");
                }
                buffer.Write(chunk, 0, bytesRead);
            }
            return buffer;
        }
    }
}
