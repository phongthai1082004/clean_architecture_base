using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
