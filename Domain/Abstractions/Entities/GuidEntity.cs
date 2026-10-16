namespace Domain.Abstractions.Entities
{
    public class GuidEntity : BaseEntity<Guid>
    {
        protected GuidEntity()
        {
        }
        protected GuidEntity(Guid id)
        {
            Id = Guid.CreateVersion7();
        }
    }
}
