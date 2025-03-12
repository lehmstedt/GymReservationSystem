namespace Domain;

public class GymClass(ClassId id, string name = "")
{
    private readonly List<string> _reservations = [];

    public ClassId Id { get; } = id;
    public IReadOnlyList<string> Reservations => _reservations;
    public string Name { get; } = name;

    public void AddReservation(string name)
    {
        _reservations.Add(name);
    }
}