using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
/*
--------------------------------------------------------------------------------
Project: RandomForest Implementation

Description:
This project involves the design and implementation of the Random Forest algorithm, a versatile ensemble learning method, primarily used for classification and regression tasks. It operates by constructing multiple decision trees during training and then outputs the mode of the class labels (for classification) or mean prediction (for regression) of individual trees.

Key Features:
1. Ensemble of Decision Trees: Utilizes multiple trees to minimize overfitting.
2. Feature Randomness: At each split in the decision tree, a random subset of features is considered, enhancing diversity.
3. Bootstrap Aggregation (Bagging): Each decision tree is trained on a randomly sampled subset of the data.
4. Hyperparameter Tuning: Allows optimization for parameters like the number of trees, tree depth, etc.

Technical Specifications:
- Programming Language: C#

Developer:
- Alikhan Akhmatov

--------------------------------------------------------------------------------
*/
namespace Random_Forest
{
    /// <summary>
    /// Represents the main window of the application.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private int GetValidatedInt(string input, string errorMessage, string paramName, int minValue)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(errorMessage);
            int value = Convert.ToInt32(input);
            if (value < minValue)
                throw new ArgumentException($"{paramName} must be >= {minValue}");
            return value;
        }

        private double GetValidatedFeatureSubsampling(string input, string errorMessage, double minValue, double maxValue)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(errorMessage);
            double value = Convert.ToDouble(input);
            if (value <= minValue || value > maxValue)
                throw new ArgumentException($"{errorMessage} (Must be between {minValue} and {maxValue})");
            return value;
        }

        private double GetValidatedTestSize(string input, string errorMessage, double minValue, double maxValue)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(errorMessage);
            double value = Convert.ToDouble(input);
            if (value <= minValue || value >= maxValue)
                throw new ArgumentException($"{errorMessage} (Must be between {minValue} and {maxValue})");
            return value;
        }

        /// <summary>
        /// Converts a list of string arrays into a list of double arrays.
        /// </summary>
        /// <param name="inputList">List of string arrays to convert.</param>
        /// <returns>List of double arrays.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a string can't be converted to double.</exception>
        static List<double[]> ConvertStringListToDoubleList(List<string[]> inputList)
        {
            List<double[]> outputList = new List<double[]>();

            foreach (var item in inputList)
            {
                double[] doubleArray = new double[item.Length];
                for (int i = 0; i < item.Length; i++)
                {
                    if (double.TryParse(item[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result))
                    {
                        doubleArray[i] = result;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Can't convert '{item[i]}' into double.");
                    }
                }
                outputList.Add(doubleArray);
            }

            return outputList;
        }
        /// <summary>
        /// Converts the first item of each string array in the list into a list of doubles.
        /// </summary>
        /// <param name="inputList">List of string arrays to convert.</param>
        /// <returns>List of doubles.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a string can't be converted to double.</exception>
        static List<double> ConvertStringListToDoubleListTarget(List<string[]> inputList)
        {
            List<double> outputList = new List<double>();

            foreach (var item in inputList)
            {
                if (double.TryParse(item[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result))
                {
                    outputList.Add(result);
                }
                else
                {
                    throw new InvalidOperationException($"Can't convert '{item[0]}' into double.");
                }
            }

            return outputList;
        }
        /// <summary>
        /// Splits the input data into training and testing sets.
        /// </summary>
        /// <param name="testSize">Proportion of data to be used for testing (between 0 and 1).</param>
        /// <param name="input">The input data.</param>
        /// <returns>Data object containing training and testing data and targets.</returns>
        static Data TrainTestSplit(double testSize, List<string[]> input)
        {
            string[] headers = input.First();
            input = input.Skip(1).ToList();
            int testCount = (int)(input.Count * testSize);

            if(testCount == 0)
            {
                testCount = 1;
            }
            else if(testCount == input.Count)
            {
                testCount = input.Count - 1;
            }

            var test = input.Take(testCount).ToList();
            var train = input.Skip(testCount).ToList();

            var trainData = train.Select(item => item.Take(item.Length - 1).ToArray()).ToList();
            var trainTarget = train.Select(item => new string[] { item.Last() }).ToList();

            var testData = test.Select(item => item.Take(item.Length - 1).ToArray()).ToList();
            var testTarget = test.Select(item => new string[] { item.Last() }).ToList();
            List<double[]> trainDataDoubles = ConvertStringListToDoubleList(trainData);
            List<double> trainTargetDoubles = ConvertStringListToDoubleListTarget(trainTarget);
            List<double[]> testDataDoubles = ConvertStringListToDoubleList(testData);
            List<double> testTargetDoubles = ConvertStringListToDoubleListTarget(testTarget);
            return new Data(headers, trainDataDoubles, trainTargetDoubles, testDataDoubles, testTargetDoubles);
        }
        /// <summary>
        /// Calculates the accuracy between true values and predicted values.
        /// </summary>
        /// <param name="trueValues">Actual values.</param>
        /// <param name="predictedValues">Predicted values.</param>
        /// <returns>Accuracy of predictions.</returns>
        /// <exception cref="ArgumentException">Thrown when the lists do not have the same size.</exception>
        static double CalculateAccuracy(List<double> trueValues, List<double> predictedValues)
        {
            if (trueValues.Count != predictedValues.Count)
                throw new ArgumentException("The lists must have the same size.");

            int correctPredictions = 0;

            for (int i = 0; i < trueValues.Count; i++)
            {
                if (trueValues[i] == predictedValues[i])
                    correctPredictions++;
            }

            return (double)correctPredictions / trueValues.Count;
        }
        /// <summary>
        /// Handles the MouseLeftButtonDown event, which allows the user to drag the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Generates an artificial dataset.
        /// </summary>
        /// <param name="numSamples">The total number of data samples (both training and testing).</param>
        /// <param name="numFeatures">The number of features per sample.</param>
        /// <param name="numClasses">The number of distinct target classes.</param>
        /// <param name="testPercentage">The percentage of data samples used for testing. E.g. 0.2 for 20% testing data.</param>
        /// <returns>An instance of the <see cref="Data"/> struct with generated data.</returns>
        public static Data GenerateArtificialDataset(int seed, int numSamples, int numFeatures, int numClasses, double testPercentage = 0.2)
        {
            Random rand = new Random(seed);

            List<double[]> allData = new List<double[]>();
            List<double> allTarget = new List<double>();

            // Generate headers
            string[] headers = Enumerable.Range(1, numFeatures).Select(i => $"Feature {i}").ToArray();

            // Generate data and targets
            for (int i = 0; i < numSamples; i++)
            {
                int targetClass = rand.Next(numClasses);
                allTarget.Add(targetClass);

                double[] sample = new double[numFeatures];
                double noise;

                for (int j = 0; j < numFeatures; j++)
                {
                    noise = GenerateNormal(rand) * 0.015;

                    if (j % 3 == 0)
                    {
                        sample[j] = Math.Sin(targetClass + noise);
                    }
                    else if (j % 3 == 1)
                    {
                        sample[j] = Math.Cos(targetClass + noise);
                    }
                    else
                    {
                        sample[j] = targetClass * 2 + noise;
                    }

                    sample[j] = Math.Round(sample[j], 2);
                }

                allData.Add(sample);
            }

            // Split data into training and testing
            int numTestSamples = (int)(numSamples * testPercentage);
            int numTrainSamples = numSamples - numTestSamples;

            List<double[]> trainData = allData.GetRange(0, numTrainSamples);
            List<double[]> testData = allData.GetRange(numTrainSamples, numTestSamples);

            List<double> trainTarget = allTarget.GetRange(0, numTrainSamples);
            List<double> testTarget = allTarget.GetRange(numTrainSamples, numTestSamples);

            return new Data(headers, trainData, trainTarget, testData, testTarget);
        }

        /// <summary>
        /// Simulates a value from a normal distribution using the Central Limit Theorem.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="mean">The mean of the distribution. Defaults to 0.</param>
        /// <param name="stdDev">The standard deviation of the distribution. Defaults to 1.</param>
        /// <returns>A value sampled from the specified normal distribution.</returns>
        private static double GenerateNormal(Random rand, double mean = 0, double stdDev = 1)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return 100*(mean + stdDev * randStdNormal);
        }


        /// <summary>
        /// Handles the event when the Start button is clicked. 
        /// Loads a CSV file, processes the data, trains a random forest, and visualizes the results.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            double testSize;
            List<string[]> input = new List<string[]>();
            CSVProcessor processor = new CSVProcessor();
            input = processor.LoadCSV("heart.csv");
            Data data;
            int seed;
            int datasetSeed;
            int maxDepth;
            int minToSplit;
            int treesNumber;
            double featureSubsampling;
            int numOfSamples;
            int numOfFeatures;
            int numOfClasses;
            try
            {
                testSize = GetValidatedTestSize(TestSizeTextBox.Text, "You need to write test size number.", 0, 1);
                seed = GetValidatedInt(SeedForestTextBox.Text, "Seed cannot be empty.", "Seed", 0);
                maxDepth = GetValidatedInt(TreeDepthTextBox.Text, "You need to write depth of trees.", "Depth of trees", 1);
                minToSplit = GetValidatedInt(MinToSplitTextBox.Text, "You need to write minimum examples required to split number.", "Number of minimum examples", 1);
                treesNumber = GetValidatedInt(TreeNumberTextBox.Text, "You need to write number of trees.", "Number of trees", 1);
                featureSubsampling = GetValidatedFeatureSubsampling(FeautureSubsamplingTextBox.Text, "You need to write feature subsampling number.", 0, 1);

                if (YesRadioButton.IsChecked == true)
                {
                    numOfSamples = GetValidatedInt(NumSamplesTextBoxTextBox.Text, "You need to write number of samples", "Number of samples", 1);
                    numOfFeatures = GetValidatedInt(NumFeaturesTextBoxTextBox.Text, "You need to write number of features", "Number of features", 1);
                    numOfClasses = GetValidatedInt(NumClassesTextBox.Text, "You need to write number of classes", "Number of classes", 2);
                    datasetSeed = GetValidatedInt(SeedGenerationTextBox.Text, "Seed cannot be empty.", "Seed", 0);
                    data = GenerateArtificialDataset(datasetSeed, numOfSamples, numOfFeatures, numOfClasses, testSize);
                }
                else if (NoRadioButton.IsChecked == true)
                {
                    data = TrainTestSplit(testSize, input);
                }
                else
                {
                    throw new Exception("Dataset isn't selected.");
                }

                string criterion;
                if (GiniRadioButton.IsChecked == true)
                {
                    criterion = "gini";
                }
                else
                {
                    criterion = "entropy";
                }

                RandomForest forest;
                if (criterion == "gini")
                {
                    forest = new RandomForest(seed, maxDepth, minToSplit, treesNumber, featureSubsampling, "gini");
                }
                else
                {
                    forest = new RandomForest(seed, maxDepth, minToSplit, treesNumber, featureSubsampling, "entropy");
                }
                forest.Fit(data);
                var predictTrain = forest.Predict(data.trainData);
                var predictTest = forest.Predict(data.testData);
                var accTrain = CalculateAccuracy(predictTrain, data.trainTarget);
                var accTest = CalculateAccuracy(predictTest, data.testTarget);
                Visualization visualizationWindow = new Visualization
                {
                    AccTrain = accTrain,
                    AccTest = accTest
                };
                foreach (var tree in forest.Trees)
                {
                    visualizationWindow.AddTree(tree);
                }
                visualizationWindow.Show();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unhandled exception: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

