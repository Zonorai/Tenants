using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;

namespace Zonorai.Tenants.Application.Common;

public interface IEventStore
{
    public Task AddEvent<T>(T @event) where T : INotification;
    public IReadOnlyCollection<INotification> Events { get; }
}