namespace Domain;

public record ClassId()
{
    private readonly Guid _guid = Guid.NewGuid();

    public virtual bool Equals(ClassId? other)
    {
        return _guid.Equals(other?._guid);
    }
    
    public override string ToString() => _guid.ToString();

    public override int GetHashCode()
    {
        return _guid.GetHashCode();
    }
}