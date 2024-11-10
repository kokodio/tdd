using System.Drawing;
using TagsCloudVisualization.Layouters;

namespace TagsCloudVisualizationTest;

public static class LayoutRegistry
{
    private const int DefaultLayoutRectangleCount = 1000;

    public static List<Rectangle> DefaultLayoutRectangles = [];
    
    private static readonly Random Random = new();
    public static CircularCloudLayouter DefaultLayout(int sizeFrom = 10, int sizeTo = 100)
    {
        var layouter = new CircularCloudLayouter();
        DefaultLayoutRectangles = new List<Rectangle>(DefaultLayoutRectangleCount);
        
        for (var i = 0; i < DefaultLayoutRectangleCount; i++)
        {
            DefaultLayoutRectangles.Add(layouter.PutNextRectangle( new Size(Random.Next(sizeFrom, sizeTo), Random.Next(sizeFrom, sizeTo))));
        }
        
        return layouter;
    }
}