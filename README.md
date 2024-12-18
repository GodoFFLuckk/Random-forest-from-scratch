# Random-forest-from-scratch
## Project Overview
This application is designed to predict heart attacks. Its core is a Random Forest algorithm that analyzes both the target dataset and an artificially generated dataset that I added to provide more opportunities to observe the algorithm's performance.
The target dataset link : https://www.kaggle.com/datasets/pritsheta/heart-attack/data

## Main Components
### Hyperparameter Tuning
The user configures the system's hyperparameters, specifically for the model and the dataset. For the model, the user can adjust the data volume to be used for testing, tree depth, the minimum number of instances for splitting, the number of trees, the criterion used and the seed.
The dataset can be either a pre-existing one or artificially generated. The hyperparameters for the dataset include the number of classes, features, instances, and the seed.
### Data preprocessing
In the case if prepared dataset is selected, we will use preprocessing is implemented using the CSVProcessor class. After obtaining the data from the CSV, we split it using the TrainTestSplit function. LINQ library is used for data manipulation, which conveniently allows selecting the required data.
### Training
After splitting the data using the TrainTestSplit function, we proceed to training. Each decision tree is represented by an instance of the DecisionTree class. Thanks to the nature of the algorithm, we can train each tree independently of the others. This allows us to leverage a parallel for loop to speed up the training process by concurrently executing the .fit method for each tree.

The .fit method implements the decision tree training algorithm through recursive calls to the SplitRecursively function in the Node class. This function handles the following tasks:

- Feature Subsampling: Features are randomly sampled using the SubsampleFeatures method based on a subsampling ratio.
- Decision Quality Evaluation: The GetBestDecision method evaluates candidate split points based on the selected criterion (e.g., Gini index or entropy).
- Recursive Splitting: Data is divided into left and right child nodes based on the best split decision, and the process is repeated until stopping conditions are met (e.g., tree depth or minimum samples for a split).

This design ensures that each decision tree is built efficiently while taking advantage of parallel execution for training the forest.
### Predict
After completing the training phase, we proceed to the prediction stage. Each tree in the ensemble votes for the predicted label of the test samples. The process works as follows:
- The PredictSingle method recursively traverses the decision tree from the root node to the leaves, determining the prediction based on the split decisions at each node.
- The predictions from all trees are collected into a dictionary, where the labels correspond to the number of votes.
- The label with the most votes (majority vote) is selected as the final prediction.
- To accelerate the prediction process, parallel execution is utilized through Parallel.ForEach, which processes test samples independently.

Prediction in RandomForest is efficient due to parallelization and the ensemble of trees, which compensates for the errors of individual models.
### Dataset generation
If option 'use atrificial dataset' has been choosed, we proceed to the dataset generation stage. The artificial dataset is created with samples, features, and target classes structured as follows:

- Feature and Target Assignment:
Each sample is assigned a target class, randomly selected from the specified number of classes. The features are then generated based on the assigned class:
Features divisible by 3 are calculated using the sin function with added noise: Math.Sin(targetClass + noise).
Features with indices yielding 1 modulo 3 use the cosine function: Math.Cos(targetClass + noise).
Remaining features are linearly scaled by the class value with noise: targetClass * 2 + noise.
The noise is sampled from a normal distribution and ensures variability in the feature values.

- Dataset Splitting:
Once all samples are generated, the data is divided into training and testing subsets. The test size is determined by the testPercentage parameter (defaulting to 20%), ensuring a balanced split for model evaluation.

- Output Formatting:
The generated dataset includes feature headers (e.g., "Feature 1", "Feature 2") for readability and separate lists for training and testing data, along with their corresponding target labels.

This method ensures a reproducible and structured dataset generation process, introducing controlled randomness to simulate real-world data variability while maintaining a clear relationship between features and target classes.