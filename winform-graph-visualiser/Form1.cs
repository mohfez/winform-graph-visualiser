using GraphViewer.TreeGraph;
using System;
using System.Windows.Forms;

namespace winform_graph_visualiser
{
    public partial class Form1 : Form
    {
        private TreeGraphNode root;

        public Form1()
        {
            InitializeComponent();
        }

        private TreeGraphNode ConstructBinaryTree(string[] values, int index = 0)
        {
            TreeGraphNode node = null;

            if (index < values.Length)
            {
                node = new TreeGraphNode(values[index]);
                node.left = ConstructBinaryTree(values, 2 * index + 1);
                node.right = ConstructBinaryTree(values, 2 * index + 2);
            }

            return node;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            root = ConstructBinaryTree(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", });
            TreeGraphBase.MakeTree(root, graphView1);
        }

        private void homeBtn_Click(object sender, EventArgs e)
        {
            if (root == null) return;
            TreeGraphBase.GoHome(root, graphView1);
        }
    }
}
