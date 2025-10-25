using DynamicHead.Blazor.Extensions;
using DynamicHead.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicHead.Blazor.Tests.Extensions;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddDynamicHead_RegistersService_AsScopedByDefault()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDynamicHead();
        var provider = services.BuildServiceProvider();

        // Assert
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDynamicHeadService));
        Assert.Multiple(() =>
        {
            Assert.That(descriptor, Is.Not.Null, "Service descriptor should exist.");
            Assert.That(descriptor!.Lifetime, Is.EqualTo(ServiceLifetime.Scoped), "Default lifetime should be Scoped.");
        });

        var instance1 = provider.GetRequiredService<IDynamicHeadService>();
        var instance2 = provider.CreateScope().ServiceProvider.GetRequiredService<IDynamicHeadService>();

        // Each scope should get a different instance
        Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    [Test]
    public void AddDynamicHead_RegistersService_WithCustomLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDynamicHead(ServiceLifetime.Singleton);
        var provider = services.BuildServiceProvider();

        // Assert
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDynamicHeadService));
        Assert.Multiple(() =>
        {
            Assert.That(descriptor, Is.Not.Null);
            Assert.That(descriptor!.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
        });

        // Verify Singleton behavior: same instance everywhere
        var instance1 = provider.GetRequiredService<IDynamicHeadService>();
        var instance2 = provider.GetRequiredService<IDynamicHeadService>();
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void AddDynamicHead_CanBeChained()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDynamicHead();

        // Assert
        Assert.That(result, Is.SameAs(services), "Should return the same IServiceCollection for chaining.");
    }

    [Test]
    public void AddDynamicHead_DoesNotThrow_WhenCalledMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act + Assert: Should not throw when adding multiple times
        Assert.DoesNotThrow(() =>
        {
            services.AddDynamicHead();
            services.AddDynamicHead();
        });

        // Should have two separate service descriptors (per call)
        var count = services.Count(d => d.ServiceType == typeof(IDynamicHeadService));
        Assert.That(count, Is.EqualTo(2));
    }
}