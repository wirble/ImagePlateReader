using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader
{
    public class MatchedImageLog
    {
        private string id;
        private string createdDatetime;
        private string plate;
        private string overallConfidence;
        private string processingTimeMs;
        private string originalImage;
        private string matchedImage;
        private string createdImage;
        private string createdImageFullName;

        public string Id { get => id; set => id = value; }
        public string CreatedDatetime { get => createdDatetime; set => createdDatetime = value; }
        public string Plate { get => plate; set => plate = value; }
        public string OverallConfidence { get => overallConfidence; set => overallConfidence = value; }
        public string ProcessingTimeMs { get => processingTimeMs; set => processingTimeMs = value; }
        public string MatchedImage { get => matchedImage; set => matchedImage = value; }
        public string OriginalImage { get => originalImage; set => originalImage = value; }
        public string CreatedImage { get => createdImage; set => createdImage = value; }
        
        public string CreatedImageFullName { get => createdImageFullName; set => createdImageFullName = value; }
        
    }
}
