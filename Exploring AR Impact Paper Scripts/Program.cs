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
        /// Args are expected in the following order: Gestures, Looks, Utterances, Utterances Counts, Output.
        /// </summary>
        private enum ArgsIndices
        {
            Gestures = 0,
            Looks = 1,
            Utterances = 2,
            UtterancesCount = 3,
            OutputFolder = 4
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
        /// Expects a list of file names for the input data in the following order: Gestures, Looks, Utterances, Utterances Counts, Output.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Initialize the sets of issues.
            Dictionary<string, List<string>> issues = new Dictionary<string, List<string>>()
            {
                { "Gestures", new List<string>() },
                { "Looks", new List<string>() },
                { "Utterances", new List<string>() },
                { "Utterances Count", new List<string>() }
            };

            // Validate the Gestures table.
            ValidateTableHandler validateGestures = DataValidator.ValidateGesturesData;
            Program.ValidateTable(args[(int)Program.ArgsIndices.Gestures], "Gestures", validateGestures, out List<string> gesturesIssues);
            issues["Gestures"] = gesturesIssues;

            // Validate the Looks table.
            ValidateTableHandler validateLooks = DataValidator.ValidateLooksData;
            Program.ValidateTable(args[(int)Program.ArgsIndices.Looks], "Looks", validateLooks, out List<string> looksIssues);
            issues["Looks"] = looksIssues;

            // Validate the Utterances table.
            ValidateTableHandler validateUtterances = DataValidator.ValidateUtterancesData;
            Program.ValidateTable(args[(int)Program.ArgsIndices.Utterances], "Utterances", validateUtterances, out List<string> utterancesIssues);
            issues["Utterances"] = utterancesIssues;

            // Validate the Utterances Count table.
            ValidateTableHandler validateUtterancesCount = DataValidator.ValidateUtterancesCountData;
            Program.ValidateTable(args[(int)Program.ArgsIndices.UtterancesCount], "Utterances Count", validateUtterancesCount, out List<string> utterancesCountIssues);
            issues["Utterances Count"] = utterancesCountIssues;

            // Generate the issues log.
            Program.GenerateOutputLog(args[(int)Program.ArgsIndices.OutputFolder], issues);
        }

        /// <summary>
        /// Validates the data in the table CSV.
        /// </summary>
        /// <param name="filePath">The file path to the table file.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        private static void ValidateTable(string filePath, string tableName, ValidateTableHandler validationMethod, out List<string> issues)
        {
            // Initialize the issues list.
            issues = new List<string>();

            // Read the table file data.
            string[][] data = null;
            try
            {
                data = DataReader.ReadCsvData(filePath);
            }
            catch (Exception ex)
            {
                // An error occured while trying to read the file data. Add an issue to the list.
                issues.Add(tableName + " table error - " + ex.Message);

                // Return, as no data validation is possible.
                return;
            }

            // Validate the table file data.
            validationMethod(data, out issues);
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
                    }

                    // Write the end of the log.
                    writer.WriteLine("END OF LOG");
                }
            }
        }
    }
}
