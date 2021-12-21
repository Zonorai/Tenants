using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MediatR;
using Zonorai.Tenants.Application.Common;

namespace Zonorai.Tenants.Infrastructure.Services;

internal class EventStore : IEventStore
{
    private List<INotification> _events { get; } = new();

    public Task AddEvent<T>(T @event) where T : INotification
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public IReadOnlyCollection<INotification> Events => new ReadOnlyCollection<INotification>(_events);

    internal void Clear()
    {
        _events.Clear();
    }
}