using System.Collections.ObjectModel;
using System.Drawing;

namespace TagsCloudVisualization.Layouters;

public class CircularCloudLayouter : ICloudLayouter
{
    private readonly List<Rectangle> rectangles = [];
    private double angle;
    private double radius;
    private const double AngleStep = 0.1;
    private const double RadiusStep = 0.1;

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (rectangleSize.Height < 0 || rectangleSize.Width < 0)
        {
            throw new ArgumentOutOfRangeException($"{rectangleSize} меньше 0");
        }

        var rectangle = new Rectangle(FindNextLocation(rectangleSize), rectangleSize);

        rectangles.Add(rectangle);

        return rectangle;
    }
    
    private Point FindNextLocation(Size rectangleSize)
    {
        var location = Point.Empty;;
        Rectangle guessRectangle;

        if (rectangles.Count == 0) return location;
        
        do
        {
            location = GetPointOnSpiral();
            guessRectangle = new Rectangle(location, rectangleSize);
            radius += RadiusStep;
            angle += AngleStep;
        } while (rectangles.Any(rect => rect.IntersectsWith(guessRectangle)));
        
        return location;
    }
    
    private Point GetPointOnSpiral()
    {
        var (sin, cos) = Math.SinCos(angle);
        var x = (int)(radius * cos);
        var y = (int)(radius * sin);
        return new Point(x, y);
    }
}