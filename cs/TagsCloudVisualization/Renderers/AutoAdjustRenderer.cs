using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualization.Renderers;

public class AutoAdjustRenderer(Pen? pen = null) : IRenderer
{
    private int left;
    private int right;
    private int top;
    private int bottom;
    private readonly List<Rectangle> rectangles = [];
    private readonly Pen pen = pen ?? new Pen(Color.Black);

    public void AddRectangle(Rectangle rectangle)
    {
        left = Math.Min(left, rectangle.Left);
        right = Math.Max(right, rectangle.Right);
        top = Math.Min(top, rectangle.Top);
        bottom = Math.Max(bottom, rectangle.Bottom);

        rectangles.Add(rectangle);
    }

    public void SaveImage(string filename)
    {
        var width = Math.Abs(left) + Math.Abs(right) + 1;
        var height = Math.Abs(top) + Math.Abs(bottom) + 1;

        var bitmap = new Bitmap(width, height);
        var graphics = Graphics.FromImage(bitmap);

        DrawRectangles(graphics);

        bitmap.Save(filename, ImageFormat.Png);
    }

    public void Clear()
    {
        rectangles.Clear();
        left = 0;
        right = 0;
        top = 0;
        bottom = 0;
    }
    
    public void AddRectangles(Rectangle[] inputRectangles)
    {
        foreach (var rect in inputRectangles)
        {
            AddRectangle(rect);
        }
    }
    
    private void DrawRectangles(Graphics graphics)
    {
        foreach (var rectangle in rectangles)
        {
            var x = rectangle.X + Math.Abs(left);
            var y = rectangle.Y + Math.Abs(top);
            graphics.DrawRectangle(pen, x, y, rectangle.Width, rectangle.Height);
        }
    }
}