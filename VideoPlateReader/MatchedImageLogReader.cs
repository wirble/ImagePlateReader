using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader
{
    public class MatchedImageLogReader
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static IList<MatchedImageLog> Read(string path)
        {
            if (!File.Exists(path))
            {
                logger.Trace($"Plate match doesn't exist, create blank one: {path}");
                File.WriteAllLines(path,new string[] { "" });//just create blank file
            }
            logger.Trace($"Reading plate match file: {path}");
            var file = File.ReadAllLines(path);
            IList<MatchedImageLog> lines = new List<MatchedImageLog>();
            foreach(var line in file)
            {
                
                string[] prop = new string[8];
                if (line.Contains(","))
                {
                    prop = line.Split(',');
                    if (!string.IsNullOrWhiteSpace(prop[0]))
                    {
                        lines.Add(new MatchedImageLog()
                        {
                            Id = prop[0],
                            Plate = prop[1],
                            OriginalImage = prop[2],
                            MatchedImage = prop[3],
                            CreatedImage = prop[4],
                            CreatedDatetime = prop[5],
                            CreatedImageFullName = prop[6],
                            ProcessingTimeMs = prop[7],
                            OverallConfidence = prop[8]


                        });
                    }

                }
                  
            }
            return lines;
        }
        /// <summary>
        /// returns only the entry that is newer than the "log"
        /// </summary>
        /// <param name="path"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static IList<MatchedImageLog> LogDiff(string path,IList<MatchedImageLog> log)
        {
            IList<MatchedImageLog> diff = new List<MatchedImageLog>();
            //just go by lines
            var curLog = Read(path);
            logger.Trace($"Reading plate match file (LogDiff): {path}");
            logger.Trace($"Current Log count: {curLog.Count} VS New Log Count: {log.Count}");
            if (log.Count < curLog.Count)
            {
                for(int i=log.Count;i<curLog.Count;i++)
                {
                    diff.Add(curLog[i]);
                }
            }

            return diff;
        }
    }
}
