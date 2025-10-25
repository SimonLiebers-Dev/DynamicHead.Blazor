using Microsoft.AspNetCore.Components;

namespace DynamicHead.Blazor.Tests.Services;

using DynamicHead.Blazor.Services;

[TestFixture]
public class DynamicHeadServiceTests
{
    [Test]
    public void Register_AddsFragment_AndReturnsValidGuid()
    {
        // Arrange
        var service = new DynamicHeadService();
        var fragment = new RenderFragment(_ => { });

        // Act
        var id = service.Register(fragment);
        var fragments = service.GetRenderFragments();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(fragments, Has.Count.EqualTo(1));
        });
        Assert.That(fragments.First(), Is.SameAs(fragment));
    }

    [Test]
    public void Unregister_RemovesFragment_WhenIdExists()
    {
        // Arrange
        var service = new DynamicHeadService();
        var fragment = new RenderFragment(_ => { });
        var id = service.Register(fragment);

        // Act
        service.Unregister(id);
        var fragments = service.GetRenderFragments();

        // Assert
        Assert.That(fragments, Is.Empty);
    }

    [Test]
    public void Unregister_DoesNothing_WhenIdDoesNotExist()
    {
        // Arrange
        var service = new DynamicHeadService();
        var fragment = new RenderFragment(_ => { });
        service.Register(fragment);

        // Act
        service.Unregister(Guid.NewGuid());
        var fragments = service.GetRenderFragments();

        // Assert
        Assert.That(fragments, Has.Count.EqualTo(1));
    }

    [Test]
    public void GetRenderFragments_ReturnsCopy_NotReference()
    {
        // Arrange
        var service = new DynamicHeadService();
        var fragment = new RenderFragment(_ => { });
        service.Register(fragment);

        // Act
        var fragmentsA = service.GetRenderFragments();
        var fragmentsB = service.GetRenderFragments();

        // Assert
        Assert.That(fragmentsA, Is.Not.SameAs(fragmentsB));
        Assert.That(fragmentsA, Has.Count.EqualTo(fragmentsB.Count));
    }

    [Test]
    public void OnChanged_Event_IsRaised_OnRegisterAndUnregister()
    {
        // Arrange
        var service = new DynamicHeadService();
        var fragment = new RenderFragment(_ => { });
        var changeCount = 0;
        service.OnChanged += () => changeCount++;

        // Act
        var id = service.Register(fragment);
        service.Unregister(id);

        // Assert
        Assert.That(changeCount, Is.EqualTo(2));
    }

    [Test]
    public void Register_NullFragment_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new DynamicHeadService();

        // Act & Assert
        Assert.That(() => service.Register(null!), Throws.TypeOf<ArgumentNullException>());
    }
}