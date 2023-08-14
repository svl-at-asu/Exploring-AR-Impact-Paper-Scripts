using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidationScripts
{
    /// <summary>
    /// Transforms the input data into a single table. See README file for expected input and output formats.
    /// </summary>
    internal static class DataTransformer
    {
        /// <summary>
        /// Transforms the input data tables into a single output event table. See README file for expected input and output formats.
        /// </summary>
        /// <param name="inputData">The table data in a 2D string arrays indexed by row then column, keyed by table name.</param>
        /// <param name="trialData">The trial data in a 2D string arrays indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns></returns>
        internal static List<string[]> TransformData(Dictionary<string, string[][]> inputData, string[][] trialData, out List<string> issues)
        {
            // Initialize the issues list.
            issues = new List<string>();

            // Initialize the transformed data.
            List<string[]> transformedData = new List<string[]>();

            // Transform the Gestures.
            for (int gesturesLineNumber = 1; gesturesLineNumber < inputData["Gestures"].Length; gesturesLineNumber++)
            {
                // Get a reference to the line.
                string[] gestureLine = inputData["Gestures"][gesturesLineNumber];

                // Find the Trial.
                string trialString = "";
                string trialTimeString = "";
                string chartType = "";
                string taskType = "";
                string taskNumber = "";
                int trialIndex = DataTransformer.GetIndexOfTrialRecord(trialData, gestureLine[0], gestureLine[1], gestureLine[2]);
                if (trialIndex == -1)
                {
                    // The event's time stamp does not fall into any of the trials. Add an issue to the list.
                    issues.Add("Gestures line " + gesturesLineNumber + " - Event's time stamp is outside of all trial times. Read: \"" + gestureLine[2] + "\", expected a time stamp falling within the trial times for Team " + gestureLine[0] + " " + gestureLine[1] + " video.");
                }
                else
                {
                    trialString = trialData[trialIndex][1];

                    // Calculate the Trial Time.
                    TimeSpan trialTime = DataTransformer.GetTrialTime(gestureLine[2], trialData[trialIndex][7]);
                    trialTimeString = DataTransformer.GetTimeStampString(trialTime);

                    // Get the chart and task data.
                    chartType = trialData[trialIndex][3];
                    taskType = trialData[trialIndex][4];
                    taskNumber = trialData[trialIndex][5];
                }

                // Get the Modality.
                string modality = gestureLine[1].Contains("Hololens") ?
                    "Hololens" :
                    "Desktop";

                // Transform the data into the overall events record format.
                string[] transformedLine =
                {
                    gestureLine[0],     // Team
                    trialString,        // Trial
                    modality,           // Modality
                    chartType,          // Chart Type
                    taskType,           // Task Type
                    taskNumber,         // Task Number
                    trialTimeString,    // Trial Time
                    "Gesture",          // Event Type
                    gestureLine[4],     // Participant
                    gestureLine[3],     // Action
                    gestureLine[5],     // Action Target
                    gestureLine[6],     // Action Intent
                    ""                  // Utterance Purpose
                };

                // Add the transformed data line to the transformed data.
                transformedData.Add(transformedLine);
            }

            // Transform the Looks.
            for (int looksLineNumber = 1; looksLineNumber < inputData["Looks"].Length; looksLineNumber++)
            {
                // Get a reference to the line.
                string[] looksLine = inputData["Looks"][looksLineNumber];

                // Find the Trial.
                string trialString = "";
                string trialTimeString = "";
                string chartType = "";
                string taskType = "";
                string taskNumber = "";
                int trialIndex = DataTransformer.GetIndexOfTrialRecord(trialData, looksLine[0], looksLine[1], looksLine[2]);
                if (trialIndex == -1)
                {
                    // The event's time stamp does not fall into any of the trials. Add an issue to the list.
                    issues.Add("Looks line " + looksLineNumber + " - Event's time stamp is outside of all trial times. Read: \"" + looksLine[2] + "\", expected a time stamp falling within the trial times for Team " + looksLine[0] + " " + looksLine[1] + " video.");
                }
                else
                {
                    trialString = trialData[trialIndex][1];

                    // Calculate the Trial Time.
                    TimeSpan trialTime = DataTransformer.GetTrialTime(looksLine[2], trialData[trialIndex][7]);
                    trialTimeString = DataTransformer.GetTimeStampString(trialTime);

                    // Get the chart and task data.
                    chartType = trialData[trialIndex][3];
                    taskType = trialData[trialIndex][4];
                    taskNumber = trialData[trialIndex][5];
                }

                // Get the Modality.
                string modality = looksLine[1].Contains("Hololens") ?
                    "Hololens" :
                    "Desktop";

                // Transform the data into the overall events record format.
                string[] transformedLine =
                {
                    looksLine[0],       // Team
                    trialString,        // Trial
                    modality,           // Modality
                    chartType,          // Chart Type
                    taskType,           // Task Type
                    taskNumber,         // Task Number
                    trialTimeString,    // Trial Time
                    "Look",             // Event Type
                    looksLine[3],       // Participant
                    "",                 // Action
                    "",                 // Action Target
                    "",                 // Action Intent
                    ""                  // Utterance Purpose
                };

                // Add the transformed data line to the transformed data.
                transformedData.Add(transformedLine);
            }

            // Transform the Utterances.
            for (int utteranceLineNumber = 1; utteranceLineNumber < inputData["Utterances"].Length; utteranceLineNumber++)
            {
                // Get a reference to the line.
                string[] utteranceLine = inputData["Utterances"][utteranceLineNumber];

                // Find the Trial.
                string trialString = "";
                string trialTimeString = "";
                string chartType = "";
                string taskType = "";
                string taskNumber = "";
                int trialIndex = DataTransformer.GetIndexOfTrialRecord(trialData, utteranceLine[0], utteranceLine[1], utteranceLine[2]);
                if (trialIndex == -1)
                {
                    // The event's time stamp does not fall into any of the trials. Add an issue to the list.
                    issues.Add("Utterances line " + utteranceLineNumber + " - Event's time stamp is outside of all trial times. Read: \"" + utteranceLine[2] + "\", expected a time stamp falling within the trial times for Team " + utteranceLine[0] + " " + utteranceLine[1] + " video.");
                }
                else
                {
                    trialString = trialData[trialIndex][1];

                    // Calculate the Trial Time.
                    TimeSpan trialTime = DataTransformer.GetTrialTime(utteranceLine[2], trialData[trialIndex][7]);
                    trialTimeString = DataTransformer.GetTimeStampString(trialTime);

                    // Get the chart and task data.
                    chartType = trialData[trialIndex][3];
                    taskType = trialData[trialIndex][4];
                    taskNumber = trialData[trialIndex][5];
                }

                // Get the Modality.
                string modality = utteranceLine[1].Contains("Hololens") ?
                    "Hololens" :
                    "Desktop";

                // Transform the data into the overall events record format.
                string[] transformedLine =
                {
                    utteranceLine[0],   // Team
                    trialString,        // Trial
                    modality,           // Modality
                    chartType,          // Chart Type
                    taskType,           // Task Type
                    taskNumber,         // Task Number
                    trialTimeString,    // Trial Time
                    "Utterance",        // Event Type
                    utteranceLine[4],   // Participant
                    "",                 // Action
                    "",                 // Action Target
                    "",                 // Action Intent
                    utteranceLine[3],   // Utterance Purpose
                    utteranceLine[5],   // Deictic Pronouns
                    utteranceLine[6]    // Spatial Deictic
                };

                // Add the transformed data line to the transformed data.
                transformedData.Add(transformedLine);
            }

            // Return the transformed data.
            return transformedData;
        }

        /// <summary>
        /// Gets the index of the trial record for the trial the given event falls into. Returns -1 if the given event does not fall into any trial time ranges.
        /// </summary>
        /// <param name="trialData">The trial data containing trial numbers and start and end times.</param>
        /// <param name="team">The team the event is for.</param>
        /// <param name="video">The video the event was observed in.</param>
        /// <param name="eventTimeStamp">The video time stamp of the event.</param>
        /// <returns>The index of the trial record for the trial the given event falls into, or -1 if the given event does not fall into any trial time ranges.</returns>
        private static int GetIndexOfTrialRecord(string[][] trialData, string team, string video, string eventTimeStamp)
        {
            // Check to see which trial the given event time falls within.
            TimeSpan eventTime = DataTransformer.ParseTimeStamp(eventTimeStamp);
            
            for (int recordIndex = 1; recordIndex < trialData.Length; recordIndex++)
            {
                // Get a reference to the record.
                string[] trialRecord = trialData[recordIndex];

                // Check to see if the event time stamp falls within the times of the trial.
                TimeSpan trialStart = DataTransformer.ParseTimeStamp(trialRecord[7]);
                TimeSpan trialEnd = DataTransformer.ParseTimeStamp(trialRecord[8]);

                if (trialRecord[0] == team &&
                    trialRecord[6] == video &&
                    eventTime >= trialStart && eventTime <= trialEnd)
                {
                    return recordIndex;
                }
            }

            // If no value was returned yet, then no record was found.
            return -1;
        }

        /// <summary>
        /// Gets the time of the event relative to the trial.
        /// </summary>
        /// <param name="eventTimeStamp">The video time stamp of the event.</param>
        /// <param name="trialStartTime">The video time stamp of the trial's start.</param>
        /// <returns>The time the event occurs after the trial's start time.</returns>
        private static TimeSpan GetTrialTime(string eventTimeStamp, string trialStartTime)
        {
            TimeSpan trialStart = DataTransformer.ParseTimeStamp(trialStartTime);
            TimeSpan eventTime = DataTransformer.ParseTimeStamp(eventTimeStamp);

            return eventTime - trialStart;
        }

        /// <summary>
        /// Parses a time stamp in the format "mm:ss" or "m:ss".
        /// </summary>
        /// <param name="timeStamp">The time stamp to parse.</param>
        /// <returns></returns>
        private static TimeSpan ParseTimeStamp(string timeStamp)
        {
            string[] parts = timeStamp.Split(':');
            return new TimeSpan(0, int.Parse(parts[0]), int.Parse(parts[1]));
        }

        /// <summary>
        /// Converts a time span to a string in the format "mm:ss".
        /// </summary>
        /// <param name="timeSpan">The time span to convert to string.</param>
        /// <returns></returns>
        private static string GetTimeStampString(TimeSpan timeSpan)
        {
            // Add a leading zero if the seconds or minutes is a single digit.
            string minutesString = timeSpan.Minutes < 10 ?
                "0" + timeSpan.Minutes.ToString() :
                timeSpan.Minutes.ToString();
            string secondsString = timeSpan.Seconds < 10 ? 
                "0" + timeSpan.Seconds.ToString() :
                timeSpan.Seconds.ToString();

            return minutesString + ":" + secondsString;
        }
    }
}
