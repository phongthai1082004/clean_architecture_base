namespace Domain.Abstractions.Entities
{
    public abstract class GuidEntity : BaseEntity<Guid>
    {
        protected GuidEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
