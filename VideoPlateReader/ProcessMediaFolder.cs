using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VideoPlateReader.Data.Repositories;
using VideoPlateReader.Event;

namespace VideoPlateReader
{
    public class ProcessMediaFolder
    {
        private string initialFolder;
        private string initialRegion;
        private string initialLastFileProcess;
        private string initialMatchedImageLog;
        private string foundPlatesPath;
        private IList<FileProp> filesList = new List<FileProp>();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private IList<MatchImage> processedImages = new List<MatchImage>();
        private IEventAggregator EventAggregator;
        public ProcessMediaFolder() : this("", "", new EventAggregator()) { }
        public ProcessMediaFolder(string path, string region,IEventAggregator eventAggregator)
        {

            this.EventAggregator = eventAggregator;
            logger.Trace($"Constructor: ProcessMediaFolder(string path, string region): {path}, {region}");
            this.InitialFolder = path ??  Path.Combine(ProcessMedia.AssemblyDirectory, ConfigurationManager.AppSettings["initialFolder"] ?? @"Data\");
            this.FoundPlatesPath = Path.Combine(ProcessMedia.AssemblyDirectory, ConfigurationManager.AppSettings["foundPlatesPath"] ?? @"Data\FoundPlates\");
            this.InitialRegion = region ?? ConfigurationManager.AppSettings["initialRegion"] ?? "us";
            this.FilesList = GetListOfFiles(path??this.initialFolder);
            this.InitialLastFileProcess = Path.Combine(ProcessMedia.AssemblyDirectory, ConfigurationManager.AppSettings["initialLastFileProcess"] ?? @"Data\LasFileProcessed.txt");
            this.InitialMatchedImageLog = Path.Combine(ProcessMedia.AssemblyDirectory, ConfigurationManager.AppSettings["initialMatchedImageLog"] ?? $"Data\\MatchedImageLog.txt");
        }
        //private void ArchiveMatchedImageLogByDate(string path) {
        //    string currentDay = DateTime.Today.ToString("YYYYMMdd");
        //    string pathDate = Regex.Match(path, @"\d+").Value;
        //    if(currentDay!=pathDate)
        //        this.InitialMatchedImageLog = Path.Combine(ProcessMedia.AssemblyDirectory, $"Data\\MatchedImageLog{currentDay}.txt");

        //}

        /// <summary>
        /// process all images in folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IList<FileProp> GetListOfFiles(string path)
        {

            logger.Trace($"IList<FileProp> GetListOfFiles(string path): {path}");
            IVideoFiles files = new ImageFiles();
            return files.GetImageList(path).ToList();
        }
        /// <summary>
        /// process images from current time
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IList<FileProp> GetListOfFilesWithDateTime(string path)
        {
            logger.Trace($"IList<FileProp> GetListOfFilesWithDateTime(string path): {path}");
            IVideoFiles files = new ImageFiles();
            DateTime curDt = GetLastProcessedDateTime();
            
            SetLastProcessedDateTime(DateTime.Now.AddDays(-1).ToString());
            return files.GetImageListWithDateTime(path,curDt).ToList();
        }

        public IList<MatchImage> MatchPlates()
        {
            //if (FilesList == null || FilesList.Count <= 0)
            //    throw new ArgumentException("Can't process or match plates if there are no files.");
            logger.Trace($"IList<MatchImage> MatchPlates()");
            return ProcessMedia.ProcessImageFiles(FilesList);
        }

        /// <summary>
        /// loops through processing constantly for new files
        /// </summary>
        /// <returns></returns>
        public async Task<IList<MatchImage>> MatchPlatesWithDateTimeAsync()
        {
            logger.Trace($"Task<IList<MatchImage>> MatchPlatesWithDateTimeAsync() BEFORE");
            IList<MatchImage> plates = new List<MatchImage>();
            try
            {
                this.FilesList = GetListOfFilesWithDateTime(this.InitialFolder);
                SetLastProcessedDateTime(DateTime.Now.ToString());
                plates =  await Task.Run(()=>MatchPlates());
            }
            catch (ArgumentException ex) {
                logger.Error(ex, "MatchPlatesWithDateTimeAsync(), ArgumentException");
            }
            catch(Exception ex)
            {
                logger.Error(ex, "MatchPlatesWithDateTimeAsync(), Exception");
            }
            logger.Trace($"Task<IList<MatchImage>> MatchPlatesWithDateTimeAsync() AFTER");
            return plates;

        }
        public async Task ProcessAndMonitorFolderAsync()
        {
            int count = 0;
            while (true)
            {

                try
                {

                    
                    
                    this.ProcessedImages = await  this.MatchPlatesWithDateTimeAsync();
                    logger.Trace($"Processed Images: {processedImages.Count}");
                    foreach(var file in this.ProcessedImages)
                    {
                        logger.Trace($"Matched image: {file.Properties.Name}");
                        file.Properties.FoundPlatePath = this.foundPlatesPath; //set the directory of where the matched image will go
                        string firstMatched = file.GetAllPlatesString().Trim();
                        //foreach (var f in file.MatchedPlates)
                        //{
                        //    if (!string.IsNullOrEmpty(f.Plate))
                        //        firstMatched += f;
                        //}
                        //firstMatched = firstMatched.Trim();
                        if (!string.IsNullOrWhiteSpace(firstMatched))
                        {

                            string tickTime = firstMatched + "." + file.Properties.CreationTime.Ticks.ToString();
                            file.Properties.FoundPlatePath = file.MatchedImage.SaveByDate(this.FoundPlatesPath,tickTime + ".m" + file.Properties.Extension,file.Properties.Extension);//write to disk
                            file.Properties.FoundPlatePath = file.OriginalImage.SaveByDate(this.FoundPlatesPath,tickTime + ".o" + file.Properties.Extension, file.Properties.Extension);
                            file.LogMatchedFile(this.initialMatchedImageLog, file.ToString() + Environment.NewLine);

                            this.EventAggregator.GetEvent<MatchedPlateEvent>().Publish(null);

                            string toEmail = ConfigurationManager.AppSettings["toEmail"];
                            string fromEmail = ConfigurationManager.AppSettings["fromEmail"];
                            string isSendEmail = ConfigurationManager.AppSettings["isSendEmail"].ToLower();
                            Task<bool> success = null;
                            if(!string.IsNullOrWhiteSpace(toEmail) && isSendEmail=="true")
                               success = EmailMessage.SendMsgWithInlinePicturesAsync(toEmail, fromEmail, file.GetAllPlatesString(), "test","","","","", file.MatchedImage.ToByteArray());
                            logger.Trace($"Send email: {success}, {file.ToString()}");
                        }
                    }
                    if (processedImages.Count == 0)
                        count++;

                    logger.Trace($"Number of process count: {count}");
                    
                }
                catch (InvalidOperationException ex)
                { //cancelled

                    logger.Error(ex, $"ProcessAndMonitorFolder() error");
                    
                }
                if (count >= 5)
                {
                    logger.Trace($"Sleep no picture to process.");
                    await Task.Delay(5000);
                    count = 0;
                }

            }
        }
        private void SetLastProcessedDateTime(string datetime)
        {
            //if (!File.Exists(InitialLastFileProcess))
            //using (StreamWriter w = new StreamWriter(initialLastFileProcess,true))
            //{
            //    w.WriteLine(datetime);
            //}
            File.WriteAllText(this.InitialLastFileProcess, datetime);
            
            
        }
        /// <summary>
        /// no datetime found, set current datetime, only get list from current time and on
        /// </summary>
        /// <returns></returns>
        private DateTime GetLastProcessedDateTime()
        {
            string curDt = string.Empty;
            if (File.Exists(InitialLastFileProcess))
                curDt = File.ReadAllText(InitialLastFileProcess);
            curDt = string.IsNullOrWhiteSpace(curDt) ?DateTime.Now.AddHours(-24).ToString():curDt;
            return Convert.ToDateTime(curDt);
        }
        public string InitialFolder { get => initialFolder; set => initialFolder = value; }
        public string InitialRegion { get => initialRegion; set => initialRegion = value; }
        public IList<FileProp> FilesList { get => filesList; set => filesList = value; }
        public string InitialLastFileProcess { get => initialLastFileProcess; set => initialLastFileProcess = value; }
        public string FoundPlatesPath { get => foundPlatesPath; set => foundPlatesPath = value; }
        public string InitialMatchedImageLog { get => initialMatchedImageLog; set => initialMatchedImageLog = value; }
        public IList<MatchImage> ProcessedImages { get => processedImages; set => processedImages = value; }
    }
}
