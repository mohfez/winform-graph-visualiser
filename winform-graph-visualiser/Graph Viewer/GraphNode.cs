using System.Drawing;
using System.Windows.Forms;

namespace GraphViewer
{
    public class GraphNode
    {
        public string text;
        public (int x, int y) position;
        public int minWidth;
        public int minHeight;

        public (int gwidth, int gheight) graphicalSize;

        public GraphNode(string text, int minWidth = 34, int minHeight = 34)
        {
            this.text = text;
            this.position = (0, 0);
            this.minWidth = minWidth;
            this.minHeight = minHeight;

            Size stringSize = TextRenderer.MeasureText(text, Control.DefaultFont);
            graphicalSize = (minWidth + stringSize.Width, minHeight + stringSize.Height);
        }

        public GraphNode(string text, (int x, int y) position, int minWidth, int minHeight)
        {
            this.text = text;
            this.position = position;
            this.minWidth = minWidth;
            this.minHeight = minHeight;

            Size stringSize = TextRenderer.MeasureText(text, Control.DefaultFont);
            graphicalSize = (minWidth + stringSize.Width, minHeight + stringSize.Height);
        }
    }
}
