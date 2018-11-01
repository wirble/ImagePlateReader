using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader
{
    public class FileProp
    {
        private string name;
        private string extension;
        private string fullName;
        private DateTime lastWriteTime;
        private DateTime lastWriteTimeUtc;
        private DateTime creationTime;
        private DateTime creationTimeUtc;
        private DirectoryInfo directory;
        private string directoryName;
        private long length;
        private DateTime lastAccessTime;
        private DateTime lastAccessTimeUtc;
        private string foundPlatePath;
        private int h;

  
        public string Name { get => name; set => name = value; }
        public string FullName { get => fullName; set => fullName = value; }
        public DateTime LastWriteTime { get => lastWriteTime; set => lastWriteTime = value; }
        public DateTime LastWriteTimeUtc { get => lastWriteTimeUtc; set => lastWriteTimeUtc = value; }
        public DateTime CreationTime { get => creationTime; set => creationTime = value; }
        public DateTime CreationTimeUtc { get => creationTimeUtc; set => creationTimeUtc = value; }
        public DirectoryInfo Directory { get => directory; set => directory = value; }
        public string DirectoryName { get => directoryName; set => directoryName = value; }
        public long Length { get => length; set => length = value; }
        public DateTime LastAccessTime { get => lastAccessTime; set => lastAccessTime = value; }
        public DateTime LastAccessTimeUtc { get => lastAccessTimeUtc; set => lastAccessTimeUtc = value; }
        public string Extension { get => extension; set => extension = value; }
        public string FoundPlatePath { get => foundPlatePath; set => foundPlatePath = value; }
        public int H { get => h; set => h = value; }

        public override string ToString()
        {

            return $"{name},{CreationTime},{fullName}";

        }
    }
}
