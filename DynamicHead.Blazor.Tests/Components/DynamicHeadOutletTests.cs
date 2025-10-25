using DynamicHead.Blazor.Components;
using DynamicHead.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DynamicHead.Blazor.Tests.Components;

[TestFixture]
public class DynamicHeadOutletTests
{
    private Bunit.TestContext _ctx = null!;
    private Mock<IDynamicHeadService> _mockService = null!;

    [SetUp]
    public void SetUp()
    {
        _ctx?.Dispose();
        _ctx = new Bunit.TestContext();

        _mockService = new Mock<IDynamicHeadService>(MockBehavior.Strict);
        _mockService.SetupAllProperties();

        _ctx.Services.AddSingleton(_mockService.Object);
    }

    [TearDown]
    public void TearDown() => _ctx.Dispose();

    [Test]
    public void SubscribesTo_OnChanged_AndInitializesContent()
    {
        // Arrange
        var fragments = new List<RenderFragment>
        {
            b => b.AddMarkupContent(0, "<title>Initial</title>"),
            b => b.AddMarkupContent(1, "<meta name='desc'/>")
        };

        _mockService.Setup(s => s.GetRenderFragments()).Returns(fragments);

        // Act
        var cut = _ctx.RenderComponent<DynamicHeadOutlet>();

        // Assert
        _mockService.VerifyAdd(s => s.OnChanged += It.IsAny<Action>(), Times.Once);
        _mockService.Verify(s => s.GetRenderFragments(), Times.Once);

        // The rendered HTML should contain both fragments
        var markup = cut.Markup;
        Assert.That(markup, Does.Contain("<title>Initial</title>"));
        Assert.That(markup, Does.Contain("<meta name='desc'/>"));
    }

    [Test]
    public void ReRenders_WhenOnChangedTriggered()
    {
        // Arrange
        var initial = new List<RenderFragment> { b => b.AddMarkupContent(0, "<title>Initial</title>") };
        var updated = new List<RenderFragment> { b => b.AddMarkupContent(0, "<title>Updated</title>") };

        var onChangedHandlers = new List<Action>();

        _mockService.SetupAdd(s => s.OnChanged += It.IsAny<Action>())
            .Callback<Action>(a => onChangedHandlers.Add(a));

        _mockService.SetupRemove(s => s.OnChanged -= It.IsAny<Action>())
            .Callback<Action>(a => onChangedHandlers.Remove(a));

        // First render → initial content
        _mockService.SetupSequence(s => s.GetRenderFragments())
            .Returns(initial)
            .Returns(updated);

        var cut = _ctx.RenderComponent<DynamicHeadOutlet>();

        // Act — simulate the service firing OnChanged
        onChangedHandlers.ForEach(a => a.Invoke());

        // Assert
        var markup = cut.Markup;
        Assert.That(markup, Does.Contain("<title>Updated</title>"));
        _mockService.Verify(s => s.GetRenderFragments(), Times.Exactly(2));
    }

    [Test]
    public void Unsubscribes_OnDispose()
    {
        // Arrange
        var handler = default(Action);
        _mockService.SetupAdd(s => s.OnChanged += It.IsAny<Action>())
            .Callback<Action>(a => handler = a);
        _mockService.SetupRemove(s => s.OnChanged -= It.IsAny<Action>())
            .Callback<Action>(a => handler = null);

        _mockService.Setup(s => s.GetRenderFragments()).Returns([]);

        var cut = _ctx.RenderComponent<DynamicHeadOutlet>();

        // Act
        _ctx.DisposeComponents();

        // Assert
        _mockService.VerifyRemove(s => s.OnChanged -= It.IsAny<Action>(), Times.Once);
        Assert.That(handler, Is.Null, "Should have unsubscribed event handler");
    }

    [Test]
    public void RendersEmpty_WhenNoFragmentsExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetRenderFragments()).Returns(new List<RenderFragment>());

        // Act
        var cut = _ctx.RenderComponent<DynamicHeadOutlet>();

        // Assert
        Assert.That(cut.Markup.Trim(), Is.Empty.Or.EqualTo(""));
        _mockService.Verify(s => s.GetRenderFragments(), Times.Once);
    }
}