namespace TagsCloudVisualizationTest;

public static class EnumerableExtensions
{
    public static double StdDev(this IEnumerable<double> values)
    {
        var mean = 0.0;
        var sum = 0.0;
        var stdDev = 0.0;
        var count = 0;
        
        foreach (var value in values)
        {
            var delta = value - mean;
            count++;
            mean += delta / count;
            sum += delta * (value - mean);
        }

        if (count > 1)
        {
            stdDev = Math.Sqrt(sum / (count - 1));
        }

        return stdDev;
    }
}