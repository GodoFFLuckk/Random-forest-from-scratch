using System;
using System.Collections.Generic;

namespace Random_Forest
{
    /// <summary>
    /// Represents a decision tree for classification or regression tasks.
    /// </summary>
    public class DecisionTree
    {
        /// <summary>
        /// Gets the root node of the decision tree.
        /// </summary>
        public Node root { get; }

        /// <summary>
        /// Gets the maximum depth of the decision tree.
        /// </summary>
        public int maxDepth { get; }

        /// <summary>
        /// Gets the minimum number of examples required for a node to be split.
        /// </summary>
        public int minToSplit { get; }

        /// <summary>
        /// Gets the criterion used to evaluate the quality of a split.
        /// </summary>
        public string criterion { get; }

        /// <summary>
        /// Gets the fraction of features to be considered at each split.
        /// </summary>
        public double featureSubsampling { get; private set; }

        /// <summary>
        /// Gets the random number generator used for feature subsampling.
        /// </summary>
        public Random generatorFeatureSubsampling { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionTree"/> class.
        /// </summary>
        /// <param name="maxDepth">Maximum depth of the tree.</param>
        /// <param name="minToSplit">Minimum examples required to split a node.</param>
        /// <param name="seed">Seed for the random number generator used in feature subsampling.</param>
        /// <param name="featureSubsampling">Fraction of features to consider at each split.</param>
        /// <param name="criterion">Criterion to evaluate the quality of a split.</param>
        public DecisionTree(int maxDepth, int minToSplit, int seed, double featureSubsampling, string criterion)
        {
            this.maxDepth = maxDepth;
            this.minToSplit = minToSplit;
            root = new Node(this, 0);
            generatorFeatureSubsampling = new Random(seed);
            this.featureSubsampling = featureSubsampling;
            this.criterion = criterion;
        }

        /// <summary>
        /// Fits the decision tree to the provided training data and target values.
        /// </summary>
        /// <param name="trainData">The training data.</param>
        /// <param name="trainTarget">The target values associated with the training data.</param>
        public void Fit(List<double[]> trainData, List<double> trainTarget)
        {
            root.SplitRecursively(trainData, trainTarget);
        }
    }
}
