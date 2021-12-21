using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;

namespace Zonorai.Tenants.Application.Common;

public interface IEventStore
{
    public IReadOnlyCollection<INotification> Events { get; }
    public Task AddEvent<T>(T @event) where T : INotification;
}