using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesDataScripts
{
    internal class DataTransformer
    {
        /// <summary>
        /// Smoothes the data in each of the given columns. Each column to smooth is treated as an independent time series, with each line being a time sample.
        /// </summary>
        /// <param name="data">The data containing columns to be smoothed in a 2D string array indexed by row then column.</param>
        /// <param name="columns">The list of column numbers to apply smoothing to (zero-based). Each column is independetly smoothed as its own time series.</param>
        /// <param name="alpha">The weight for the exponential smoothing.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        /// <returns>The smoothed data in a 2D string array, indexed by row then column.</returns>
        internal static string[][] SmoothData(string[][] data, int[] columns, double alpha, ref List<string> issues, string issueHeader)
        {
            // Initialize the return data structure.
            string[][] result = new string[data.Length][];

            // For each line in the data...
            for (int lineNum = 0; lineNum < data.Length; lineNum++)
            {
                // Initialize the row for the result.
                result[lineNum] = new string[data[lineNum].Length];

                // For each column in the line...
                for (int columnNum = 0; columnNum < data[lineNum].Length; columnNum++)
                {
                    // If the column is not one to be smoothed, simply copy it as-is.
                    if (columns.Contains(columnNum) == false)
                    {
                        result[lineNum][columnNum] = data[lineNum][columnNum];
                    }
                    else
                    {
                        // Parse the value.
                        double value;
                        if (double.TryParse(data[lineNum][columnNum], out value) == false)
                        {
                            issues.Add(issueHeader + "Error trying to parse line " + lineNum + ", column " + columnNum + ". Read: \"" + data[lineNum][columnNum] + "\", expected a valid double.");
                        }

                        // The first value is itself.
                        if (lineNum == 0)
                        {
                            result[lineNum][columnNum] = value.ToString();
                        }
                        else
                        {
                            // Parse the previous line's value.
                            double previous;
                            if (double.TryParse(result[lineNum - 1][columnNum], out previous) == false)
                            {
                                issues.Add(issueHeader + "Error trying to parse previous value at line " + lineNum + ", column " + columnNum + ". Read: \"" + data[lineNum][columnNum] + "\", expected a valid double.");
                            }

                            // Calculate the exponential smoothing. The function is technically recursive, but the
                            //      previous line's calculated value is the solution to t-1.
                            double newValue = alpha * value + (1 - alpha) * previous;

                            result[lineNum][columnNum] = newValue.ToString();
                        }
                    }
                }
            }

            return result;
        }
    }
}
