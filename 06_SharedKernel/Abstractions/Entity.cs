namespace SharedKernel.Abstractions;
public abstract class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedOn { get; private set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }
    protected Entity(Guid id)
    {
        Id = id;
        CreatedOn = DateTime.UtcNow;
    }
}
