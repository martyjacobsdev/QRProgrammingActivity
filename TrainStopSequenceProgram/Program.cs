namespace TrainStopSequenceProgram
{
    public class Program
    {
        /// <summary>
        ///  The purpose of this method is to initialize the programming activity.
        /// </summary>
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.Error.WriteLine("There was no sequence file argument.");
            }
            else
            {
                Solution solution = new Solution(args[0]);
            }
        }
    }




    /// <summary>
    ///  The purpose of this class is to provide a <c>Solution</c> for the OSE programming activity.
    /// </summary>
    public class Solution
    {
        public Solution(string filename)
        {
            GetStoppingSequenceDescription(filename);
        }

        /// <summary>
        ///  The purpose of this method is to get the appropriate stopping sequence description.
        /// </summary>
        public void GetStoppingSequenceDescription(string filename)
        {
            List<TrainStation> trainStopSequence = ReadTrainStopSequenceFile(filename);
            int numberOfStations = trainStopSequence.Count;

            if (numberOfStations == 2)
            {
                Console.WriteLine("This train stops at {0} and {1} only", trainStopSequence[0].Name, trainStopSequence[1].Name);
            }

            if (numberOfStations >= 3)
            {
                BuildStoppingSequenceDescription(trainStopSequence);
            }

        }

        /// <summary>
        ///  The purpose of this method perform the file handling of the train stop sequence file.
        /// </summary>
        private List<TrainStation> ReadTrainStopSequenceFile(string filename)
        {
            List<TrainStation> trainStopSequence = new List<TrainStation>();

            try
            {
                using (StreamReader readtext = new StreamReader(filename))
                {
                    string? line;

                    while ((line = readtext.ReadLine()) != null)
                    {
                        string[] trainStop = line.Split(',');
                        trainStopSequence.Add(new TrainStation(trainStop[0].Trim(), System.Convert.ToBoolean(trainStop[1].Trim())));
                    }
                }
            }
            catch 
            {
                Console.Error.WriteLine("Something went wrong reading the file, please check the input and try again.");
            }

            return trainStopSequence;

        }

        /// <summary>
        ///  The purpose of this method is to build the stopping sequence description. 
        /// </summary>
        private void BuildStoppingSequenceDescription(List<TrainStation> trainStopSequence)
        {
            IEnumerable<TrainStation> expressStations = trainStopSequence.Where(station => station.IsStoppingStation == false);

            if (trainStopSequence.All(x => (x.IsStoppingStation == true)))
            {
                Console.WriteLine("This train stops at all stations");
                return;
            }

            if (expressStations.Count() == 1)
            {
                Console.WriteLine("This train stops at all stations except {0}", expressStations.First().Name);
                return;
            }

            if (expressStations.Count() >= 2)
            {

                List<Tuple<int, int>> expressRanges = GetConsecutiveExpressRanges(trainStopSequence);

                ProcessExpressSections(expressRanges, trainStopSequence);

            }

        }

        /// <summary>
        ///  The purpose of this method is to get the express range positions from the train stopping sequence. 
        ///  Ranges are seperated at intermediate and last station stops.  
        /// </summary>
        private List<Tuple<int, int>> GetConsecutiveExpressRanges(List<TrainStation> trainSequence)
        {

            List<Tuple<int, int>> indexRanges = new List<Tuple<int, int>>();
            int? begin = null;
            int? end = null;

            // checks the train sequence to find stopping station ranges.
            for (int i = 0; i < trainSequence.Count; ++i)
            {
                if (trainSequence[i].IsStoppingStation)
                {
                    if (begin == null)
                    {
                        begin = i;
                    }
                    else if (end == null)
                    {
                        end = i;

                        // checks if there is a range between stopping stations, if so adds it.
                        if (end.Value - begin.Value >= 2)
                        {
                            indexRanges.Add(new Tuple<int, int>(begin.Value, end.Value));
                        }
                        // prepares for another range.
                        begin = end;
                        end = null;
                    }
                }

            }

            return indexRanges;
        }

        /// <summary>
        ///  The purpose of this method is to process an unknown amount of express sections. 
        /// </summary>
        private void ProcessExpressSections(List<Tuple<int, int>> expressRanges, List<TrainStation> trainStopSequence)
        {

            if (expressRanges.Count == 0)
            {
                return;
            }

            string? fromStation = trainStopSequence.ElementAt(expressRanges[0].Item1).Name;
            string? toStation = null;

            if (expressRanges.Count == 1)
            {
                toStation = trainStopSequence.ElementAt(expressRanges[0].Item2).Name;
            }
            else
            {
                toStation = trainStopSequence.ElementAt(expressRanges[1].Item2).Name;
            }

            if (fromStation == trainStopSequence[0].Name)
            {
                PrintInitialExpressSection(expressRanges, trainStopSequence, fromStation, toStation);
            }
            else
            {
                PrintNextExpressSection(expressRanges, trainStopSequence, fromStation, toStation);
            }

            if (expressRanges.Count > 2)
            {
                var newExpressRange = expressRanges.GetRange(2, expressRanges.Count - 2);

                ProcessExpressSections(newExpressRange, trainStopSequence);
            }

            return;
        }

        /// <summary>
        ///  The purpose of this method is to format & print an initial express section description. 
        /// </summary>
        private void PrintInitialExpressSection(List<Tuple<int, int>> expressRanges, List<TrainStation> trainStopSequence,
            string? fromStation, string? toStation)
        {
            Console.Write("This train runs express from {0} to {1}", fromStation, toStation);
            if (expressRanges.Count > 1)
            {
                string? intermediateStation = trainStopSequence.ElementAt(expressRanges[1].Item1).Name;
                Console.Write(", stopping only at {0}", intermediateStation);
            }
            Console.WriteLine();

        }

        /// <summary>
        ///  The purpose of this method is to format & print other express section descriptions. 
        /// </summary>
        private void PrintNextExpressSection(List<Tuple<int, int>> expressRanges, List<TrainStation> trainStopSequence,
            string? fromStation, string? toStation)
        {
            Console.Write("then runs express from {0} to {1}", fromStation, toStation);
            if (expressRanges.Count > 1)
            {
                string? intermediateStation = trainStopSequence.ElementAt(expressRanges[1].Item1).Name;
                Console.Write(", stopping only at {0}", intermediateStation);
            }
            Console.WriteLine();

        }

    }

}