using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VideoPlateReader
{
    public static class  ImageExtension
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Save the image to directory.  Creates new directory by date if requires and saves images in there, returns the new parent path.
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="parentPath">Directory path</param>
        /// <param name="fileName">name of image to save</param>
        /// <param name="ext">the type of file</param>
        /// <returns>returns the new path directory</returns>
        public static string SaveByDate(this BitmapImage image, string parentPath,string fileName,string ext)
        {
            string currentDt = DateTime.Today.ToString("yyyyMMdd");
            
            string fullPath = parentPath + currentDt + "\\";
            string fileFullPath = fullPath + @"\" + fileName;

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            logger.Trace($"ImageExtension.Save(BitmapImage,string,string):{fileFullPath}");
            BitmapEncoder enc = EncoderSelect(ext);

            //BitmapEncoder encoder = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(fileFullPath, System.IO.FileMode.Create))
            {
                enc.Save(fileStream);
            }

            return fullPath;
        }
        public static void Save(this BitmapImage image, string filePath, string ext)
        {
            logger.Trace($"ImageExtension.Save(BitmapImage,string,string):{filePath}");
            BitmapEncoder enc = EncoderSelect(ext);

            //BitmapEncoder encoder = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                enc.Save(fileStream);
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        /// <summary>
        /// Save the image to directory.  Creates new directory by date if requires and saves images in there, returns the new parent path.
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="parentPath">Directory path</param>
        /// <param name="fileName">name of image to save</param>
        /// <param name="ext">the type of file</param>
        /// <returns>returns the new path directory</returns>
        public static string SaveByDate(this BitmapSource image, string parentPath, string fileName, string ext)
        {
            string currentDt = DateTime.Today.ToString("yyyyMMdd");

            string fullPath = parentPath + currentDt + "\\";
            string fileFullPath = fullPath + @"\" + fileName;

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            logger.Trace($"ImageExtension.Save(BitmapSource,string,string):{fileFullPath}");

            BitmapEncoder enc = EncoderSelect(ext);
            //BitmapEncoder encoder = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));
            using (var fileStream = new System.IO.FileStream(fileFullPath, System.IO.FileMode.Create))
            {
                
                enc.Save(fileStream);
            }
            return fullPath;
        }
        public static void Save(this BitmapSource image, string filePath, string ext)
        {
            logger.Trace($"ImageExtension.Save(BitmapSource,string,string):{filePath}");

            BitmapEncoder enc = EncoderSelect(ext);
            //BitmapEncoder encoder = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));
            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {

                enc.Save(fileStream);
            }
        }
        public static List<Byte[]> ToByteArray(this BitmapSource image)
        {
            List<byte[]> img = new List<byte[]>();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.QualityLevel = 100;
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                byte[] bit = stream.ToArray();
                img.Add(bit);
                stream.Close();
            }

            return img;
        }
        private static BitmapEncoder EncoderSelect(string ext)
        {
            BitmapEncoder enc = null;
            switch (ext.ToLower())
            {
                case ".jpg":
                    enc = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    enc = new BmpBitmapEncoder();
                    break;
                case ".gif":
                    enc = new GifBitmapEncoder();
                    break;
                case ".png":
                    enc = new PngBitmapEncoder();
                    break;
                case ".tff":
                    enc = new TiffBitmapEncoder();
                    break;
                case ".wmp":
                    enc = new WmpBitmapEncoder();
                    break;
                default:
                    enc = new JpegBitmapEncoder();
                    break;

            }

            return enc;
        }
        public static void LogMatchedFile(this MatchImage image,string path,string info)
        {
            File.AppendAllText(path, info);
        }
        
    }


}
