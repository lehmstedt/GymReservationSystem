using Domain;
using Infrastructure;

namespace Configurator;

public static class ReservationSystemConfigurator
{
    public static ReservationSystem ConfigureReservationSystem()
    {
        var wod = new GymClass(new ClassId(), "WOD");
        wod.AddReservation("Jean Didier");
        var spinning = new GymClass(new ClassId(), "Spinning");
        var port = new InMemoryReservationPort([wod, spinning]);

        return new ReservationSystem(port);
    } 
}