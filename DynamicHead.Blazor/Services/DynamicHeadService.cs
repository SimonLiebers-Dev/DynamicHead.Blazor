using Microsoft.AspNetCore.Components;

namespace DynamicHead.Blazor.Services;

/// <summary>
/// Default implementation of the <see cref="IDynamicHeadService"/> interface.
/// </summary>
/// <remarks>
/// The <see cref="DynamicHeadService"/> maintains a thread-safe collection of 
/// <see cref="RenderFragment"/> instances that represent dynamic &lt;head&gt; content.  
/// It provides registration, removal, and change-notification functionality 
/// to components such as <see cref="Components.DynamicHeadContent"/> and 
/// <see cref="Components.DynamicHeadOutlet"/>.
/// <para>
/// Whenever a head fragment is added or removed, the <see cref="OnChanged"/> event 
/// is triggered to notify all active outlets that they should re-render.
/// </para>
/// </remarks>
internal class DynamicHeadService : IDynamicHeadService
{
    private readonly Dictionary<Guid, RenderFragment> _entries = new();
    private readonly Lock _lock = new();

    public event Action? OnChanged;

    /// <inheritdoc cref="IDynamicHeadService"/>
    public Guid Register(RenderFragment fragment)
    {
        ArgumentNullException.ThrowIfNull(fragment);

        var id = Guid.NewGuid();
        lock (_lock)
        {
            _entries[id] = fragment;
        }

        NotifyChanged();
        return id;
    }

    /// <inheritdoc cref="IDynamicHeadService"/>
    public void Unregister(Guid id)
    {
        lock (_lock)
        {
            if (_entries.Remove(id))
                NotifyChanged();
        }
    }

    /// <inheritdoc cref="IDynamicHeadService"/>
    public List<RenderFragment> GetRenderFragments()
    {
        lock (_lock)
        {
            return _entries.Values.ToList();
        }
    }

    /// <summary>
    /// Invokes the <see cref="OnChanged"/> event to notify subscribers that 
    /// the collection of registered head fragments has changed.
    /// </summary>
    private void NotifyChanged() => OnChanged?.Invoke();
}