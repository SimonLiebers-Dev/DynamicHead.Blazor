using Microsoft.AspNetCore.Components;

namespace DynamicHead.Blazor.Services;

/// <summary>
/// Defines an interface for a service that manages dynamic &lt;head&gt; content
/// within a Blazor application.
/// </summary>
/// <remarks>
/// The <see cref="IDynamicHeadService"/> is responsible for tracking registered 
/// <see cref="RenderFragment"/> instances that represent &lt;head&gt; content, 
/// notifying subscribers when the collection changes, and providing access 
/// to all active fragments.
/// <para>
/// This interface is primarily intended for internal use by 
/// <see cref="Components.DynamicHeadOutlet"/> and 
/// <see cref="Components.DynamicHeadContent"/> components.
/// </para>
/// </remarks>
internal interface IDynamicHeadService
{
    /// <summary>
    /// Occurs when the set of registered head fragments changes, 
    /// such as when a fragment is added or removed.
    /// </summary>
    event Action? OnChanged;

    /// <summary>
    /// Registers a new <see cref="RenderFragment"/> representing a block 
    /// of head content to be rendered by a <see cref="Components.DynamicHeadOutlet"/>.
    /// </summary>
    /// <param name="fragment">The render fragment containing the head content.</param>
    /// <returns>
    /// A unique <see cref="Guid"/> identifier for the registered fragment, 
    /// which can later be used to unregister it.
    /// </returns>
    Guid Register(RenderFragment fragment);

    /// <summary>
    /// Removes a previously registered <see cref="RenderFragment"/> from the service.
    /// </summary>
    /// <param name="id">The unique identifier returned by <see cref="Register(RenderFragment)"/>.</param>
    void Unregister(Guid id);

    /// <summary>
    /// Retrieves all currently registered <see cref="RenderFragment"/> instances.
    /// </summary>
    /// <returns>
    /// A list of <see cref="RenderFragment"/> objects that represent the 
    /// active head content to be rendered.
    /// </returns>
    List<RenderFragment> GetRenderFragments();
}