using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewportDataScripts
{
    /// <summary>
    /// Clips the input data into trails. See README file for expected input and output formats.
    /// </summary>
    internal static class DataClipper
    {
        /// <summary>
        /// Extracts the viewport traking data for each trial from a full session recording.
        /// </summary>
        /// <param name="sessionData">The full session viewport tracking data in a 2D string array indexed by row then column.</param>
        /// <param name="clippingStartTimes">A list of the trial start times in the session.</param>
        /// <param name="clippingEndTimes">A list of the trial end times in the session.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        /// <returns>The trial viewport tracking data in a 3D string array, indexed by trial number then row then column.</returns>
        internal static List<string[]>[] ExtractTrials(string[][] sessionData, string[] clippingStartTimes, string[] clippingEndTimes, ref List<string> issues, string issueHeader)
        {
            // Initialize the return data structure.
            List<string[]>[] trialData = new List<string[]>[clippingStartTimes.Length];
            for (int i = 0; i < clippingStartTimes.Length; i++)
            {
                trialData[i] = new List<string[]>();
            }

            // Initialize the line pointer to the start of the file.
            int lineIndex = 0;

            // For each pair of start/stop times in the clipping times array...
            for (int clipIndex = 0; clipIndex < clippingStartTimes.Length; clipIndex++)
            {
                // Parse the trial start time.
                DateTime startTime;
                if (DateTime.TryParse(clippingStartTimes[clipIndex], out startTime) == false)
                {
                    issues.Add(issueHeader + "Clipping Time Parse Error - Could not parse clipping start time. Read: \"" + clippingStartTimes[clipIndex] + "\", expected a valid datetime.");
                    return null;
                }

                // Parse the trial end time.
                DateTime endTime;
                if (DateTime.TryParse(clippingEndTimes[clipIndex], out endTime) == false)
                {
                    issues.Add(issueHeader + "Clipping Time Parse Error - Could not parse clipping end time. Read: \"" + clippingEndTimes[clipIndex] + "\", expected a valid datetime.");
                    return null;
                }

                // Ensure the line number is within the session data's length.
                if (lineIndex >= sessionData.Length)
                {
                    issues.Add(issueHeader + "Session Data Format Error - Session data is empty!");
                    return null;
                }

                // Parse the line's time stamp.
                DateTime lineTimeStamp = DateTime.MinValue;
                if (DateTime.TryParse(sessionData[lineIndex][0], out lineTimeStamp) == false)
                {
                    // An error occured while attempting to parse the time stamp. Add an entry to the error log and continue to the next line.
                    issues.Add(issueHeader + "Data Time Parse Error - Could not parse time stamp of line " + lineIndex + ". Read: \"" + sessionData[lineIndex][0] + "\", expected a valid datetime.");
                }

                // Advance the line pointer until the first time stamp that is at or passed the trial start time.
                while (lineTimeStamp < startTime)
                {
                    // Move on to the next line.
                    lineIndex++;

                    // Ensure the line number is within the session data's length.
                    if (lineIndex >= sessionData.Length)
                    {
                        issues.Add(issueHeader + "Session Data Format Error - The end of the session data has been reached, but no time has been found passed the start time for trial " + (clipIndex + 1) + "! Start time read: " + startTime.ToString());
                        return trialData;
                    }

                    // Parse the line's time stamp.
                    if (DateTime.TryParse(sessionData[lineIndex][0], out lineTimeStamp) == false)
                    {
                        // An error occured while attempting to parse the time stamp. Add an entry to the error log and continue to the next line.
                        issues.Add(issueHeader + "Data Time Parse Error - Could not parse time stamp of line " + lineIndex + ". Read: \"" + sessionData[lineIndex][0] + "\", expected a valid datetime. Looking for starting line for trial " + clipIndex + ".");
                    }

                }
                // lineIndex is now at the first line equal to or passed the trial start tine.
                issues.Add(issueHeader + " - Started clipping for trial " + (clipIndex + 1) + " at input line " + (lineIndex + 1) + ".");

                // Copy over the data until the first time stamp passed the trial end time.
                while (lineTimeStamp <= endTime)
                {
                    // Copy over the current line.
                    trialData[clipIndex].Add(sessionData[lineIndex]);

                    // Move on to the next line.
                    lineIndex++;

                    // Ensure the line number is within the session data's length.
                    if (lineIndex >= sessionData.Length)
                    {
                        issues.Add(issueHeader + "Session Data Format Error - The end of the session data (line " + lineIndex + ") has been reached, but no time has been found passed the end time for trial " + (clipIndex + 1) + "! End time read: " + startTime.ToString());
                        return trialData;
                    }

                    // Parse the line's time stamp.
                    if (DateTime.TryParse(sessionData[lineIndex][0], out lineTimeStamp) == false)
                    {
                        // An error occured while attempting to parse the time stamp. Add an entry to the error log and continue to the next line.
                        issues.Add(issueHeader + "Data Time Parse Error - Could not parse time stamp of line " + lineIndex + ". Read: \"" + sessionData[lineIndex][0] + "\", expected a valid datetime. Copying data line for trial " + clipIndex + ".");
                    }
                }
                // lineIndex is now at the first line passed the trial end time.
                issues.Add(issueHeader + " - Finished clipping for trial " + (clipIndex + 1) + " at input line " + lineIndex + ".");

                // Move on to the next trial.
            }

            return trialData;
        }
    }
}
