namespace Domain;

public record ClassId()
{
    private readonly Guid _guid = Guid.NewGuid();

    public ClassId(Guid guid) : this()
    {
        _guid = guid;
    }

    public virtual bool Equals(ClassId? other)
    {
        return _guid.Equals(other?._guid);
    }

    public Guid ToGuid() => _guid;

    public override int GetHashCode()
    {
        return _guid.GetHashCode();
    }
}