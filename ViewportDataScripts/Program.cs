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
        /// Args are expected in the following order: Trial Data, Output.
        /// </summary>
        private enum ArgsIndices
        {
            NumberOfTeams = 0,
            InputDataFolder = 1,
            TrialData = 2,
            OutputFolder = 3
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

                // Read the data.
                string[][] fileData = DataReader.ReadCsvData(filePath);

                // Clip the data into trials.
                string[][][] trialsData = DataClipper.ExtractTrials(fileData);
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
