using MediatR;

namespace Ordering.Domain.Abstractions
{
    //domain event represents something that has occurred in the domain that
    //other parts of the system might be interested in reacting to.
    public interface IDomainEvent : INotification
    {
        Guid EventId => Guid.NewGuid();
        public DateTime OccurredOn => DateTime.Now;
        public string EventType => GetType().AssemblyQualifiedName;
    }
}
