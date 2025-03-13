namespace Infrastructure;

public class GymClassEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<string> Reservations { get; set; }
}