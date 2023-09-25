using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainStopSequenceProgram
{
    public class TrainStation
    {

        public TrainStation(string? name, bool isStoppingStation)
        {
            Name = name;
            IsStoppingStation = isStoppingStation;
        }

        public string? Name { get; set; }
        public bool IsStoppingStation { get; set; }
    }
}
