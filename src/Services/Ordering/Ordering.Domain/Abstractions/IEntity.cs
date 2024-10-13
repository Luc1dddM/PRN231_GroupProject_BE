namespace Ordering.Domain.Abstractions
{
    public interface IEntity<T> : IEntity
    {
        public T EntityId { get; set; }
    }


    public interface IEntity
    {
        //common properties for audit
        int Id { get; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
