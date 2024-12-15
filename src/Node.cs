using System;
using System.Collections.Generic;
using System.Linq;

namespace Random_Forest
{
    /// <summary>
    /// Represents a node in the decision tree.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Indicates if the node is a leaf node.
        /// </summary>
        public bool isLeaf { get; set; }

        /// <summary>
        /// Left child node.
        /// </summary>
        public Node left { get; set; }

        /// <summary>
        /// Right child node.
        /// </summary>
        public Node right { get; set; }

        private int depth;
        private DecisionTree parentTree;

        /// <summary>
        /// Best decision at this node based on training data.
        /// </summary>
        public Decision bestDecision { get; set; }

        /// <summary>
        /// Label assigned to this node if it's a leaf.
        /// </summary>
        public double label { get; set; }

        public int examplesCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the Node class.
        /// </summary>
        /// <param name="tree">Parent tree of this node.</param>
        /// <param name="depth">Depth of this node in the decision tree.</param>
        public Node(DecisionTree tree, int depth)
        {
            this.depth = depth;
            isLeaf = false;
            parentTree = tree;
        }

        /// <summary>
        /// Determines if the node can be split.
        /// </summary>
        /// <returns>True if the node can be split, false otherwise.</returns>
        public bool CanSplit(int examplesCount)
        {
            return depth < parentTree.maxDepth && examplesCount>this.parentTree.minToSplit && examplesCount > 1;
        }

        /// <summary>
        /// Extracts specified columns from a matrix.
        /// </summary>
        /// <param name="matrix">Input matrix.</param>
        /// <param name="columns">Columns to extract.</param>
        /// <returns>Extracted columns from the matrix.</returns>
        public static List<double[]> ExtractColumns(List<double[]> matrix, List<int> columns)
        {
            return matrix.Select(row => columns.Select(col => row[col]).ToArray()).ToList();
        }

        /// <summary>
        /// Subsamples features using the parent tree's subsampling ratio and generator.
        /// </summary>
        /// <param name="numberOfFeatures">Total number of features.</param>
        /// <returns>A boolean array where true indicates the feature is selected.</returns>
        public bool[] SubsampleFeatures(int numberOfFeatures)
        {
            bool[] result = new bool[numberOfFeatures];

            int countTrueValues = (int)Math.Ceiling(parentTree.featureSubsampling * numberOfFeatures);

            for (int i = 0; i < countTrueValues; i++)
            {
                result[i] = true;
            }

            Shuffle(result);

            return result;
        }
        /// <summary>
        /// Shuffles the elements of the specified array using the Fisher-Yates algorithm.
        /// </summary>
        /// <param name="array">The boolean array to shuffle.</param>
        /// <remarks>
        /// The Fisher-Yates shuffle algorithm ensures an equal likelihood for all possible permutations of the array elements.
        /// This method leverages the random number generator <see cref="parentTree.generatorFeatureSubsampling"/> to ensure the randomness of the shuffle.
        /// </remarks>
        private void Shuffle(bool[] array)
        {
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = parentTree.generatorFeatureSubsampling.Next(i + 1);
                bool temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }


