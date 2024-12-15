using System;
using System.Windows;
using System.Windows.Controls;

namespace Random_Forest
{
    /// <summary>
    /// Represents a visualization window for decision trees in the Random Forest algorithm.
    /// </summary>
    public partial class Visualization : Window
    {
        // Properties to hold the accuracy values for training and testing data.
        public double AccTrain { get; set; }
        public double AccTest { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Visualization"/> class.
        /// </summary>
        public Visualization()
        {
            // Initialize the window components.
            InitializeComponent();

            // Attach an event handler for the Loaded event of the window.
            this.Loaded += Visualization_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the Visualization window.
        /// Sets the text for the accuracy TextBlocks.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Visualization_Loaded(object sender, RoutedEventArgs e)
        {
            // Display the accuracy values on the corresponding TextBlocks.
            AccTrainTextBlock.Text = $"Accuracy Train: {AccTrain}";
            AccTestTextBlock.Text = $"Accuracy Test: {AccTest}";
        }

        /// <summary>
        /// Adds a visual representation of a DecisionTree to the window.
        /// </summary>
        /// <param name="tree">The DecisionTree to be visualized.</param>
        public void AddTree(DecisionTree tree)
        {
            // Create a new TreeView to represent the decision tree.
            var treeView = new TreeView();

            // Create the root node for the tree.
            var root = CreateTreeNode(tree.root);
            treeView.Items.Add(root);

            // Add the TreeView to the ItemsControl on the window.
            TreesControl.Items.Add(treeView);
        }

        /// <summary>
        /// Recursively creates a TreeViewItem to represent a Node of the DecisionTree.
        /// </summary>
        /// <param name="node">The node of the DecisionTree.</param>
        /// <returns>A TreeViewItem representing the provided Node.</returns>
        private TreeViewItem CreateTreeNode(Node node)
        {
            // Create a new TreeViewItem.
            var treeNode = new TreeViewItem();

            // Create a content presenter to display the data of the node using the defined DataTemplate.
            var contentPresenter = new ContentPresenter();
            contentPresenter.Content = node;
            contentPresenter.ContentTemplate = this.FindResource("NodeTemplate") as DataTemplate;
            treeNode.Header = contentPresenter;

            // If the node is not a leaf, recursively create TreeViewItems for its children.
            if (!node.isLeaf)
            {
                treeNode.Items.Add(CreateTreeNode(node.left));
                treeNode.Items.Add(CreateTreeNode(node.right));
            }

            return treeNode;
        }
    }
}
