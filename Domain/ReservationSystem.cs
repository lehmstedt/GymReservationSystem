using System.Collections.ObjectModel;

namespace Domain;

public class ReservationSystem(IReservationPort reservationPort)
{
    public async Task<ReservationResult> Reserve(ClassId? classId, string name)
    {
        var foundClass = await reservationPort.GetClass(classId);
        if (foundClass == null) return ReservationResult.ClassNotFound;
        
        foundClass.AddReservation(name);
        await reservationPort.Save(foundClass);
        return ReservationResult.Ok;

    }

    public async Task<IReadOnlyCollection<GymClassListEntry>> ListClasses()
    {
        var foundClasses = await reservationPort.ListClasses();
        return foundClasses.Select(c => new GymClassListEntry(c.Name, c.Reservations)).ToList();
    }
}