
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using VideoPlateReader.Object;

namespace VideoPlateReader
{
    public class MatchImage
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public MatchImage()
        {
            logger.Trace($"Constructor: MatchImage()");
            MatchedPlates = new List<MatchedPlateProp>();
            PlateCount = 0;
            ProcessingTimeMs = 0.0f;
            Id = Guid.NewGuid().ToString();//creates unique id for new matchedimage
        }
        private string id;
        private BitmapImage originalImage;
        private BitmapSource matchedImage;
        private IList<MatchedPlateProp> matchedPlates;
        private FileProp properties;
        private string json;
        private int plateCount;
        private float processingTimeMs;

        public BitmapImage OriginalImage { get => originalImage; set => originalImage = value; }
        public BitmapSource MatchedImage { get => matchedImage; set => matchedImage = value; }
        public FileProp Properties { get => properties; set => properties = value; }
        public int PlateCount { get => plateCount; set => plateCount = value; }
        public string Json { get => json; set => json = value; }
        public IList<MatchedPlateProp> MatchedPlates { get => matchedPlates; set => matchedPlates = value; }
        public float ProcessingTimeMs { get => processingTimeMs; set => processingTimeMs = value; }
        public string Id { get => id; set => id = value; }

        public string GetAllPlatesString()
        {
            string p = string.Empty;
            foreach (var plate in this.MatchedPlates)
            {
                if (!string.IsNullOrWhiteSpace(plate.Plate))
                    p += plate.Plate.Trim() + "-";
                
            }
            if (p.EndsWith("-"))
                p = p.Substring(0, p.Length - 1);
            return p;
        }
        public string GetAllConfidenceLevelString()
        {
            string p = string.Empty;
            foreach (var plate in this.MatchedPlates)
            {
                if (!string.IsNullOrWhiteSpace(plate.OverallConfidence.ToString()))
                    p += plate.OverallConfidence.ToString() + "-";

            }
            if (p.EndsWith("-"))
                p = p.Substring(0, p.Length - 1);
            return p;
        }
        public override string ToString()
        {
            string p = GetAllPlatesString();
            return $"{Id},{p},{Properties.FoundPlatePath}{p}.{Properties.CreationTime.Ticks.ToString()}.o{Properties.Extension},{Properties.FoundPlatePath}{p}.{Properties.CreationTime.Ticks.ToString()}.m{Properties.Extension},{Properties.ToString()},{ProcessingTimeMs},{GetAllConfidenceLevelString()}";
        }
    }
}
