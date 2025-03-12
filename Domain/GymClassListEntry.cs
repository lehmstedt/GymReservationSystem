namespace Domain;

public record GymClassListEntry(string Name, IReadOnlyCollection<string> Reservations)
{
    public readonly string Name = Name ?? throw new ArgumentNullException(nameof(Name));
    public readonly IReadOnlyCollection<string> Reservations = Reservations ?? throw new ArgumentNullException(nameof(Reservations));
}