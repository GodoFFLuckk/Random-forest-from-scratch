using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Random_Forest
{
    /// <summary>
    /// Provides functionality to process CSV files.
    /// </summary>
    public class CSVProcessor
    {
        /// <summary>
        /// Loads data from a CSV file.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <returns>A list of string arrays representing rows and columns from the CSV file.</returns>
        public List<string[]> LoadCSV(string path)
        {
            var lines = File.ReadAllLines(path).ToList();
            List<string[]> data = new List<string[]>();

            foreach (var line in lines)
            {
                string[] items = line.Split(',');
                data.Add(items);
            }

            return data;
        }

        /// <summary>
        /// Processes and prints the data from a list of string arrays (typically obtained from a CSV).
        /// </summary>
        /// <param name="data">The data to process.</param>
        public void ProcessData(List<string[]> data)
        {
            foreach (var row in data)
            {
                foreach (var item in row)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}
