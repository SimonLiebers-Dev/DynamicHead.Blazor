using Bunit;
using DynamicHead.Blazor.Components;
using DynamicHead.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DynamicHead.Blazor.Tests.Components;

[TestFixture]
public class DynamicHeadContentTests
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
    public void RegistersContent_OnFirstRender()
    {
        // Arrange
        var fragment = (RenderFragment)(b => b.AddMarkupContent(0, "<title>Home</title>"));

        _mockService.Setup(s => s.Register(It.IsAny<RenderFragment>())).Returns(Guid.NewGuid());

        // Act
        var cut = _ctx.RenderComponent<DynamicHeadContent>(p => p.Add(x => x.ChildContent, fragment));

        // bUnit executes OnAfterRender automatically, so registration should happen
        _mockService.Verify(s => s.Register(It.Is<RenderFragment>(f => f == fragment)), Times.Once);
        _mockService.VerifyNoOtherCalls();
    }

    [Test]
    public void ReRegisters_WhenChildContentChanges()
    {
        // Arrange
        var fragA = (RenderFragment)(b => b.AddMarkupContent(0, "<meta name='a'/>"));
        var fragB = (RenderFragment)(b => b.AddMarkupContent(0, "<meta name='b'/>"));
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        _mockService.Setup(s => s.Register(fragA)).Returns(id1);
        _mockService.Setup(s => s.Register(fragB)).Returns(id2);
        _mockService.Setup(s => s.Unregister(id1));

        // Act
        var cut = _ctx.RenderComponent<DynamicHeadContent>(p => p.Add(x => x.ChildContent, fragA));

        // Simulate parameter change → triggers re-register
        cut.SetParametersAndRender(p => p.Add(x => x.ChildContent, fragB));

        // Assert
        _mockService.Verify(s => s.Register(fragA), Times.Once);
        _mockService.Verify(s => s.Unregister(id1), Times.Once);
        _mockService.Verify(s => s.Register(fragB), Times.Once);
        _mockService.VerifyNoOtherCalls();
    }

    [Test]
    public void DoesNotReRegister_WhenChildContentIsSameReference()
    {
        // Arrange
        var fragment = (RenderFragment)(b => b.AddMarkupContent(0, "<meta name='same'/>"));
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.Register(fragment)).Returns(id);

        // Act
        var cut = _ctx.RenderComponent<DynamicHeadContent>(p => p.Add(x => x.ChildContent, fragment));

        // Same reference → should not unregister or re-register
        cut.SetParametersAndRender(p => p.Add(x => x.ChildContent, fragment));

        // Assert
        _mockService.Verify(s => s.Register(fragment), Times.Once);
        _mockService.VerifyNoOtherCalls();
    }

    [Test]
    public void Unregisters_OnDispose()
    {
        // Arrange
        var fragment = (RenderFragment)(b => b.AddMarkupContent(0, "<title>Dispose Test</title>"));
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.Register(fragment)).Returns(id);
        _mockService.Setup(s => s.Unregister(id));

        // Act
        var cut = _ctx.RenderComponent<DynamicHeadContent>(p => p.Add(x => x.ChildContent, fragment));

        _ctx.DisposeComponents();

        // Assert
        _mockService.Verify(s => s.Register(fragment), Times.Once);
        _mockService.Verify(s => s.Unregister(id), Times.Once);
        _mockService.VerifyNoOtherCalls();
    }

    [Test]
    public void Dispose_DoesNotUnregister_WhenNoRegistration()
    {
        // Arrange: no ChildContent → should never register or unregister
        var cut = _ctx.RenderComponent<DynamicHeadContent>();

        // Act
        cut.Dispose();

        // Assert
        _mockService.VerifyNoOtherCalls();
    }
}