namespace Infrastructure;

public class GymClassEntity
{
    public Guid Id { get; set; }
    public required string Name { get; init; }
    public required List<string> Reservations { get; set; }
}