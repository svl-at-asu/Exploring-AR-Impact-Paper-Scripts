using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesDataScripts
{
    internal class Program
    {
        /// <summary>
        /// Args are expected in the following order: Number of teams, Trials per team, Columns to smooth, Smoothing weight, Sample rate, Dataset rate, Input data folder, Output folder.
        /// </summary>
        private enum ArgsIndices
        {
            NumberOfTeams = 0,
            TrialsPerTeam = 1,
            ColumnsToSmooth = 2,
            SmoothingWeight = 3,
            SampleRate = 4,
            DatasetRate = 5,
            InputDataFolder = 6,
            OutputFolder = 7
        }

        /// <summary>
        /// The file name for the output validation log.
        /// </summary>
        private static readonly string OutputLogFileName = "ValidationLog.txt";

        static void Main(string[] args)
        {
            // Parse the number of teams.
            int numTeams = int.Parse(args[(int)Program.ArgsIndices.NumberOfTeams]);

            // Parse the number of trials.
            int numTrials = int.Parse(args[(int)Program.ArgsIndices.TrialsPerTeam]);

            // Assemble the file path.
            string filePath = args[(int)Program.ArgsIndices.InputDataFolder];

            // Ensure the path ends in a slash before appending the file name.
            if (filePath[filePath.Length - 1] != '\\')
            {
                filePath += "\\";
            }

            // Initialize the issues set.
            Dictionary<string, List<string>> issues = new Dictionary<string, List<string>>()
            {
                { "Program Args", new List<string>() },
                { "Input", new List<string>() }
            };

            // Parse the columns to smooth.
            string[] columnToSmoothStrings = args[(int)Program.ArgsIndices.ColumnsToSmooth].Split(',');
            List<int> columnsToSmooth = new List<int>();
            foreach (string columnString in columnToSmoothStrings)
            {
                columnsToSmooth.Add(int.Parse(columnString));
            }

            // Parse the smoothing weight.
            double smoothingWeight;
            if (double.TryParse(args[(int)Program.ArgsIndices.SmoothingWeight], out smoothingWeight) == false)
            {
                issues["Program Args"].Add("Error trying to parse program argument " + (int)Program.ArgsIndices.SmoothingWeight + ". Read: \"" + args[(int)Program.ArgsIndices.SmoothingWeight] + "\", expected a valid double.");
            }

            // Parse the sample and dataset rates.
            bool skipSampling = false;
            if (int.TryParse(args[(int)Program.ArgsIndices.SampleRate], out int sampleRate) == false)
            {
                issues["Program Args"].Add("Error trying to parse program argument " + (int)Program.ArgsIndices.SampleRate + ". Read: \"" + args[(int)Program.ArgsIndices.SampleRate] + "\", expected a valid integer.");
                skipSampling = true;
            }

            if (int.TryParse(args[(int)Program.ArgsIndices.DatasetRate], out int datasetRate) == false)
            {
                issues["Program Args"].Add("Error trying to parse program argument " + (int)Program.ArgsIndices.DatasetRate + ". Read: \"" + args[(int)Program.ArgsIndices.DatasetRate] + "\", expected a valid integer.");
                skipSampling = true;
            }

            // For each team...
            for (int teamNum = 1; teamNum <= numTeams; teamNum++)
            {
                // For each trial...
                for (int trialNum = 1; trialNum <= numTrials; trialNum++)
                {
                    // Intialize the two data structures to hold the device data.
                    string[][][] participantData = new string[2][][];

                    // For each device...
                    for (int deviceNum = 0; deviceNum < 2; deviceNum++)
                    {
                        // Assemble the file name to read.
                        string fileName = "Team" + teamNum + "_Trial" + trialNum + "_Hololens_Device" + deviceNum + ".csv";
                        string trialFilePath = filePath + fileName;

                        // Read the trial table data file.
                        string[][] trialData = null;
                        try
                        {
                            trialData = DataReader.ReadCsvData(trialFilePath);
                        }
                        catch (Exception ex)
                        {
                            // An error occured while trying to read the file data. Add an issue to the list.
                            issues["Input"].Add("Error reading input file \"" + filePath + "\": " + ex.Message);
                            continue;
                        }

                        // Initialize the smoothing issues set and issue header.
                        List<string> smoothingIssues = new List<string>();
                        string issueHeader = "Smoothing Error Team " + teamNum + ", Trial " + trialNum + ", Device " + deviceNum + " - ";

                        // Smooth the data and add the issue set to the issues.
                        participantData[deviceNum] = DataTransformer.SmoothData(trialData, columnsToSmooth.ToArray(), smoothingWeight, ref smoothingIssues, issueHeader);
                        issues.Add("Input " + fileName, smoothingIssues);

                        // Output the smoothed data.
                        Program.GenerateTrialsOutputs(
                            args[(int)Program.ArgsIndices.OutputFolder],
                            teamNum.ToString(),
                            trialNum.ToString(),
                            "Hololens",
                            deviceNum.ToString(),
                            participantData[deviceNum]
                            );
                    }

                    // Skip angle calculations if either paticipants' data is missing.
                    string angleIssueKey = "Participant Angles Team " + teamNum + " Trial " + trialNum;
                    if (participantData[0] == null || participantData[1] == null)
                    {
                        issues.Add(angleIssueKey, new List<string>() { "Participant data missing, angle calculations skipped." });
                        continue;
                    }

                    List<string> angleIssues = new List<string>();
                    string trialIssueHeader = "Participant Angle Calculation Error Team " + teamNum + ", Trial " + trialNum + " - ";

                    // Calculate the participant angles for the trial.
                    string[][] participantAnglesData = DataTransformer.CalculateAngles(
                        participantData[0],
                        participantData[1],
                        1,
                        3,
                        ref angleIssues,
                        trialIssueHeader
                        );
                    issues.Add(angleIssueKey, angleIssues);

                    // Sample the calculated angles, or just pass along the existing data if sampling is to be skipped.
                    string[][] sampledAnglesData = skipSampling ? participantAnglesData : DataTransformer.SampleAtRate(
                        participantAnglesData,
                        datasetRate,
                        sampleRate
                        );

                    // Output the participant angles data.
                    Program.GenerateAnglesOutputs(
                        args[(int)Program.ArgsIndices.OutputFolder],
                        teamNum.ToString(),
                        trialNum.ToString(),
                        sampledAnglesData);
                }
            }

            // Generate the issues log.
            Program.GenerateOutputLog(args[(int)Program.ArgsIndices.OutputFolder], issues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputFolderPath"></param>
        /// <param name="teamNumber"></param>
        /// <param name="modality"></param>
        /// <param name="deviceId"></param>
        /// <param name="trialsData"></param>
        private static void GenerateTrialsOutputs(string outputFolderPath, string teamNumber, string trialNumber, string modality, string deviceId, string[][] trialsData)
        {
            // Ensure the path ends in a slash before appending the file name.
            if (outputFolderPath[outputFolderPath.Length - 1] != '\\')
            {
                outputFolderPath += "\\";
            }

            // Create the file name and append it to the output folder path.
            string fileName = "Team" + teamNumber + "_Trial" + trialNumber + "_" + modality + "_Device" + deviceId + "_Smoothed.csv";
            string fullPath = outputFolderPath + fileName;

            // Create the file.
            using (FileStream stream = File.Create(fullPath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // For each line in the trial data...
                    foreach (string[] lineData in trialsData)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputFolderPath"></param>
        /// <param name="teamNumber"></param>
        /// <param name="modality"></param>
        /// <param name="anglesData"></param>
        private static void GenerateAnglesOutputs(string outputFolderPath, string teamNumber, string trialNumber, string[][] anglesData)
        {
            // Ensure the path ends in a slash before appending the file name.
            if (outputFolderPath[outputFolderPath.Length - 1] != '\\')
            {
                outputFolderPath += "\\";
            }

            // Create the file name and append it to the output folder path.
            string fileName = "Angles_Team" + teamNumber + "_Trial" + trialNumber +".csv";
            string fullPath = outputFolderPath + fileName;

            // Create the file.
            using (FileStream stream = File.Create(fullPath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // Write the header line.
                    writer.WriteLine("time,p1_x,p1_y,p2_x,p2_y,p1_angle,p2_angle,distance");

                    // For each line in the trial data...
                    foreach (string[] lineData in anglesData)
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
