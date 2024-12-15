using System;
using System.Collections.Generic;

namespace Random_Forest
{
    /// <summary>
    /// Represents a dataset containing training and testing data.
    /// </summary>
    public struct Data
    {
        /// <summary>
        /// Gets or sets the training data.
        /// </summary>
        public List<double[]> trainData { get; set; }

        /// <summary>
        /// Gets or sets the testing data.
        /// </summary>
        public List<double[]> testData { get; set; }

        /// <summary>
        /// Gets or sets the target values for the training data.
        /// </summary>
        public List<double> trainTarget { get; set; }

        /// <summary>
        /// Gets or sets the target values for the testing data.
        /// </summary>
        public List<double> testTarget { get; set; }

        /// <summary>
        /// Contains headers for the data columns.
        /// </summary>
        private string[] headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> struct.
        /// </summary>
        /// <param name="headers">Headers for the data columns.</param>
        /// <param name="trainData">Training data.</param>
        /// <param name="trainTarget">Target values for the training data.</param>
        /// <param name="testData">Testing data.</param>
        /// <param name="testTarget">Target values for the testing data.</param>
        public Data(string[] headers, List<double[]> trainData, List<double> trainTarget, List<double[]> testData, List<double> testTarget)
        {
            this.trainData = trainData;
            this.testData = testData;
            this.trainTarget = trainTarget;
            this.testTarget = testTarget;
            this.headers = headers;
        }
    }
}
