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

        /// <summary>
        /// Calculates the angles with participant 1 and 2 respectively as vertices formed by the triangle defined by each participant and the chart.
        /// </summary>
        /// <param name="participantData1">The location data for participant 1 at each time step.</param>
        /// <param name="participantData2">The location data for participant 2 at each time step.</param>
        /// <param name="xCol">The column in each participant data that contains the x value to use.</param>
        /// <param name="yCol">The column in each participant data that contains the y value to use.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        /// <returns>A 2D string array indexed by row and then column, containing the time stamp, participant 1 x, participant 1 y, participant 2 x, participant 2 y, participant 1 angle, and participant 2 angle in that order for each time step.</returns>
        internal static string[][] CalculateAngles(string[][] participantData1, string[][] participantData2, int xCol, int yCol, ref List<string> issues, string issueHeader)
        {
            // Get the shorter length between the two data arrays and initialize the results to that length.
            int shorterLength = Math.Min(participantData1.Length, participantData2.Length);
            string[][] result = new string[shorterLength][];

            // For each line in the shorter data set...
            for (int lineNum = 0; lineNum < shorterLength; lineNum++)
            {
                // Parse the coordinates of participant 1.
                double participant1_x;
                if (double.TryParse(participantData1[lineNum][xCol], out participant1_x) == false)
                {
                    issues.Add(issueHeader + "Error parsing x value for participant 1 on line " + (lineNum + 1) + " in column " + xCol);
                    continue;
                }
                double participant1_y;
                if (double.TryParse(participantData1[lineNum][yCol], out participant1_y) == false)
                {
                    issues.Add(issueHeader + "Error parsing y value for participant 1 on line " + (lineNum + 1) + " in column " + yCol);
                    continue;
                }

                // Parse the coordinates of participant 2.
                double participant2_x;
                if (double.TryParse(participantData2[lineNum][xCol], out participant2_x) == false)
                {
                    issues.Add(issueHeader + "Error parsing x value for participant 2 on line " + (lineNum + 1) + " in column " + xCol);
                    continue;
                }
                double participant2_y;
                if (double.TryParse(participantData2[lineNum][yCol], out participant2_y) == false)
                {
                    issues.Add(issueHeader + "Error parsing y value for participant 2 on line " + (lineNum + 1) + " in column " + yCol);
                    continue;
                }

                // The chart is centered at the origin.
                double chart_x = 0;
                double chart_y = 0;

                // Find the length of each side of the triangle formed by the two participants and the chart.
                double length_p1_chart = Math.Sqrt(Math.Pow(participant1_x - chart_x, 2) + Math.Pow(participant1_y - chart_y, 2));
                double length_p2_chart = Math.Sqrt(Math.Pow(participant2_x - chart_x, 2) + Math.Pow(participant2_y - chart_y, 2));
                double length_p1_p2 = Math.Sqrt(Math.Pow(participant1_x - participant2_x, 2) + Math.Pow(participant1_y - participant2_y, 2));

                // Use the law of cosines to calculate the angle with participant 1 and participant 2 as the vertex.
                double angle_p1_radians = Math.Acos((Math.Pow(length_p1_p2, 2) + Math.Pow(length_p1_chart, 2) - Math.Pow(length_p2_chart, 2)) / (2 * length_p1_p2 * length_p1_chart));
                double angle_p2_radians = Math.Acos((Math.Pow(length_p1_p2, 2) + Math.Pow(length_p2_chart, 2) - Math.Pow(length_p1_chart, 2)) / (2 * length_p1_p2 * length_p2_chart));

                // Convert the angles to degrees.
                double angle_p1 = angle_p1_radians * 180 / Math.PI;
                double angle_p2 = angle_p2_radians * 180 / Math.PI;

                // Write the output values to the line, preserving the time stamp.
                result[lineNum] = new string[7]
                {
                    participantData1[lineNum][0],
                    participant1_x.ToString(),
                    participant1_y.ToString(),
                    participant2_x.ToString(),
                    participant2_y.ToString(),
                    angle_p1.ToString(),
                    angle_p2.ToString()
                };
            }

            return result;
        }
    }
}
