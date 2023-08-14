using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesDataScripts
{
    /// <summary>
    /// Reads the input data. See README file for expected input formats.
    /// </summary>
    internal static class DataReader
    {
        /// <summary>
        /// Reads the input data from a CSV file with the given file name.
        /// </summary>
        /// <param name="filePath">The filepath to read data from.</param>
        /// <returns>A 2D string array with the parsed data indexed by line and then column.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while attempting to read the file.</exception>
        internal static string[][] ReadCsvData(string filePath)
        {
            // Read all lines from the file.
            string[] lines;
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                // An exception was thrown while trying to read the file data.
                throw new Exception("Error reading file at given path \"" + filePath + "\".", ex);
            }

            // Parse each line and add it to the return value, indexed by line number.
            string[][] data = new string[lines.Length][];
            int lineNumber = 0;
            foreach (string line in lines)
            {
                // Split the columns (comma-separated values).
                data[lineNumber] = line.Split(',');

                // Increment the line number counter.
                lineNumber++;
            }

            // Return the parsed data.
            return data;
        }
    }
}
