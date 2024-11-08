using System.Drawing;
using TagsCloudVisualization.Layouters;
using TagsCloudVisualization.Renderers;

namespace TagsCloudVisualization;

public class TagCloudApp
{
    private IRenderer render;
    private ICloudLayouter layout;

    public void Run()
    {
        render = new AutoAdjustRenderer();
        layout = new CircularCloudLayouter();

        var rnd = new Random();

        for (var i = 0; i < 100; i++)
        {
            var randomSize = new Size(rnd.Next(10, 100), rnd.Next(10, 100));
            var rect = layout.PutNextRectangle(randomSize);
            render.AddRectangle(rect);
        }

        render.SaveImage("render.png");
    }
}