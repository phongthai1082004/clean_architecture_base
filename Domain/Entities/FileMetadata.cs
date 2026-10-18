using Domain.Abstractions.Entities;

namespace Domain.Entities
{
    public class FileMetadata : GuidEntity
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public string ContentType { get; private set; }
        public long FileSize { get; private set; }

        internal FileMetadata(string fileName, string filePath, string contentType, long fileSize)
        {
            FileName = fileName;
            FilePath = filePath;
            ContentType = contentType;
            FileSize = fileSize;
        }

        public bool IsImage() => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        public bool IsPdf() => ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);
    }
}
