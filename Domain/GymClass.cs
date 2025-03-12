namespace Domain;

public class GymClass(ClassId classId, string name = "")
{
    private readonly List<string> _reservations = [];

    public ClassId ClassId { get; } = classId;
    public IReadOnlyList<string> Reservations => _reservations;
    public string Name { get; } = name;

    public void AddReservation(string name)
    {
        _reservations.Add(name);
    }
}