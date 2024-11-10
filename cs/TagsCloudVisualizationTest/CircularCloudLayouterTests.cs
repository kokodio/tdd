using System.Drawing;
using FluentAssertions;
using TagsCloudVisualization.Layouters;
using TagsCloudVisualization.Renderers;

namespace TagsCloudVisualizationTest;

public class CircularCloudLayouterTests
{
    private CircularCloudLayouter layout;

    private List<Rectangle> rectangles;

    [SetUp]
    public void SetUp()
    {
        layout = new CircularCloudLayouter();
        rectangles = [];
    }

    [Test]
    public void PutNextRectangle_ReturnRectangleEmpty_OnSizeEmpty()
    {
        layout.PutNextRectangle(Size.Empty).Should().BeEquivalentTo(Rectangle.Empty);
    }

    [TestCase(0, 0)]
    [TestCase(5, 5)]
    [TestCase(0, 5)]
    public void PutNextRectangle_ReturnLocationZero_OnFirstInput(int height, int width)
    {
        var size = new Size(width, height);
        var expected = new Rectangle(0, 0, width, height);
        var result = layout.PutNextRectangle(size);

        rectangles.Add(result);

        result.Should().BeEquivalentTo(expected);
    }

    [TestCase(-1, 0)]
    [TestCase(-1, 5)]
    [TestCase(5, -1)]
    [TestCase(0, -1)]
    public void PutNextRectangle_ThrowArgumentException_WhenSizeIsNegative(int height, int width)
    {
        var size = new Size(width, height);

        var act = () => layout.PutNextRectangle(size);
        
        act.Should().Throw<ArgumentOutOfRangeException>($"{size} меньше 0");
    }

    [Test]
    public void IntersectsWith_BeFalse()
    {
        layout = LayoutRegistry.DefaultLayout();
        rectangles = LayoutRegistry.DefaultLayoutRectangles;

        var isIntersect = rectangles
            .SelectMany((rect, i) => rectangles.Skip(i + 1).Select(r => (rect, r)))
            .Any(pair => pair.rect.IntersectsWith(pair.r));

        isIntersect.Should().BeFalse();
    }

    [Test]
    [Repeat(5)]
    public void Layout_BeCircle_WhenDeviationIsLessThanTolerance()
    {
        layout = LayoutRegistry.DefaultLayout();
        rectangles = LayoutRegistry.DefaultLayoutRectangles;

        var totalArea = 0d;
        var weightedXSum = 0d;
        var weightedYSum = 0d;
        var maxRadius = 0d;

        foreach (var rect in rectangles)
        {
            maxRadius = Math.Max(maxRadius, GetDistanceToCenter(rect));
            
            var centerX = rect.X + rect.Width / 2.0;
            var centerY = rect.Y + rect.Height / 2.0;
            var area = rect.Width * rect.Height;
            
            weightedXSum += centerX * area;
            weightedYSum += centerY * area;
            
            totalArea += area;
        }

        var centerXOfMass = weightedXSum / totalArea;
        var centerYOfMass = weightedYSum / totalArea;
        var deviationLenght = Math.Sqrt(Math.Pow(centerXOfMass, 2) + Math.Pow(centerYOfMass, 2));

        var toleranceValue = maxRadius * 0.15;

        deviationLenght.Should().BeLessThan(toleranceValue);
    }
    
    [Test]
    [Repeat(5)]
    public void Layout_BeDense()
    {
        layout = LayoutRegistry.DefaultLayout();
        rectangles = LayoutRegistry.DefaultLayoutRectangles;
        
        var outliersCount = (int)(rectangles.Count * 0.02);
        
        var totalArea = rectangles
            .Select(x => x.Height * x.Width)
            .Sum();
        
        var maxRadius = rectangles
            .Select(GetDistanceToCenter)
            .OrderDescending()
            .Skip(outliersCount)
            .First();

        var circleArea = Math.PI * Math.Pow(maxRadius, 2);
        
        var dense = totalArea / circleArea;

        dense.Should().BeGreaterThan(0.5);
    }

    [Test]
    [Repeat(5)]
    public void Layout_HasUniformDensityInFourDirections()
    {
        layout = LayoutRegistry.DefaultLayout();
        rectangles = LayoutRegistry.DefaultLayoutRectangles;
        
        const int centerX = 0;
        const int centerY = 0;

        var quadrants = new Dictionary<string, int>
        {
            { "TopRight", 0 },
            { "TopLeft", 0 },
            { "BottomRight", 0 },
            { "BottomLeft", 0 }
        };
        
        foreach (var rect in rectangles)
        {
            var rectCenterX = rect.X + rect.Width / 2.0;
            var rectCenterY = rect.Y + rect.Height / 2.0;
            var area = rect.Width * rect.Height;

            switch (rectCenterX)
            {
                case >= centerX when rectCenterY >= centerY:
                    quadrants["TopRight"] += area;
                    break;
                case < centerX when rectCenterY >= centerY:
                    quadrants["TopLeft"] += area;
                    break;
                case >= centerX when rectCenterY < centerY:
                    quadrants["BottomRight"] += area;
                    break;
                default:
                    quadrants["BottomLeft"] += area;
                    break;
            }
        }

        var totalArea = quadrants.Values.Sum();
        
        foreach (var quadrant in quadrants)
        {
            var percent = quadrant.Value / (double)totalArea;

            percent.Should().BeGreaterThan(0.15, $"Плотность {quadrant.Key} ниже среднего");
            percent.Should().BeLessThan(0.35, $"Плотность {quadrant.Key} выше среднего");
        }
    }

    [Test]
    [Repeat(5)]
    public void Outliers_NotBeFarAway()
    {
        layout = LayoutRegistry.DefaultLayout();
        rectangles = LayoutRegistry.DefaultLayoutRectangles;
        
        var outliersCount = (int)(rectangles.Count * 0.02);
        var tenPercent = (int)(rectangles.Count * 0.1);

        var sortedDistance = rectangles
            .Select(GetDistanceToCenter)
            .OrderDescending()
            .ToArray();

        var outliersDistance = sortedDistance
            .Take(outliersCount)
            .Average();
        
        var tenPercentDistance = sortedDistance
            .Take(tenPercent)
            .Average();
        
        var averageDistance = sortedDistance
            .Skip(outliersCount)
            .Average();
        
        var averageCoefficient = outliersDistance / averageDistance;
        var tenPercentCoefficient = outliersDistance / tenPercentDistance;

        averageCoefficient.Should().BeLessThan(1.9);
        tenPercentCoefficient.Should().BeLessThan(1.2);
    }

    private static double GetDistanceToCenter(Rectangle x)
    {
        return Math.Sqrt(Math.Pow(x.X + x.Width / 2.0, 2) + Math.Pow(x.Y + x.Height / 2.0, 2));
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.FailCount <= 0) return;

        var render = new ContentFittingRenderer();
        var name = $"{TestContext.CurrentContext.Test.Name}.png";

        foreach (var rectangle in rectangles)
        {
            render.AddRectangle(rectangle);
        }

        render.SaveImage(name);
        Console.WriteLine($"Tag cloud visualization saved to file {name}");
    }
}