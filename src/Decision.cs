using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Random_Forest
{
    /// <summary>
    /// Represents a decision rule based on a feature threshold.
    /// </summary>
    public class Decision
    {
        /// <summary>
        /// Gets the feature index on which the decision is based.
        /// </summary>
        public int featureNum { get; }

        /// <summary>
        /// Gets the threshold value for making a decision.
        /// </summary>
        public double treshold { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decision"/> class.
        /// </summary>
        /// <param name="f">The feature index on which the decision is based.</param>
        /// <param name="t">The threshold value for making a decision.</param>
        public Decision(int f, double t)
        {
            featureNum = f;
            treshold = t;
        }

        /// <summary>
        /// Applies the decision rule on a set of data.
        /// </summary>
        /// <param name="X">The data to apply the decision rule on.</param>
        /// <returns>A boolean array indicating the result of the decision for each item in the data.</returns>
        public bool[] Applicate(List<double[]> X)
        {
            bool[] result = new bool[X.Count];

            for (int i = 0; i < X.Count; i++)
            {
                result[i] = X[i][featureNum] < treshold;
            }

            return result;
        }
    }
}
