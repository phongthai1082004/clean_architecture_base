namespace Domain.Abstractions.Entities
{
    public class GuidEntitySoftDelete : BaseEntitySoftDelete<Guid>
    {
        protected GuidEntitySoftDelete()
        {
            Id = Guid.NewGuid();
        }
    }
}
