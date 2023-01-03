using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exploring_AR_Impact_Paper_Scripts
{
    /// <summary>
    /// Validates the input data. See README file for expected input formats.
    /// </summary>
    internal static class DataValidator
    {
        /// <summary>
        /// Validates the gesture data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The gesture data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static bool ValidateGesturesData(string[][] data, out string[] issues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates the look data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The look data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static bool ValidateLooksData(string[][] data, out string[] issues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates the utterance data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The utterance data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static bool ValidateUtterancesData(string[][] data, out string[] issues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates the utterances count data. See README file for expected input formats.
        /// </summary>
        /// <param name="data">The utterances count data in a 2D string array indexed by row then column.</param>
        /// <param name="issues">A list of issues found in the current data.</param>
        /// <returns>Whether the data is valid. The issues list is empty if true.</returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static bool ValidateUtterancesCountData(string[][] data, out string[] issues)
        {
            throw new NotImplementedException();
        }
    }
}
