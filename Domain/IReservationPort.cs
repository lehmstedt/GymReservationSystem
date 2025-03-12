namespace Domain;

public interface IReservationPort
{
    Task<GymClass?> GetClass(ClassId? classId);
    Task Save(GymClass foundClass);
    Task<IReadOnlyCollection<GymClass>> ListClasses();
}