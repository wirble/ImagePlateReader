using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader.Event
{
    public class MatchedPlateSelectedEvent:PubSubEvent<MatchedImageLog>
    {
    }
}
