using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VideoPlateReader.Data.Repositories
{
    public class ImageFiles : IVideoFiles
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IEnumerable<FileProp> GetImageList(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            var files = info.GetFiles()
                .Where(f => new string[] { ".jpg", ".png", ".gif" }.Contains(f.Extension))

                .OrderBy(p => p.CreationTime).Select(f => new FileProp()
                {
                    FullName = f.FullName,
                    Extension = f.Extension,
                    Name = f.Name,
                    CreationTime = f.CreationTime,
                    CreationTimeUtc = f.CreationTimeUtc,
                    Directory = f.Directory,
                    DirectoryName = f.DirectoryName,
                    LastAccessTime = f.LastAccessTime,
                    LastAccessTimeUtc = f.LastAccessTimeUtc,
                    LastWriteTime = f.LastWriteTime,
                    LastWriteTimeUtc = f.LastWriteTimeUtc,
                    Length = f.Length
                });
            logger.Trace($"Number of files found. {files.Count<FileProp>()} from directory: {path}");
            return files;
            //return Directory.GetFiles(path)
            //    .Where(f => new string[] { ".jpg", ".png", ".gif" }.Contains(Path.GetExtension(f)));
        }

        public IEnumerable<FileProp> GetImageListWithDateTime(string path, DateTime dt)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            var filesprop = info.GetFiles()
                .Where(f => new string[] { ".jpg", ".png", ".gif" }.Contains(f.Extension))
                .Where(f => f.CreationTime > dt)
                .OrderBy(p => p.CreationTime);

            var files = filesprop.Select(f => new FileProp()
            {
                FullName = f.FullName,
                Extension = f.Extension,
                Name = f.Name,
                CreationTime = f.CreationTime,
                CreationTimeUtc = f.CreationTimeUtc,
                Directory = f.Directory,
                DirectoryName = f.DirectoryName,
                LastAccessTime = f.LastAccessTime,
                LastAccessTimeUtc = f.LastAccessTimeUtc,
                LastWriteTime = f.LastWriteTime,
                LastWriteTimeUtc = f.LastWriteTimeUtc,
                Length = f.Length
            });
            //.Select(f => f.FullName);
            logger.Trace($"Number of files found. {files.Count<FileProp>()} with datetime: {dt.ToLongDateString()} from directory: {path}");
            return files;
            
        }
    }
}
