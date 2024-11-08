using System.Drawing;
using FluentAssertions;
using TagsCloudVisualization.Layouters;
using TagsCloudVisualization.Renderers;

namespace TagsCloudVisualizationTest;

public class CircularCloudLayouterTests
{
    private CircularCloudLayouter layout;

    [SetUp]
    public void SetUp()
    {
        layout = new CircularCloudLayouter();
    }
    
    [Test]
    public void PutNextRectangle_ReturnRectangleEmpty_OnSizeEmpty()
    {
        layout.PutNextRectangle(Size.Empty).Should().BeEquivalentTo(Rectangle.Empty);
    }
    
    [TestCase(0,0)]
    [TestCase(5,5)]
    [TestCase(0,5)]
    public void PutNextRectangle_ReturnLocationZero_OnFirstInput(int height, int width)
    {
        var layout = new CircularCloudLayouter();
        var size = new Size(width, height);
        var expected = new Rectangle(0,0,width,height);
        
        layout.PutNextRectangle(size).Should().BeEquivalentTo(expected);
    }
    
    [TestCase(0,0)]
    [TestCase(5,5)]
    [TestCase(0,5)]
    public void PutNextRectangle_ReturnLocationZero_AfterResetLayout(int height, int width)
    {
        var layout = new CircularCloudLayouter();
        var size = new Size(width, height);
        var expected = new Rectangle(0,0,width,height);

        for (var i = 0; i < 10; i++)
        {
            layout.PutNextRectangle(size);
        }
        
        layout.ResetLayout();
        
        layout.PutNextRectangle(size).Should().BeEquivalentTo(expected);
    }
    
    [TestCase(-1,0)]
    [TestCase(-1,5)]
    [TestCase(5,-1)]
    [TestCase(0,-1)]
    public void PutNextRectangle_ThrowArgumentException_WhenSizeIsNegative(int height, int width)
    {
        var size = new Size(width, height);
        
        
        var act = () => layout.PutNextRectangle(size);
        
        act.Should().Throw<ArgumentException>("Размер меньше 0");
    }

    [Test]
    public void GetAllRectangles_BeEmpty_OnInitialize()
    {
        layout.GetAllRectangles().Should().BeEmpty();
    }
    
    [Test]
    public void GetAllRectangles_BeEmpty_AfterResetLayout()
    {
        layout.PutNextRectangle(Size.Empty);
        layout.GetAllRectangles().Should().HaveCount(1);
        
        layout.ResetLayout();
        layout.GetAllRectangles().Should().BeEmpty();
    }
    
    [Test]
    public void GetAllRectangles_NotBeEmpty_WhenValidPutNextRectangle()
    {
        layout.PutNextRectangle(Size.Empty);
        
        layout.GetAllRectangles().Should().NotBeEmpty();
    }
    
    [TestCase(0)]
    [TestCase(5)]
    [TestCase(10)]
    public void GetAllRectangles_CountEqualPutNextRectangleInvokes_WhenValidPutNextRectangle(int count)
    {
        var layout = new CircularCloudLayouter();

        for (var i = 0; i < count; i++)
        {
            layout.PutNextRectangle(SizeRegistry.DefaultSize);
        }

        layout.GetAllRectangles().Should().HaveCount(count);
    }
    
    [TestCase(0,0)]
    [TestCase(0,10)]
    [TestCase(10,50)]
    public void IntersectsWith_BeFalse_OnRandomData(int sizeFrom, int sizeTo)
    {
        var random = new Random();
        
        for (var i = 0; i < 1000; i++)
        {
            var randomSize = new Size(random.Next(sizeFrom, sizeTo), random.Next(sizeFrom, sizeTo));
            layout.PutNextRectangle(randomSize);
        }
        
        var rect = layout.GetAllRectangles();
        
        var isIntersect = false;
        for (var i = 0; i < rect.Length; i++)
        {
            for (var j = i + 1; j < rect.Length; j++)
            {
                if (!rect[i].IntersectsWith(rect[j])) continue;
                isIntersect = true;
                break;
            }
            if (isIntersect) break;
        }

        isIntersect.Should().BeFalse();
    }
    
    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.FailCount > 0)
        {
            var render = new AutoAdjustRenderer();
            render.AddRectangles(layout.GetAllRectangles());
            render.SaveImage("Failed.png");
        }
    }
}