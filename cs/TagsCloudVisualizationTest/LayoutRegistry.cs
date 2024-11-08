using System.Drawing;
using TagsCloudVisualization.Layouters;

namespace TagsCloudVisualizationTest;

public static class LayoutRegistry
{
    public static CircularCloudLayouter DefaultLayout()
    {
        var layouter = new CircularCloudLayouter();
        
        for (var i = 0; i < 13; i++)
        {
            layouter.PutNextRectangle(SizeRegistry.DefaultSize);
        }
        
        return layouter;
    }
}