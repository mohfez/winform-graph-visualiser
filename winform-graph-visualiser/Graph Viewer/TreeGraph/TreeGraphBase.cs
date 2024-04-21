using System;

namespace GraphViewer.TreeGraph
{
    public static class TreeGraphBase
    {
        /// <summary>
        /// Using the root and it's children, it will construct a tree-shape graph for you.
        /// </summary>
        public static void MakeTree(TreeGraphNode root, GraphView view)
        {
            view.Clear();
            CalculateNodePositions(root, null, 0, 0, root.graphicalSize.gheight, view);
            GoHome(root, view);
        }

        /// <summary>
        /// Go back home.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="view"></param>
        public static void GoHome(TreeGraphNode root, GraphView view)
        {
            view.GoTo(root.position);
        }

        /// <summary>
        /// Just calculates the needed node positions to make the tree shape.
        /// </summary>
        private static int CalculateNodePositions(TreeGraphNode curr, TreeGraphNode parent, int depth, int offsetX, int verticalSpacing, GraphView view)
        {
            if (curr == null) return 0;
            if (parent != null) view.AddEdge(parent, curr);
            else view.AddNode(curr);

            int diameter = (int)Math.Max(curr.graphicalSize.gwidth * 1.5, curr.graphicalSize.gheight * 1.5);
            int leftSubtreeWidth = CalculateNodePositions(curr.left, curr, depth + 1, offsetX, verticalSpacing + curr.graphicalSize.gheight / 2, view);
            int rightSubtreeWidth = CalculateNodePositions(curr.right, curr, depth + 1, offsetX + leftSubtreeWidth + diameter, verticalSpacing + curr.graphicalSize.gheight / 2, view);

            curr.position = (offsetX + leftSubtreeWidth, depth * verticalSpacing);
            return leftSubtreeWidth + rightSubtreeWidth + diameter;
        }
    }
}