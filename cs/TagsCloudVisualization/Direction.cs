namespace TagsCloudVisualization;

public static class DirectionExtension
{
    public static Direction NextDirection(this Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => throw new ArgumentOutOfRangeException($"Неожиданное значение Direction: {currentDirection}")
        };
    }

    public static Direction PreviousDirection(this Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.Right => Direction.Up,
            Direction.Down => Direction.Right,
            Direction.Left => Direction.Down,
            Direction.Up => Direction.Left,
            _ => throw new ArgumentOutOfRangeException($"Неожиданное значение Direction: {currentDirection}")
        };
    }
}

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
}
