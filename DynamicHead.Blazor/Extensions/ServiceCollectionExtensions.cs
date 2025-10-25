using System.ComponentModel;
using DynamicHead.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicHead.Blazor.Extensions;

/// <summary>
/// Provides extension methods for registering the <see cref="DynamicHeadService"/> 
/// in a Blazor application's dependency injection container.
/// </summary>
/// <remarks>
/// This class is part of the <c>DynamicHead.Blazor</c> library, which enables dynamic
/// management of &lt;head&gt; content through components like 
/// <see cref="Components.DynamicHeadOutlet"/> and <see cref="Components.DynamicHeadContent"/>.
/// <para>
/// Call <see cref="AddDynamicHead(IServiceCollection, ServiceLifetime)"/> in your 
/// <c>Program.cs</c> to enable the service.
/// </para>
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Always)]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="DynamicHeadService"/> to the specified 
    /// <see cref="IServiceCollection"/> for use within a Blazor application.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the <see cref="DynamicHeadService"/> will be added.
    /// </param>
    /// <param name="lifetime">
    /// The lifetime of the service within the dependency injection container.
    /// The default is <see cref="ServiceLifetime.Scoped"/>, which is recommended 
    /// for both Blazor Server and Blazor WebAssembly applications.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.
    /// </returns>
    /// <example>
    /// <code lang="csharp">
    /// using DynamicHead.Blazor;
    /// 
    /// builder.Services.AddDynamicHead();
    /// </code>
    /// </example>
    /// <remarks>
    /// Use this method once at application startup.  
    /// The <see cref="DynamicHeadService"/> allows components to dynamically register, update, 
    /// and remove &lt;head&gt; elements at runtime.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Always)]
    public static IServiceCollection AddDynamicHead(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var descriptor = new ServiceDescriptor(typeof(IDynamicHeadService), typeof(DynamicHeadService), lifetime);
        services.Add(descriptor);
        return services;
    }
}