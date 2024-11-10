using System.Collections.ObjectModel;
using System.Drawing;

namespace TagsCloudVisualization.Layouters;

public class CircularCloudLayouter : ICloudLayouter
{
    private Direction currentDirection = Direction.Up;
    private readonly List<Rectangle> rectangles = [];
    private readonly Queue<Vertex> placesQueue = [];
    
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
        var location = Point.Empty;
        var guessRectangle = new Rectangle(location, rectangleSize);
        
        if (rectangles.Count != 0)
        {
            do
            {
                var vertex = placesQueue.Dequeue();
                location = GetLocationOnVertex(vertex, rectangleSize);
                guessRectangle = new Rectangle(location, rectangleSize);
            } while (rectangles.Any(rect => rect.IntersectsWith(guessRectangle)));
        }

        UpdatePlaces(guessRectangle, currentDirection);

        currentDirection = currentDirection.NextDirection();

        return location;
    }

    private void UpdatePlaces(Rectangle rectangle, Direction direction)
    {
        foreach (var value in Vertex.GetRectangleVertices(rectangle, direction))
        {
            placesQueue.Enqueue(value);
        }
    }

    private Point GetLocationOnVertex(Vertex vertex, Size rectangleSize)
    {
        return vertex.Direction switch
        {
            Direction.Up => new Point(vertex.Location.X, vertex.Location.Y - rectangleSize.Height),
            Direction.Right => vertex.Location,
            Direction.Down => new Point(vertex.Location.X - rectangleSize.Width, vertex.Location.Y),
            Direction.Left => new Point(vertex.Location.X - rectangleSize.Width,
                vertex.Location.Y - rectangleSize.Height),
            _ => throw new ArgumentOutOfRangeException($"Неожиданное значение Direction: {vertex.Direction}")
        };
    }
}