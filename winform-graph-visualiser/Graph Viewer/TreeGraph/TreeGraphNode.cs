namespace GraphViewer.TreeGraph
{
    public class TreeGraphNode : GraphNode
    {
        public TreeGraphNode left;
        public TreeGraphNode right;

        public TreeGraphNode(string text, TreeGraphNode left = null, TreeGraphNode right = null, int minWidth = 34, int minHeight = 34) : base(text, minWidth, minHeight)
        {
            this.left = left;
            this.right = right;
        }
    }
}
