using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidationScripts
{
    internal class Program
    {
        /// <summary>
        /// Args are expected in the following order: Gestures, Looks, Utterances, Utterances Counts, Trial Data, Output.
        /// </summary>
        private enum ArgsIndices
        {
            Gestures = 0,
            Looks = 1,
            Utterances = 2,
            UtterancesCount = 3,
            TrialData = 4,
            OutputFolder = 5
        }

        /// <summary>
        /// The file name for the output validation log.
        /// </summary>
        private static readonly string OutputLogFileName = "ValidationLog.txt";

        /// <summary>
        /// The file name for the output transformed table.
        /// </summary>
        private static readonly string OutputTransformedTableFileName = "FullEventTable.csv";

        /// <summary>
        /// The delegate handler for the table validation function.
        /// </summary>
        /// <param name="data">The gesture data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns></returns>
        private delegate bool ValidateTableHandler(string[][] data, out List<string> issues);

        /// <summary>
        /// The names of each table this script processes.
        /// </summary>
        private static readonly string[] TableNames = { "Gestures", "Looks", "Utterances", "Utterances Count" };

        /// <summary>
        /// Expects a list of file names for the input data in the following order: Gestures, Looks, Utterances, Utterances Counts, Trial Data, Output.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Initialize the table data and issue sets.
            Dictionary<string, string[][]> tableData = new Dictionary<string, string[][]>();
            Dictionary<string, List<string>> issues = new Dictionary<string, List<string>>();

            Dictionary<string, ValidateTableHandler> validationHandlers = new Dictionary<string, ValidateTableHandler>()
            {
                { "Gestures", DataValidator.ValidateGesturesData },
                { "Looks", DataValidator.ValidateLooksData },
                { "Utterances", DataValidator.ValidateUtterancesData },
                { "Utterances Count", DataValidator.ValidateUtterancesCountData }
            };

            // Read and validate each table.
            foreach (string tableName in Program.TableNames)
            {
                // Read the table file data.
                try
                {
                    // Get the file path from the program args ising the ArgsIndices enum.
                    Enum.TryParse(tableName.Replace(" ", ""), out Program.ArgsIndices argsIndex);
                    tableData[tableName] = DataReader.ReadCsvData(args[(int)argsIndex]);
                }
                catch (Exception ex)
                {
                    // An error occured while trying to read the file data. Add an issue to the list.
                    issues.Add(tableName, new List<string>()
                    {
                        tableName + " table error - " + ex.Message
                    });

                    // Move on to the next table, as no data validation is possible.
                    continue;
                }

                // Validate the table.
                validationHandlers[tableName](tableData[tableName], out List<string> tableIssues);

                issues.Add(tableName, tableIssues);
            }

            // Read the trial table data file.
            string[][] trialData = null;
            try
            {
                trialData = DataReader.ReadCsvData(args[(int)Program.ArgsIndices.TrialData]);
            }
            catch(Exception ex)
            {
                // An error occured while trying to read the file data. Add an issue to the list.
                issues.Add("Trial Data", new List<string>()
                {
                    "Trial Data table error - " + ex.Message
                });
            }

            // Transform the data into the output event table, if it was successfully read.
            if (trialData != null)
            {
                List<string[]> eventTableData = DataTransformer.TransformData(tableData, trialData, out List<string> transformIssues);
                issues.Add("Trial Data", transformIssues);

                // Output the transformed data table.
                Program.GenerateEventTable(args[(int)Program.ArgsIndices.OutputFolder], eventTableData);
            }

            // Generate the issues log.
            Program.GenerateOutputLog(args[(int)Program.ArgsIndices.OutputFolder], issues);
        }

        /// <summary>
        /// Generates an event table as a CSV file at the given path.
        /// </summary>
        /// <param name="outputFilePath">The path to generate the table at.</param>
        /// <param name="tableData">The table data to generate the table file from.</param>
        private static void GenerateEventTable(string outputFilePath, List<string[]> tableData)
        {
            // Create the full file path for the output table.
            // Ensure the path ends in a slash before appending the file name.
            if (outputFilePath[outputFilePath.Length - 1] != '\\')
            {
                outputFilePath += "\\";
            }
            string outputTableFullPath = outputFilePath + Program.OutputTransformedTableFileName;

            // Create the file.
            using (FileStream stream = File.Create(outputTableFullPath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // Write the table header.
                    writer.WriteLine("Team, Trial, Modality, Trial Time, Event Type, Participant, Action, Action Target, Action Intent, Utterance Purpose, Deictic Pronouns, Spatial Deictic");

                    // Write the table lines.
                    foreach (string[] lineData in tableData)
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
