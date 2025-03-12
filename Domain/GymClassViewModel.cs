namespace Domain;

public record GymClassViewModel(GymClass GymClass)
{
    public ClassId Id { get; } = GymClass.Id;
    public readonly string Name = GymClass.Name;
    public readonly IReadOnlyCollection<string> Reservations = GymClass.Reservations;
}