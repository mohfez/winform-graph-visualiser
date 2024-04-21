using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GraphViewer
{
    public class GraphView : Panel
    {
        public bool renderOffScreen = false;
        public bool autoInvalidate = true;

        private List<GraphNode> nodes;
        private List<(GraphNode, GraphNode)> edges;

        private Point lastMousePosition;
        private Point contentOffset;
        private Bitmap offScreenBuffer;
        private float zoom = 1f;

        public GraphView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            AutoScroll = false;

            MouseDown += GraphContent_MouseDown;
            MouseMove += GraphContent_MouseMove;
            MouseUp += GraphContent_MouseUp;
            MouseWheel += GraphContent_MouseWheel;

            nodes = new List<GraphNode>();
            edges = new List<(GraphNode, GraphNode)>();
        }

        /// <summary>
        /// Add a node to the graph.
        /// </summary>
        public void AddNode(GraphNode node)
        {
            nodes.Add(node);
            if (autoInvalidate) Invalidate();
        }

        /// <summary>
        /// Get all nodes in the graph.
        /// </summary>
        public List<GraphNode> GetNodes() => nodes;

        /// <summary>
        /// Remove an edge from the graph.
        /// </summary>
        public void RemoveNode(GraphNode node)
        {
            nodes.Remove(node);
            if (autoInvalidate) Invalidate();
        }

        /// <summary>
        /// Add an edge to the graph.
        /// </summary>
        public void AddEdge(GraphNode from, GraphNode to)
        {
            if (!nodes.Contains(from)) nodes.Add(from);
            if (!nodes.Contains(to)) nodes.Add(to);

            edges.Add((from, to));
            if (autoInvalidate) Invalidate();
        }

        /// <summary>
        /// Get the list of edges.
        /// </summary>
        public List<(GraphNode from, GraphNode to)> GetEdges() => edges;

        /// <summary>
        /// Remove an edge from the graph.
        /// </summary>
        public void RemoveEdge(GraphNode from, GraphNode to)
        {
            nodes.Remove(from);
            nodes.Remove(to);

            edges.Remove((from, to));
            if (autoInvalidate) Invalidate();
        }

        /// <summary>
        /// Clear the graph for new nodes/edges.
        /// </summary>
        public void Clear()
        {
            edges.Clear();
            nodes.Clear();
            if (autoInvalidate) Invalidate();
        }

        /// <summary>
        /// Move the view to the given coords.
        /// </summary>
        public void GoTo((int x, int y) pos)
        {
            contentOffset = new Point((Width / 2) - pos.x, (Height / 2) - pos.y);
            Invalidate();
        }

/*
*                              PAINT CONTROLS
* ------------------------------------------------------------------------
*/

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (offScreenBuffer == null) offScreenBuffer = new Bitmap(Width, Height);

            using (Graphics offScreenGraphics = Graphics.FromImage(offScreenBuffer))
            {
                offScreenGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                offScreenGraphics.Clear(Color.White);
                offScreenGraphics.ScaleTransform(zoom, zoom);

                Rectangle visibleRect = new Rectangle(0, 0, (int)((Width) / zoom), (int)((Height) / zoom));

                foreach (GraphNode node in nodes)
                {
                    if (!renderOffScreen && !IsNodeVisible(node, visibleRect)) continue;
                    DrawNode(node, offScreenGraphics);
                }

                foreach ((GraphNode from, GraphNode to) in edges)
                {
                    if (!renderOffScreen && !IsNodeVisible(from, visibleRect)) continue;
                    DrawArrow(from, to, offScreenGraphics);
                }
            }

            pe.Graphics.DrawImage(offScreenBuffer, 0, 0);
        }

        private bool IsNodeVisible(GraphNode node, Rectangle visibleRect)
        {
            Rectangle nodeRect = new Rectangle(node.position.x + contentOffset.X, node.position.y + contentOffset.Y, node.graphicalSize.gwidth, node.graphicalSize.gheight);
            return visibleRect.IntersectsWith(nodeRect);
        }

        private void DrawNode(GraphNode node, Graphics graphics)
        {
            int posX = node.position.x + contentOffset.X;
            int posY = node.position.y + contentOffset.Y;

            graphics.FillEllipse(Brushes.White, posX, posY, node.graphicalSize.gwidth, node.graphicalSize.gheight);
            graphics.DrawEllipse(Pens.Black, posX, posY, node.graphicalSize.gwidth, node.graphicalSize.gheight);

            graphics.DrawString(node.text, DefaultFont, Brushes.Black, posX + (node.minWidth / 2) + 2, posY + (node.minHeight / 2));
        }

        private void DrawArrow(GraphNode from, GraphNode to, Graphics graphics)
        {
            int arrowSize = 6;
            GraphicsPath arrowEnd = new GraphicsPath();
            arrowEnd.AddLine(arrowSize * -1, 0, arrowSize, 0);
            arrowEnd.AddLine(arrowSize * -1, 0, 0, arrowSize);
            arrowEnd.AddLine(0, arrowSize, arrowSize, 0);

            Pen linePen = new Pen(Color.Black, 1)
            {
                CustomEndCap = new CustomLineCap(null, arrowEnd, LineCap.ArrowAnchor)
            };

            (int x, int y) fromCenter = (from.position.x + contentOffset.X + (from.graphicalSize.gwidth / 2), from.position.y + contentOffset.Y + (from.graphicalSize.gheight / 2));
            (int x, int y) toCenter = (to.position.x + contentOffset.X + (to.graphicalSize.gwidth / 2), to.position.y + contentOffset.Y + (to.graphicalSize.gheight / 2));

            double angle = Math.Atan2(toCenter.y - fromCenter.y, toCenter.x - fromCenter.x);
            Point startPoint = CalculateEllipseIntersection(fromCenter, from.graphicalSize.gwidth / 2, from.graphicalSize.gheight / 2, angle);
            Point endPoint = CalculateEllipseIntersection(toCenter, (to.graphicalSize.gwidth + arrowSize * 2) / 2, (to.graphicalSize.gheight + arrowSize * 2) / 2, angle + Math.PI);

            graphics.DrawLine(linePen, startPoint, endPoint);
        }

        private Point CalculateEllipseIntersection((int x, int y) center, double radiusX, double radiusY, double angle)
        {
            double dx = radiusX * Math.Cos(angle);
            double dy = radiusY * Math.Sin(angle);

            return new Point((int)(center.x + dx), (int)(center.y + dy));
        }

/*
*                              MOUSE CONTROLS
* ------------------------------------------------------------------------
*/

        private void GraphContent_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;
        }

        private void GraphContent_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float deltaX = e.X - lastMousePosition.X;
                float deltaY = e.Y - lastMousePosition.Y;
                float sens = Math.Max(1 / zoom, 1f);

                contentOffset.X += (int)(deltaX * sens);
                contentOffset.Y += (int)(deltaY * sens);

                Invalidate();
                lastMousePosition = e.Location;
            }
        }

        private void GraphContent_MouseUp(object sender, MouseEventArgs e)
        {
            lastMousePosition = Point.Empty;
        }

        private void GraphContent_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && zoom < 2) zoom += 0.1f;
            else if (e.Delta <= 0 && zoom > 0.1f) zoom -= 0.1f;

            Invalidate();
        }
    }
}