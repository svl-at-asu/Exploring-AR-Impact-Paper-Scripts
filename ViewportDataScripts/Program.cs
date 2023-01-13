using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewportDataScripts
{
    internal class Program
    {
        /// <summary>
        /// Args are expected in the following order: Number of teams, Input data folder, Device ID, Trial data filepath, Video times data filepath, Output folder.
        /// </summary>
        private enum ArgsIndices
        {
            NumberOfTeams = 0,
            InputDataFolder = 1,
            DeviceID = 2,
            TrialData = 3,
            VideoTimesData = 4,
            OutputFolder = 5
        }

        /// <summary>
        /// The file name for the output validation log.
        /// </summary>
        private static readonly string OutputLogFileName = "ValidationLog.txt";

        static void Main(string[] args)
        {
            // Initialize the issues set.
            Dictionary<string, List<string>> issues = new Dictionary<string, List<string>>();

            // Read the trial table data file.
            string[][] trialData = null;
            try
            {
                trialData = DataReader.ReadCsvData(args[(int)Program.ArgsIndices.TrialData]);
            }
            catch (Exception ex)
            {
                // An error occured while trying to read the file data. Add an issue to the list.
                issues.Add("Trial Data", new List<string>()
                {
                    "Trial Data table error - " + ex.Message
                });
            }

            // Read the video times table data file.
            string[][] videoTimesData = null;
            try
            {
                videoTimesData = DataReader.ReadCsvData(args[(int)Program.ArgsIndices.VideoTimesData]);
            }
            catch (Exception ex)
            {
                // An error occured while trying to read the file data. Add an issue to the list.
                issues.Add("Video Times Data", new List<string>()
                {
                    "Video Times Data table error - " + ex.Message
                });
            }

            // Get the number of teams.
            int numTeams = 0;
            bool parseSuccess = int.TryParse(args[(int)Program.ArgsIndices.NumberOfTeams], out numTeams);
            if (parseSuccess == false)
            {
                // An error occured while trying to parse the number of teams. Add an issue to the list.
                issues.Add("Program Args", new List<string>()
                {
                    "Program Args error - program args at index " + (int)Program.ArgsIndices.NumberOfTeams + " is not in the expected format."
                });
            }

            // For each team...
            for (int index = 1; index < numTeams; index++)
            {
                // Assemble the file path.
                string filePath = args[(int)Program.ArgsIndices.InputDataFolder];

                // Ensure the path ends in a slash before appending the file name.
                if (filePath[filePath.Length - 1] != '\\')
                {
                    filePath += "\\";
                }
                string fileName = "Team" + index + "_Hololens_datalog.csv";
                filePath += fileName;

                // Read the data and extract the trial times.
                string[][] sessionData = DataReader.ReadCsvData(filePath);
                List<string>[] trialTimes = Program.GetTrialTimes(index.ToString(), "Hololens", videoTimesData, trialData);

                // Create the issue header and create an issues list for the session clipping.
                string issueHeader = "Session Data Team " + index + " Hololens ";
                List<string> clippingIssues = new List<string>();
                issues.Add(issueHeader + " Clipping Issues", clippingIssues);

                // Clip the data into trials.
                List<string[]>[] trialsData = DataClipper.ExtractTrials(
                    sessionData,
                    trialTimes[0].ToArray(),
                    trialTimes[1].ToArray(),
                    ref clippingIssues,
                    issueHeader
                    );

                if (trialsData != null)
                {
                    // Write the clipped trial data output files.
                    Program.GenerateTrialsOutputs(
                        args[(int)Program.ArgsIndices.OutputFolder],
                        index.ToString(),
                        "Hololens",
                        args[(int)Program.ArgsIndices.DeviceID],
                        trialsData
                        );
                }
            }

            // Generate the issues log.
            Program.GenerateOutputLog(args[(int)Program.ArgsIndices.OutputFolder], issues);
        }

        /// <summary>
        /// Gets the trial start and stop times from the video start time and trial start and stop video time stamps.
        /// </summary>
        /// <param name="teamNumber">The team number the session is for.</param>
        /// <param name="modality">The modality the session is for.</param>
        /// <param name="videoTimesData">The video times data in a 2D string array indexed by row then column.</param>
        /// <param name="trialData">The full session viewport tracking data in a 2D string array indexed by row then column.</param>
        /// <returns>A pair of companion arrays of the start and end times, with start times array at index 0 and end times array at index 1.</returns>
        private static List<string>[] GetTrialTimes(string teamNumber, string modality, string[][] videoTimesData, string[][] trialData)
        {
            // Filter to only the trials within the given session (matching team number and modality).
            string[][] sessionTrials = trialData.Where(line => line[0] == teamNumber
                                                        && line[2] == modality).ToArray();

            // Initialize the trialTimes array.
            List<string>[] trialTimes = new List<string>[2]
            {
                new List<string>(),
                new List<string>()
            };

            // Get the video start time.
            string[] videoTimeData = videoTimesData.Where(line => line[0] == teamNumber
                                                            && line[1] == modality).FirstOrDefault();
            DateTime videoEndTime = DateTime.Parse(videoTimeData[2]);
            TimeSpan videoDuration = Program.ParseTimeStamp(videoTimeData[3]);
            DateTime videoStartTime = videoEndTime - videoDuration;

            // For each trial in the session...
            foreach (string[] line in sessionTrials)
            {
                // Parse the times.
                TimeSpan startTimeStamp = Program.ParseTimeStamp(line[7]);
                TimeSpan endTimeStamp = Program.ParseTimeStamp(line[8]);

                // Add the start and end time stamps to the video start time to get the start and end times.
                DateTime startTime = videoStartTime + startTimeStamp;
                DateTime endTime = videoStartTime + endTimeStamp;

                // Add the start and end times to the trial times array.
                trialTimes[0].Add(startTime.ToString());
                trialTimes[1].Add(endTime.ToString());
            }

            return trialTimes;
        }

        /// <summary>
        /// Parses a time stamp in the format "mm:ss" or "m:ss" or "hh:mm:ss".
        /// </summary>
        /// <param name="timeStamp">The time stamp to parse.</param>
        /// <returns></returns>
        private static TimeSpan ParseTimeStamp(string timeStamp)
        {
            string[] parts = timeStamp.Split(':');

            int minutesIndex = 0;
            int secondsIndex = 1;
            if (parts.Length == 3)
            {
                minutesIndex = 1;
                secondsIndex = 2;
            }

            return new TimeSpan(0, int.Parse(parts[minutesIndex]), int.Parse(parts[secondsIndex]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputFolderPath"></param>
        /// <param name="teamNumber"></param>
        /// <param name="modality"></param>
        /// <param name="deviceId"></param>
        /// <param name="trialsData"></param>
        private static void GenerateTrialsOutputs(string outputFolderPath, string teamNumber, string modality, string deviceId, List<string[]>[] trialsData)
        {
            // Ensure the path ends in a slash before appending the file name.
            if (outputFolderPath[outputFolderPath.Length - 1] != '\\')
            {
                outputFolderPath += "\\";
            }

            // For each trial in the dataset...
            for (int trialIndex = 0; trialIndex < trialsData.Length; trialIndex++)
            {
                // Create the file name and append it to the output folder path.
                string fileName = "Team" + teamNumber + "_Trial" + (trialIndex + 1) + "_" + modality + "_Device" + deviceId +".csv";
                string fullPath = outputFolderPath + fileName;

                // Create the file.
                using (FileStream stream = File.Create(fullPath))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        // For each line in the trial data...
                        foreach (string[] lineData in trialsData[trialIndex])
                        {
                            // Build a CSV file line, with values separated by commas.
                            StringBuilder line = new StringBuilder();
                            foreach (string value in lineData)
                            {
                                line.Append(value);
                                line.Append(",");
                            }

                            // Remove the extra trailing comma, and then write the line to the file.
                            string lineString = line.ToString();
                            lineString = lineString.Remove(lineString.Length - 1);
                            writer.WriteLine(lineString);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates an output log of the given issues at the given path.
        /// </summary>
        /// <param name="outputFilePath">The path to generate the log at.</param>
        /// <param name="allIssues">A set of issue lists for each data table.</param>
        private static void GenerateOutputLog(string outputFilePath, Dictionary<string, List<string>> allIssues)
        {
            // Create the full file path for the output log.
            // Ensure the path ends in a slash before appending the file name.
            if (outputFilePath[outputFilePath.Length - 1] != '\\')
            {
                outputFilePath += "\\";
            }
            string outputLogFullPath = outputFilePath + Program.OutputLogFileName;

            // Create the file.
            using (FileStream stream = File.Create(outputLogFullPath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // Write the header.
                    writer.WriteLine("ISSUES LOG");
                    writer.WriteLine();
                    writer.WriteLine();

                    int totalIssues = 0;
                    foreach (KeyValuePair<string, List<string>> item in allIssues)
                    {
                        // Write the header.
                        writer.WriteLine(item.Key);
                        writer.WriteLine();

                        // Write the issues.
                        if (allIssues[item.Key].Count == 0)
                        {
                            writer.WriteLine("No issues found!");
                        }
                        else
                        {
                            foreach (string issue in allIssues[item.Key])
                            {
                                writer.WriteLine(issue);
                            }
                        }

                        writer.WriteLine();
                        writer.WriteLine();

                        totalIssues += item.Value.Count;
                    }

                    writer.WriteLine("Total issues found: " + totalIssues);
                    writer.WriteLine();
                    writer.WriteLine();

                    // Write the end of the log.
                    writer.WriteLine("END OF LOG");
                }
            }
        }
    }
}
