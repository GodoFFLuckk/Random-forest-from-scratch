using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Random_Forest
{
    /// <summary>
    /// Represents a Random Forest classifier.
    /// </summary>
    public class RandomForest
    {
        private int treesNumber;
        private Random generatorBootstrapping;
        public List<DecisionTree> Trees = new List<DecisionTree>();
        public int featuresCount;
        /// <summary>
        /// Initializes a new instance of the RandomForest class.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each tree.</param>
        /// <param name="minToSplit">The minimum number of samples required to split an internal node.</param>
        /// <param name="numberOfTrees">The number of trees in the forest.</param>
        /// <param name="featureSubsampling">The proportion of features to consider for splits at each node.</param>
        /// <param name="criterion">The function to measure the quality of a split.</param>
        public RandomForest(int seed, int maxDepth, int minToSplit, int numberOfTrees, double featureSubsampling, string criterion)
        {
            generatorBootstrapping = new Random(seed);
            this.treesNumber = numberOfTrees;
            for (int i = 0; i < numberOfTrees; i++)
            {
                Trees.Add(new DecisionTree(maxDepth, minToSplit, generatorBootstrapping.Next(10000), featureSubsampling, criterion));
            }
        }

        /// <summary>
        /// Generates bootstrap indices for a given length using the specified random generator.
        /// </summary>
        /// <param name="length">The number of bootstrap indices to generate.</param>
        /// <param name="generator">The random number generator.</param>
        /// <returns>A list of bootstrap indices.</returns>
        public List<int> GenerateBootstrapIndices(int length, Random generator)
        {
            return Enumerable.Range(0, length)
                             .Select(_ => generator.Next(length))
                             .ToList();
        }

        /// <summary>
        /// Fits the RandomForest model using the provided data.
        /// </summary>
        /// <param name="data">The data used for training.</param>
        public void Fit(Data data)
        {
            Parallel.ForEach(Trees, (tree, loopState, treeIndex) =>
            {
                Random localGenerator = new Random(42 + (int)treeIndex);

                var trainData = data.trainData;
                var trainTarget = data.trainTarget;

                List<int> bootstrapIndices = GenerateBootstrapIndices(trainData.Count, localGenerator);
                List<double[]> trainDataBootstraped = bootstrapIndices.Select(index => trainData[index]).ToList();
                List<double> trainTargetBootstraped = bootstrapIndices.Select(index => trainTarget[index]).ToList();

                tree.Fit(trainDataBootstraped, trainTargetBootstraped);
            });
        }

        /// <summary>
        /// Predicts the labels for the provided test data.
        /// </summary>
        /// <param name="testData">The test data for prediction.</param>
        /// <returns>A list of predicted labels.</returns>
        public List<double> Predict(List<double[]> testData)
        {
            double[] predictions = new double[testData.Count];

            Parallel.ForEach(testData, (dataPoint, state, index) =>
            {
                Dictionary<double, int> votes = new Dictionary<double, int>();

                foreach (var tree in Trees)
                {
                    double predictedLabel = PredictSingle(tree.root, dataPoint);
                    lock (votes)
                    {
                        if (!votes.ContainsKey(predictedLabel))
                        {
                            votes[predictedLabel] = 0;
                        }
                        votes[predictedLabel]++;
                    }
                }

                double majorityVote = votes.OrderByDescending(v => v.Value).First().Key;
                predictions[index] = majorityVote;
            });

            return predictions.ToList();
        }

        /// <summary>
        /// Recursively predicts a single label for a given data point.
        /// </summary>
        /// <param name="node">The current node in the decision tree.</param>
        /// <param name="dataPoint">The data point to predict.</param>
        /// <returns>The predicted label.</returns>
        private double PredictSingle(Node node, double[] dataPoint)
        {
            if (node.isLeaf)
            {
                return node.label;
            }

            if (dataPoint[node.bestDecision.featureNum] < node.bestDecision.treshold)
            {
                return PredictSingle(node.left, dataPoint);
            }
            else
            {
                return PredictSingle(node.right, dataPoint);
            }
        }
    }
}
