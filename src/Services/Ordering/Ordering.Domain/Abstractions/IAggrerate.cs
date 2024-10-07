namespace Ordering.Domain.Abstractions
{
    public interface IAggregate<T> : IAggrerate, IEntity<T>
    {

    }

    public interface IAggrerate : IEntity
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IDomainEvent[] ClearDomainEvents();
    }
}
