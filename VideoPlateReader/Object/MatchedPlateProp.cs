using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader.Object
{
    public class MatchedPlateProp
    {
        public MatchedPlateProp()
        {
            Plate = string.Empty;
            OverallConfidence = 0.0f;
        }
        private string plate;
        private float overallConfidence;

        public string Plate { get => plate; set => plate = value; }
        public float OverallConfidence { get => overallConfidence; set => overallConfidence = value; }

    }
}
