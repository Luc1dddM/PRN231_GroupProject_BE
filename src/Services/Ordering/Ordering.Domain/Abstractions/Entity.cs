namespace Ordering.Domain.Abstractions
{
    //this class access modifier is abstract in order to use as a base entity
    public abstract class Entity<T> : IEntity<T>
    {
        public int Id { get; set; }
        public T EntityId { get; set ; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
        
    }
}
