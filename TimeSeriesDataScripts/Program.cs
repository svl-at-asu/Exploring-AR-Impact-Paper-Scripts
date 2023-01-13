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
        /// Args are expected in the following order: Number of teams, Trials per team, Input data folder, Output folder.
        /// </summary>
        private enum ArgsIndices
        {
            NumberOfTeams = 0,
            TrialsPerTeam = 1,
            InputDataFolder = 2,
            OutputFolder = 3
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
            Dictionary<string, List<string>> issues = new Dictionary<string, List<string>>();
            issues.Add("Input", new List<string>());

            // For each team...
            for (int teamNum = 1; teamNum <= numTeams; teamNum++)
            {
                // For each trial...
                for (int trialNum = 1; trialNum <= numTrials; trialNum++)
                {
                    // For each device...
                    for (int deviceNum = 0; deviceNum < 2; deviceNum++)
                    {
                        // Assemble the file name to read.
                        string fileName = "Team" + teamNum + "_Trial" + trialNum + "_Hololens_Device" + deviceNum + ".csv";
                        filePath += fileName;

                        // Read the trial table data file.
                        string[][] trialData = null;
                        try
                        {
                            trialData = DataReader.ReadCsvData(filePath);
                        }
                        catch (Exception ex)
                        {
                            // An error occured while trying to read the file data. Add an issue to the list.
                            issues["Input"].Add("Error reading input file \"" + filePath + "\": " + ex.Message);
                        }
                    }
                }
            }

            // Generate the issues log.
            Program.GenerateOutputLog(args[(int)Program.ArgsIndices.OutputFolder], issues);
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
