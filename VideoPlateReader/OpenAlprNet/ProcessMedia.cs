using openalprnet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using VideoPlateReader.Object;

namespace VideoPlateReader
{
    public class ProcessMedia
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private IList<MatchImage> matchedList = new List<MatchImage>();
        private static Rectangle BoundingRectangle(List<System.Drawing.Point> points)
        {
            // Add checks here, if necessary, to make sure that points is not null,
            // and that it contains at least one (or perhaps two?) elements

            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxX = points.Max(p => p.X);
            var maxY = points.Max(p => p.Y);

            return new Rectangle(new System.Drawing.Point(minX, minY), new System.Drawing.Size(maxX - minX, maxY - minY));
        }

        //private static System.Drawing.Image CropImage(System.Drawing.Image img, Rectangle cropArea)
        //{
        //    var bmpImage = new Bitmap(img);
        //    return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        //}
        private static System.Drawing.Image CropImage(System.Drawing.Image img, Rectangle cropArea)
        {
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics gph = Graphics.FromImage(bmp))
            {
                gph.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmp;
        }
        public static Bitmap CombineImages(List<System.Drawing.Image> images)
        {
            //read all images into memory
            Bitmap finalImage = null;

            try
            {
                var width = 0;
                var height = 0;

                foreach (var bmp in images)
                {
                    width += bmp.Width;
                    height = bmp.Height > height ? bmp.Height : height;
                }

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (var g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(Color.Black);

                    //go through each image and draw it on the final image
                    var offset = 0;
                    foreach (Bitmap image in images)
                    {
                        g.DrawImage(image,
                                    new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "CombineImages()");
                if (finalImage != null)
                    finalImage.Dispose();

                throw ex;
            }
            finally
            {
                //clean up memory
                foreach (var image in images)
                {
                    image.Dispose();
                }
            }
        }
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }


       
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private static BitmapSource Bitmap2BitmapImage(Bitmap image)
        {
            var bitmap = new Bitmap(image);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }

        //public IList<MatchImage> ProcessImageFile()
        //{
        //    return ProcessImageFile(this.GetListOfFiles(this.InitialFolder));
        //}

        //public static MatchImage ProcessImageFile(string fileName,string regionUS="us")
        //{
        //    MatchImage image = ProcessImageHelper(fileName, regionUS);
        //    image.Properties = new FileProp() { FullName = fileName };
        //    return image;
        //}
        /// <summary>
        /// helper class does actual grunt work of matching plates
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="regionUS"></param>
        /// <returns></returns>
        private static MatchImage ProcessImageFile(string fileName, string regionUS="us")
        {
            logger.Trace($"MatchImage ProcessImageFile(string fileName, string regionUS='us'): {fileName},{regionUS}");
            MatchImage matchedImage = new MatchImage();
            
            try
            {
                logger.Trace($"alpr match image: {fileName}");
                
                matchedImage.Properties = new FileProp() { FullName = fileName };
                var region = regionUS ?? "us";
                String config_file = Path.Combine(AssemblyDirectory, "openalpr.conf");
                String runtime_data_dir = Path.Combine(AssemblyDirectory, "runtime_data");
                using (var alpr = new AlprNet(region, config_file, runtime_data_dir))
                {
                    if (!alpr.IsLoaded())
                    {
                        logger.Error("alpr loaded error.");
                        throw new ArgumentException("Error initializing OpenALPR");

                    }
                    matchedImage.OriginalImage = new BitmapImage(new Uri(Path.Combine(AssemblyDirectory, fileName)));

                    var results = alpr.Recognize(fileName);
                    matchedImage.Json = results.Json;
                    matchedImage.PlateCount = results.Plates.Count;
                    matchedImage.ProcessingTimeMs = results.TotalProcessingTimeMs;
                    
                    var images = new List<System.Drawing.Image>(results.Plates.Count());
                    var i = 1;
                    
                    foreach (var result in results.Plates)
                    {
                        var rect = BoundingRectangle(result.PlatePoints);
                        var img = System.Drawing.Image.FromFile(fileName);
                        var cropped = CropImage(img, rect);
                        images.Add(cropped);

                        //image.MatchedPlates.Add("\t\t-- Plate #" + i++ + " --");
                        foreach (var plate in result.TopNPlates.Take(1))
                        {
                            matchedImage.MatchedPlates.Add(new MatchedPlateProp()
                                                            {
                                                                Plate =string.Format(@"{0}", //{1}% {2}",
                                                                plate.Characters.PadRight(12)),
                                                                OverallConfidence = plate.OverallConfidence
                                                            }
                            );
                            //plate.OverallConfidence.ToString("N1").PadLeft(8),
                            //plate.MatchesTemplate.ToString().PadLeft(8)));
                        }
                    }

                    if (images.Any())
                    {
                        matchedImage.MatchedImage = Bitmap2BitmapImage(CombineImages(images));
                    }
                }
                logger.Trace($"Return from alpr image match: {matchedImage.Properties.Name}");
            }catch(Exception ex)
            {
                logger.Error(ex, "Error matching plates in alpr.");
            }
            return matchedImage;
        }

        /// <summary>
        /// match a FileProp so we can get other properties from the original files
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="regionUS"></param>
        /// <returns></returns>
        public static MatchImage ProcessImageFile(FileProp prop,string regionUS = "us") {
            MatchImage image = new MatchImage();
            try { 
                logger.Trace($"ProcessImageFile(FileProp,string) BEFORE: {prop.FullName}, region: {regionUS}");
                String config_file = Path.Combine(AssemblyDirectory, "openalpr.conf");
                String runtime_data_dir = Path.Combine(AssemblyDirectory, "runtime_data");
                logger.Trace($"ProcessImageFile(FileProp,string): {config_file}, {runtime_data_dir}");
                image = ProcessMedia.ProcessImageFile(prop.FullName, regionUS);
                logger.Trace($"ProcessImageFile(FileProp,string) AFTER: {prop.FullName}, region: {regionUS}");
                image.Properties = prop;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ProcessImageFile(FileProp,string)");
            }
            return image;
        }
        /// <summary>
        /// Takes a list of fullname (path and filename) and match the images
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        public static IList<MatchImage>  ProcessImageFiles(IList<string> filenames)
        {
            logger.Trace($"ProcessImageFiles(IList<string>): {filenames}");
            IList<MatchImage> plates = new List<MatchImage>();
            try { 
                foreach(var file in filenames)
                {
                    plates.Add(ProcessImageFile(file));
                }
            }catch(Exception ex)
                {
                    logger.Error(ex, "ProcessImageFiles(IList<FileProp>)");
                }
            return plates;
        }
        /// <summary>
        /// takes a list of FileProp and process, this way we also get other properties from the orginal files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static IList<MatchImage> ProcessImageFiles(IList<FileProp> files)
        {
            logger.Trace($"ProcessImageFiles(IList<FileProp>) BEFORE: TOTAL FILES {files.Count}");
            IList<MatchImage> plates = new List<MatchImage>();
            try
            {
                foreach (var file in files)
                {
                    logger.Trace($"ProcessImageFiles(IList<FileProp>) INLOOP: {file.Name}");
                    plates.Add(ProcessImageFile(file));
                }
                logger.Trace($"ProcessImageFiles(IList<FileProp>) BEFORE: TOTAL FILES {files.Count}");
            }catch(Exception ex)
            {
                logger.Error(ex, "ProcessImageFiles(IList<FileProp>)");
            }
            return plates;
        }

    }
}
