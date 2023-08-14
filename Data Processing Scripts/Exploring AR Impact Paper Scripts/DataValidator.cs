using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataValidationScripts
{
    /// <summary>
    /// Validates the input data. See README file for expected input formats.
    /// </summary>
    internal static class DataValidator
    {
        #region Validation Data
        /// <summary>
        /// The number of columns in the Gestures table.
        /// </summary>
        private static readonly int GesturesTableColumCount = 7;

        /// <summary>
        /// The number of columns in the Looks table.
        /// </summary>
        private static readonly int LooksTableColumCount = 4;

        /// <summary>
        /// The number of columns in the Utterances table.
        /// </summary>
        private static readonly int UtterancesTableColumCount = 7;

        /// <summary>
        /// The number of columns in the Utterances Count table.
        /// </summary>
        private static readonly int UtterancesCountTableColumCount = 8;

        /// <summary>
        /// The number of teams.
        /// </summary>
        private static readonly int NumberOfTeams = 10;

        /// <summary>
        /// The list of accepted video names.
        /// </summary>
        private static readonly string[] VideoNames =
        {
            "Desktop",
            "Desktop1",
            "Desktop2",
            "Hololens"
        };

        /// <summary>
        /// The number of trials in each modality for one team.
        /// </summary>
        private static readonly int NumberOfTrials = 12;

        /// <summary>
        /// The format regex for time stamps.
        /// </summary>
        private static readonly string TimeStampFormat = @"[0-9]?[0-9]:[0-5][0-9]";

        /// <summary>
        /// The list of accepted gesture target values.
        /// </summary>
        private static readonly string[] GestureTargetValues =
        {
            "s",
            "o",
            "c",
            "b"
        };

        /// <summary>
        /// The list of accepted gesture intent values.
        /// </summary>
        private static readonly string[] GestureIntentValues =
        {
            "r",
            "d",
            "v",
            "c"
        };

        private static readonly string[] UtterancePurposeValues =
        {
            "r",
            "p",
            "a",
            "v"
        };
        #endregion

        #region Validation Methods
        /// <summary>
        /// Validates the gesture data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The gesture data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        internal static bool ValidateGesturesData(string[][] data, out List<string> issues)
        {
            // Initialize the issues list.
            issues = new List<string>();

            // For each line in the data (skipping first header line)...
            for (int lineNumber = 1; lineNumber < data.Length; lineNumber++)
            {
                // Get a reference to the current line.
                string[] line = data[lineNumber];

                // Set the issue header.
                string issueHeader = "Gestures line " + lineNumber + " - ";

                // Ensure the line has the correct number of columns.
                if (DataValidator.ValidateLineLength(line, DataValidator.GesturesTableColumCount, ref issues, issueHeader) == false)
                {
                    // Move onto the next line, as no additional validation is possible for the current line.
                    continue;
                }

                // Ensure the team number is a valid value.
                DataValidator.ValidateTeamNumber(line[0], ref issues, issueHeader);

                // Ensure the video name is one of the accepted values.
                DataValidator.ValidateVideoName(line[1], ref issues, issueHeader);

                // Ensure the time stamp is formatted correctly.
                DataValidator.ValidateTimeStamp(line[2], ref issues, issueHeader);

                // Ensure there is some description present of the gesture.
                DataValidator.ValidateGestureDescription(line[3], ref issues, issueHeader);

                // Ensure the gesturer is one of the accepted values.
                DataValidator.ValidateIntegerInRange(line[4], 1, 2, "Gesturer", ref issues, issueHeader);

                // Ensure the target is one of the accepted values.
                DataValidator.ValidateEnum(line[5], DataValidator.GestureTargetValues, "Target", ref issues, issueHeader);

                // Ensure the intent is one of the accepted values.
                DataValidator.ValidateEnum(line[6], DataValidator.GestureIntentValues, "Intent", ref issues, issueHeader);
            }

            // Return whether there are any issues.
            return issues.Count == 0;
        }

        /// <summary>
        /// Validates the look data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The look data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        internal static bool ValidateLooksData(string[][] data, out List<string> issues)
        {
            // Initialize the issues list.
            issues = new List<string>();

            // For each line in the data (skipping first header line)...
            for (int lineNumber = 1; lineNumber < data.Length; lineNumber++)
            {
                // Get a reference to the current line.
                string[] line = data[lineNumber];

                // Set the issue header.
                string issueHeader = "Looks line " + lineNumber + " - ";

                // Ensure the line has the correct number of columns.
                if (DataValidator.ValidateLineLength(line, DataValidator.LooksTableColumCount, ref issues, issueHeader) == false)
                {
                    // Move onto the next line, as no additional validation is possible for the current line.
                    continue;
                }

                // Ensure the team number is a valid value.
                DataValidator.ValidateTeamNumber(line[0], ref issues, issueHeader);

                // Ensure the video name is one of the accepted values.
                DataValidator.ValidateVideoName(line[1], ref issues, issueHeader);

                // Ensure the time stamp is formatted correctly.
                DataValidator.ValidateTimeStamp(line[2], ref issues, issueHeader);

                // Ensure the initiator is one of the accepted values.
                DataValidator.ValidateIntegerInRange(line[3], 1, 2, "Initiator", ref issues, issueHeader);
            }

            // Return whether there are any issues.
            return issues.Count == 0;
        }

        /// <summary>
        /// Validates the utterance data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The utterance data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        internal static bool ValidateUtterancesData(string[][] data, out List<string> issues)
        {
            // Initialize the issues list.
            issues = new List<string>();

            // For each line in the data (skipping first header line)...
            for (int lineNumber = 1; lineNumber < data.Length; lineNumber++)
            {
                // Get a reference to the current line.
                string[] line = data[lineNumber];

                // Set the issue header.
                string issueHeader = "Utterances line " + lineNumber + " - ";

                // Ensure the line has the correct number of columns.
                if (DataValidator.ValidateLineLength(line, DataValidator.UtterancesTableColumCount, ref issues, issueHeader) == false)
                {
                    // Move onto the next line, as no additional validation is possible for the current line.
                    continue;
                }

                // Ensure the team number is a valid value.
                DataValidator.ValidateTeamNumber(line[0], ref issues, issueHeader);

                // Ensure the video name is one of the accepted values.
                DataValidator.ValidateVideoName(line[1], ref issues, issueHeader);

                // Ensure the time stamp is formatted correctly.
                DataValidator.ValidateTimeStamp(line[2], ref issues, issueHeader);

                // Ensure the purpose is one of the accepted values.
                DataValidator.ValidateEnum(line[3], DataValidator.UtterancePurposeValues, "Purpose", ref issues, issueHeader);

                // Ensure the speaker is one of the accepted values.
                DataValidator.ValidateIntegerInRange(line[4], 1, 2, "Speaker", ref issues, issueHeader);

                // Ensure the deictic pronouns is one of the accepted values.
                DataValidator.ValidateInteger(line[5], "Deictic Pronouns", ref issues, issueHeader);

                // Ensure the spatial deictic is one of the accepted values.
                DataValidator.ValidateInteger(line[5], "Spatial Deictic", ref issues, issueHeader);
            }

            // Return whether there are any issues.
            return issues.Count == 0;
        }

        /// <summary>
        /// Validates the utterances count data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The utterances count data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        internal static bool ValidateUtterancesCountData(string[][] data, out List<string> issues)
        {
            // Initialize the issues list.
            issues = new List<string>();

            // For each line in the data (skipping first header line)...
            for (int lineNumber = 1; lineNumber < data.Length; lineNumber++)
            {
                // Get a reference to the current line.
                string[] line = data[lineNumber];

                // Set the issue header.
                string issueHeader = "Utterances Count line " + lineNumber + " - ";

                // Ensure the line has the correct number of columns.
                if (DataValidator.ValidateLineLength(line, DataValidator.UtterancesCountTableColumCount, ref issues, issueHeader) == false)
                {
                    // Move onto the next line, as no additional validation is possible for the current line.
                    continue;
                }

                // Ensure the team number is a valid value.
                DataValidator.ValidateTeamNumber(line[0], ref issues, issueHeader);

                // Ensure the video name is one of the accepted values.
                DataValidator.ValidateVideoName(line[1], ref issues, issueHeader);

                // Ensure the trial number is one of the accepted values.
                DataValidator.ValidateIntegerInRange(line[2], 1, DataValidator.NumberOfTrials, "Trial number", ref issues, issueHeader);

                // Ensure the p1 speaking to self is one of the accepted values.
                DataValidator.ValidateInteger(line[3], "P1 speaking to self", ref issues, issueHeader);

                // Ensure the p2 speaking to self is one of the accepted values.
                DataValidator.ValidateInteger(line[4], "P2 speaking to self", ref issues, issueHeader);

                // Ensure the p1 speaking to p2 is one of the accepted values.
                DataValidator.ValidateInteger(line[5], "P1 speaking to P2", ref issues, issueHeader);

                // Ensure the p2 speaking to p1 is one of the accepted values.
                DataValidator.ValidateInteger(line[6], "P2 speaking to P1", ref issues, issueHeader);

                // Ensure the either speaking to instructor is one of the accepted values.
                DataValidator.ValidateInteger(line[7], "Either speaking to instructor", ref issues, issueHeader);
            }

            // Return whether there are any issues.
            return issues.Count == 0;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Ensures the line has the correct number of columns.
        /// </summary>
        /// <param name="line">The line to validate.</param>
        /// <param name="numColumns">The number of columns the line should have.</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static bool ValidateLineLength(string[] line,int numColumns, ref List<string> issues, string issueHeader)
        {
            // Ensure the line has the expected number of columns.
            if (line.Count() != numColumns)
            {
                // The line does not have the expected number of columns. Add an issue to the list.
                issues.Add(issueHeader + "Line does not have the expected number of values. Expected " + numColumns + " values.");
                return false;
            }

            // If no issues were found, the issues list is unchanged.
            return true;
        }

        /// <summary>
        /// Ensures the team number is a valid integet between 1 and the Number of Teams.
        /// </summary>
        /// <param name="teamNumberData">The team number data to validate.</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static void ValidateTeamNumber(string teamNumberData, ref List<string> issues, string issueHeader)
        {
            DataValidator.ValidateIntegerInRange(teamNumberData, 1, DataValidator.NumberOfTeams, "Team number", ref issues, issueHeader);
        }

        /// <summary>
        /// Ensures the video name is one of the accepted values.
        /// </summary>
        /// <param name="videoNameData">The video name to validate.</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static void ValidateVideoName(string videoNameData, ref List<string> issues, string issueHeader)
        {
            // Ensure the video name is not empty.
            if (string.IsNullOrEmpty(videoNameData))
            {
                // The video name is empty. Add an issue to the list.
                issues.Add(issueHeader + "Video name is empty. Expected one of the following values: " + DataValidator.VideoNames.ToString() + ".");

                // Return, as no additional validation is possible.
                return;
            }

            // Ensure the video name is one of the accepted values.
            if (DataValidator.VideoNames.Contains(videoNameData) == false)
            {
                // The video name is not one of the accepted values. Add an issue to the list.
                issues.Add(issueHeader + "Video name is invalid. Read: \"" + videoNameData + "\", xpected one of the following values: " + DataValidator.VideoNames.ToString() + ".");
            }

            // If no issues were found, the issues list is unchanged.
        }

        /// <summary>
        /// Ensures the video time stamp is in the correct format.
        /// </summary>
        /// <param name="timeStampData">The time stamp to validate.</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static void ValidateTimeStamp(string timeStampData, ref List<string> issues, string issueHeader)
        {
            // Ensure the time stamp is not empty.
            if (string.IsNullOrEmpty(timeStampData))
            {
                // The time stamp is empty. Add an issue to the list.
                issues.Add(issueHeader + "Time stamp is empty. Expected a value in the format: \"mm:ss\" or \"m:ss\".");

                // Return, as no additional validation is possible.
                return;
            }

            // Ensure the time stamp is in a valid format.
            Match result = Regex.Match(timeStampData, DataValidator.TimeStampFormat);
            if (result == Match.Empty)
            {
                // The time stamp is an invalid format. Add an issue to the list.
                issues.Add(issueHeader + "Time stamp is an invalid format. Read: \"" + timeStampData + "\", expected a value in the format: \"mm:ss\" or \"m:ss\".");
            }

            // If no issues were found, the issues list is unchanged.
        }

        /// <summary>
        /// Ensures the gesture description is not empty.
        /// </summary>
        /// <param name="descriptionData">The gesture description to validate.</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static void ValidateGestureDescription(string descriptionData, ref List<string> issues, string issueHeader)
        {
            // Ensure the description is not empty.
            if (string.IsNullOrEmpty(descriptionData))
            {
                // The description is empty. Add an issue to the list.
                issues.Add(issueHeader + "Gesture description is empty. Expected a non-empty description.");
            }

            // If no issues were found, the issues list is unchanged.
        }

        /// <summary>
        /// Ensures the given value is a valid integer.
        /// </summary>
        /// <param name="dataString">The value to validate.</param>
        /// <param name="valueName">The name of the value being validated (used in issue messages).</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static void ValidateInteger(string dataString, string valueName, ref List<string> issues, string issueHeader)
        {
            // Ensure the data is an integer.
            int data;
            bool validInt = int.TryParse(dataString, out data);
            if (validInt == false)
            {
                // The data is not a valid integer. Add an issue to the list.
                issues.Add(issueHeader + valueName + " is not valid. Read: \"" + dataString + "\", expected an integer value.");
            }

            // If no issues were found, the issues list is unchanged.
        }

        /// <summary>
        /// Ensures the given value is a valid integer and within the given range.
        /// </summary>
        /// <param name="dataString">The value to validate.</param>
        /// <param name="min">The lower bound of the accepted range (inclusive).</param>
        /// <param name="max">The upper bound of the accepted range (inclusive).</param>
        /// <param name="valueName">The name of the value being validated (used in issue messages).</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static void ValidateIntegerInRange(string dataString, int min, int max, string valueName, ref List<string> issues, string issueHeader)
        {
            // Ensure the data is an integer.
            int data;
            bool validInt = int.TryParse(dataString, out data);
            if (validInt == false)
            {
                // The data is not a valid integer. Add an issue to the list.
                issues.Add(issueHeader + valueName + " is not valid. Read: \"" + dataString + "\", expected an integer value in the range " + min + " - " + max + ".");

                // Return, as no additional validation is possible.
                return;
            }

            // Ensure the data is in the accepted range.
            if (data < min || data > max)
            {
                // The data is outside of the accepted range. Add an issue to the list.
                issues.Add(issueHeader + valueName + " is outside the expected range. Read: \"" + data + "\", expected an integer value in the range " + min + " - " + max + ".");
            }

            // If no issues were found, the issues list is unchanged.
        }

        /// <summary>
        /// Ensures the given value is not empty and one of the values in the list of given values.
        /// </summary>
        /// <param name="dataString">The value to validate.</param>
        /// <param name="validValues">The list of valid values.</param>
        /// <param name="valueName">The name of the value being validated (used in issue messages).</param>
        /// <param name="issues">A reference to the list of validation issues. New validation issues will be appended to this list.</param>
        /// <param name="issueHeader">The standard header to add to the beginning of each new issue appended to the issues list.</param>
        private static void ValidateEnum(string dataString, string[] validValues, string valueName, ref List<string> issues, string issueHeader)
        {
            // Ensure the data is not empty.
            if (string.IsNullOrEmpty(dataString))
            {
                // The data is empty. Add an issue to the list.
                issues.Add(issueHeader + valueName + " is empty. Expected one of the following values: " + validValues.ToString() + ".");

                // Return, as no additional validation is possible.
                return;
            }

            // Ensure the data is one of the accepted values.
            if (validValues.Contains(dataString.ToLower()) == false)
            {
                // The data is not one of the accepted values. Add an issue to the list.
                issues.Add(issueHeader + valueName + " is invalid. Read: \"" + dataString + "\", expected one of the following values: " + DataValidator.VideoNames.ToString() + ".");
            }

            // If no issues were found, the issues list is unchanged.
        }
        #endregion
    }
}