        /// <summary>
        /// Sorts columns of a matrix of doubles in ascending order.
        /// </summary>
        /// <param name="input">Input matrix.</param>
        public static void SortColumns(List<double[]> input)
        {
            int rowCount = input.Count;
            if (rowCount == 0) return;

            int colCount = input[0].Length;

            for (int col = 0; col < colCount; col++)
            {
                var sortedColumn = input.Select(row => row[col])
                                        .OrderBy(val => val)
                                        .ToList();

                for (int row = 0; row < rowCount; row++)
                {
                    input[row][col] = sortedColumn[row];
                }
            }
        }
        /// <summary>
        /// Computes the average values for each column in a matrix.
        /// </summary>
        /// <param name="matrix">Input matrix.</param>
        /// <returns>A list of average values for each column.</returns>
        public static List<double[]> ComputeAverageValues(List<double[]> matrix)
        {
            List<double[]> argValues = new List<double[]>();

            // Transpose the matrix
            int rowCount = matrix.Count;
            int colCount = matrix[0].Length;
            List<double[]> transposedMatrix = new List<double[]>();
            for (int i = 0; i < colCount; i++)
            {
                double[] column = new double[rowCount];
                for (int j = 0; j < rowCount; j++)
                {
                    column[j] = matrix[j][i];
                }
                transposedMatrix.Add(column);
            }

            // For each feature (row in the transposed matrix)
            foreach (var featureValues in transposedMatrix)
            {
                var uniqueSortedValues = featureValues.Distinct().OrderBy(val => val).ToList();
                List<double> avgValuesList = new List<double>();

                if (uniqueSortedValues.Count > 1)
                {
                    for (int j = 0; j < uniqueSortedValues.Count - 1; j++)
                    {
                        double a = uniqueSortedValues[j];
                        double b = uniqueSortedValues[j + 1];
                        avgValuesList.Add((a + b) / 2);
                    }
                }
                else
                {
                    avgValuesList.Add(uniqueSortedValues[0]);
                }

                argValues.Add(avgValuesList.ToArray());
            }

            return argValues;
        }
        /// <summary>
        /// Computes the Gini index for a given list of target values.
        /// </summary>
        /// <param name="Y">List of target values.</param>
        /// <returns>The computed Gini index.</returns>
        public double ComputeGiniIndex(List<double> Y)
        {
            int cardinality = Y.Count;

            Dictionary<double, int> groupCounts = new Dictionary<double, int>();
            for (int i = 0; i < Y.Count; i++)
            {
                if (!groupCounts.ContainsKey(Y[i]))
                {
                    groupCounts[Y[i]] = 0;
                }
                groupCounts[Y[i]]++;
            }

            double sum = 0;
            foreach (var group in groupCounts)
            {
                double p_k = (double)group.Value / cardinality;
                sum += p_k * (1 - p_k);
            }

            return cardinality * sum;
        }
        /// <summary>
        /// Computes the entropy for a given list of target values.
        /// </summary>
        /// <param name="Y">List of target values.</param>
        /// <returns>The computed entropy.</returns>
        public static double ComputeEntropy(List<double> Y)
        {
            Dictionary<double, int> valueCounts = new Dictionary<double, int>();
            int cardinality = Y.Count;

            for (int i = 0; i < cardinality; i++)
            {
                double value = Y[i];
                if (valueCounts.ContainsKey(value))
                {
                    valueCounts[value]++;
                }
                else
                {
                    valueCounts.Add(value, 1);
                }
            }

            double sum = 0;
            foreach (var keyValuePair in valueCounts)
            {
                double p_k = (double)keyValuePair.Value / cardinality;
                if (p_k != 0)
                {
                    sum += p_k * Math.Log(p_k);
                }
            }

            return sum * cardinality * -1;
        }
        /// <summary>
        /// Computes the quality of a decision based on the input data and decision rule.
        /// </summary>
        /// <param name="X">Input data matrix.</param>
        /// <param name="Y">Target values.</param>
        /// <param name="decision">Decision rule.</param>
        /// <returns>The quality metric of the decision.</returns>
        public double GetQualityOfDecision(List<double[]> X, List<double> Y, Decision decision)
        {
            bool[] XDecisions = decision.Applicate(X);
            List<double> YLeft = new List<double>();
            List<double> YRight = new List<double>();

            for (int i = 0; i < XDecisions.Length; i++)
            {
                if (XDecisions[i])
                    YLeft.Add(Y[i]);
                else
                    YRight.Add(Y[i]);
            }

            if (parentTree.criterion == "gini")
            {
                double giniLeft = ComputeGiniIndex(YLeft);
                double giniRight = ComputeGiniIndex(YRight);
                return giniLeft + giniRight;
            }
            else
            {
                double eL = ComputeEntropy(YLeft);
                double eR = ComputeEntropy(YRight);
                return eL + eR;
            }
        }
        /// <summary>
        /// Determines the best decision based on the input data and target values.
        /// </summary>
        /// <param name="X">Input data matrix.</param>
        /// <param name="Y">Target values.</param>
        /// <returns>The best decision.</returns>
        public Decision GetBestDecision(List<double[]> X, List<double> Y)
        {
            var features = SubsampleFeatures(X[0].Length);
            var choosedF = features.Select((val, idx) => new { val, idx })
                                   .Where(pair => pair.val)
                                   .Select(pair => pair.idx)
                                   .ToList();

            List<double[]> decisionsFNew = ExtractColumns(X, choosedF);
            SortColumns(decisionsFNew);
            var avg = ComputeAverageValues(decisionsFNew);
            var decisions = new List<Decision>();
            int counter = -1;
            for (int i = 0; i < choosedF.Count; i++)
            {
                counter++;
                foreach (var arg_value in avg[i])
                {
                    decisions.Add(new Decision(choosedF[counter], arg_value));
                }
            }
            var qualities = decisions.Select(decision => GetQualityOfDecision(X, Y, decision)).ToArray();

            int idxMin = Array.IndexOf(qualities, qualities.Min());
            var bestDecision = decisions[idxMin];

            return bestDecision;
        }
        /// <summary>
        /// Recursively splits the node based on the input data and target values.
        /// </summary>
        /// <param name="data">Input data matrix.</param>
        /// <param name="target">Target values.</param>
        public void SplitRecursively(List<double[]> data, List<double> target)
        {
            examplesCount = data.Count;
            if (!CanSplit(examplesCount))
            {
                isLeaf = true;
                Dictionary<double, int> counts = new Dictionary<double, int>();
                foreach (double yValue in target)
                {
                    if (counts.ContainsKey(yValue))
                        counts[yValue]++;
                    else
                        counts[yValue] = 1;
                }
                int maxCount = 0;
                foreach (var pair in counts)
                {
                    if (pair.Value > maxCount)
                    {
                        maxCount = pair.Value;
                        label = pair.Key;
                    }
                }
                return;
            }
            bestDecision = GetBestDecision(data, target);
            bool[] verifiedData = bestDecision.Applicate(data);

            List<double[]> X_left = new List<double[]>();
            List<double[]> X_right = new List<double[]>();

            List<double> Y_left = new List<double>();
            List<double> Y_right = new List<double>();

            for (int i = 0; i < verifiedData.Length; i++)
            {
                if (verifiedData[i])
                {
                    X_left.Add(data[i]);
                    Y_left.Add(target[i]);
                }
                else
                {
                    X_right.Add(data[i]);
                    Y_right.Add(target[i]);
                }
            }
            left = new Node(parentTree, depth + 1);
            right = new Node(parentTree, depth + 1);
            left.SplitRecursively(X_left, Y_left);
            right.SplitRecursively(X_right, Y_right);
        }
    }
}
