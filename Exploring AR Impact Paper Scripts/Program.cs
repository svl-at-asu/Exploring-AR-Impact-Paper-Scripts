using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exploring_AR_Impact_Paper_Scripts
{
    internal class Program
    {
        /// <summary>
        /// Args are expected in the following order: Gestures, Looks, Utterances, Utterances Counts.
        /// </summary>
        private enum ArgsIndices
        {
            Gestures = 0,
            Looks = 1,
            Utterances = 2,
            UtterancesCount = 3
        }

        /// <summary>
        /// Expects a list of file names for the input data in the following order: Gestures, Looks, Utterances, Utterances Counts.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Read the file data.
            string[][] gesturesData;
            try
            {
                gesturesData = DataReader.ReadCsvData(args[(int)Program.ArgsIndices.Gestures]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
