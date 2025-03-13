using Domain;
using Infrastructure;

namespace Configurator;

public static class ReservationSystemConfigurator
{
    private static ReservationSystem? _reservationSystem;
    public static ReservationSystem ConfigureReservationSystem()
    {
        if (_reservationSystem == null)
        {
            var port = new SqliteReservationPort();
            _reservationSystem = new ReservationSystem(port);
        }
        return _reservationSystem;
    } 
}