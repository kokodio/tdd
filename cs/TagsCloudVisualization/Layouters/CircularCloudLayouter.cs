using System.Drawing;

namespace TagsCloudVisualization.Layouters;

public class CircularCloudLayouter : ICloudLayouter
{
    private Direction circleDirection = Direction.Up;
    private readonly List<Rectangle> rectangles = [];
    private readonly Queue<Vertex> placesQueue = [];

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (rectangleSize.Height < 0 || rectangleSize.Width < 0)
        {
            throw new ArgumentException("Размер меньше 0");
        }
        
        var rectangle = new Rectangle(FindNextLocation(rectangleSize), rectangleSize);

        rectangles.Add(rectangle);

        return rectangle;
    }

    public Rectangle[] GetAllRectangles() => rectangles.ToArray();

    public void ResetLayout()
    {
        rectangles.Clear();
        placesQueue.Clear();
        circleDirection = Direction.Up;
    }

    private Point FindNextLocation(Size rectangleSize)
    {
        var location = Point.Empty;
        var guessRectangle = new Rectangle(location, rectangleSize);
        
        if (rectangles.Count == 0)
        {
            UpdatePlaces(guessRectangle, circleDirection);
            return Point.Empty;
        }
        
        do
        {
            var vertex = placesQueue.Dequeue();
            location = GetLocationOnVertex(vertex, rectangleSize);
            guessRectangle = new Rectangle(location, rectangleSize);
        } while (rectangles.Any(rect => rect.IntersectsWith(guessRectangle)));

        UpdatePlaces(guessRectangle, circleDirection);

        circleDirection = NextDirection(circleDirection);

        return location;
    }

    private void UpdatePlaces(Rectangle rectangle, Direction direction)
    {
        foreach (var value in GetAllVertices(rectangle, direction))
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
            _ => throw new ArgumentException($"Неожиданное значение Direction: {vertex.Direction}")
        };
    }

    private IEnumerable<Vertex> GetAllVertices(Rectangle rectangle, Direction direction)
    {
        for (var i = 0; i < 4; i++)
        {
            direction = NextDirection(direction);
            yield return GetVertex(rectangle, direction);
        }
    }


    private static Vertex GetVertex(Rectangle rectangle, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Vertex(rectangle.Location, rectangle.Size.Width, Direction.Up);
            case Direction.Right:
            {
                var location = rectangle.Location with { X = rectangle.Location.X + rectangle.Size.Width };
                return new Vertex(location, rectangle.Size.Height, Direction.Right);
            }
            case Direction.Down:
            {
                var location = new Point(rectangle.Location.X + rectangle.Size.Width,
                    rectangle.Location.Y + rectangle.Size.Height);
                return new Vertex(location, rectangle.Size.Width, Direction.Down);
            }
            case Direction.Left:
            {
                var location = rectangle.Location with { Y = rectangle.Location.Y + rectangle.Size.Height };
                return new Vertex(location, rectangle.Size.Height, Direction.Left);
            }
            default:
                throw new ArgumentException($"Неожиданное значение Direction: {direction}");
        }
    }

    private static Direction NextDirection(Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => throw new ArgumentException($"Неожиданное значение Direction: {currentDirection}")
        };
    }

    private static Direction PreviousDirection(Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.Right => Direction.Up,
            Direction.Down => Direction.Right,
            Direction.Left => Direction.Down,
            Direction.Up => Direction.Left,
            _ => throw new ArgumentException($"Неожиданное значение Direction: {currentDirection}")
        };
    }
}