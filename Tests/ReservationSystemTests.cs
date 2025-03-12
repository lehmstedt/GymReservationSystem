using Domain;
using Infrastructure;

namespace Tests;

public class ReservationSystemTests
{
    private readonly ReservationSystem _reservationSystem;
    private readonly IReservationPort _reservationPort;
    private readonly GymClass _classOne = new GymClass(new ClassId(), "classOne");
    private readonly GymClass _classTwo = new GymClass(new ClassId(), "classTwo");
    private readonly GymClass _classWithReservations = new GymClass(new ClassId(), "classWithReservations");
    
    public ReservationSystemTests()
    {
        _classWithReservations.AddReservation("Johnny Depp");
        _classWithReservations.AddReservation("Salvadore Dali");
        _reservationPort = new InMemoryReservationPort([_classOne, _classTwo, _classWithReservations]);
        _reservationSystem = new ReservationSystem(_reservationPort);
    }
    [Fact]
    public async Task ReserveAClass_ShouldFail_WhenClassDoesNotExist()
    {
        var nonExistingClassId = new ClassId();
        const string unusedName = "";
        var result = await _reservationSystem.Reserve(nonExistingClassId, unusedName);
        
        Assert.Equal(ReservationResult.ClassNotFound, result);
    }

    [Fact]
    public async Task ReserveAClass_ShouldSucceedAndStoreTheReservationWhenClassExists()
    {
        var result = await _reservationSystem.Reserve(_classOne.ClassId, "John Doe");
        Assert.Equal(ReservationResult.Ok, result);
        
        var gymClass = await _reservationPort.GetClass(_classOne.ClassId);
        Assert.Equal("John Doe", gymClass?.Reservations[0]);
    }

    [Fact]
    public async Task ReserveAClass_ShouldAllowMultiplePersonsInSameClass()
    {
        await _reservationSystem.Reserve(_classOne.ClassId, "John Doe");
        await _reservationSystem.Reserve(_classOne.ClassId, "Jane Doe");
        
        var gymClass = await _reservationPort.GetClass(_classOne.ClassId);
        Assert.NotNull(gymClass);
        Assert.Contains("John Doe", gymClass.Reservations);
        Assert.Contains("Jane Doe", gymClass.Reservations);
    }

    [Fact]
    public async Task ReserveAClass_ShouldAllowAPersonToRegisterToTwoClasses()
    {
        await _reservationSystem.Reserve(_classOne.ClassId, "John Doe");
        await _reservationSystem.Reserve(_classTwo.ClassId, "John Doe");
        
        var classOne = await _reservationPort.GetClass(_classOne.ClassId);
        Assert.NotNull(classOne);
        Assert.Contains("John Doe", classOne.Reservations);
        Assert.Single(classOne.Reservations);
        
        var classTwo = await _reservationPort.GetClass(_classTwo.ClassId);
        Assert.NotNull(classTwo);
        Assert.Contains("John Doe", classTwo.Reservations);
        Assert.Single(classTwo.Reservations);
    }

    [Fact]
    public async Task ReserveAClass_ShouldAllowAPersonToReserveOneClassAndAnotherPersonAnotherOne()
    {
        await _reservationSystem.Reserve(_classOne.ClassId, "John Doe");
        await _reservationSystem.Reserve(_classTwo.ClassId, "Jane Doe");
        
        var classOne = await _reservationPort.GetClass(_classOne.ClassId);
        Assert.NotNull(classOne);
        Assert.Contains("John Doe", classOne.Reservations);
        Assert.Single(classOne.Reservations);
        
        var classTwo = await _reservationPort.GetClass(_classTwo.ClassId);
        Assert.NotNull(classTwo);
        Assert.Contains("Jane Doe", classTwo.Reservations);
        Assert.Single(classTwo.Reservations);
    }

    [Fact]
    public async Task ListClasses_ShouldReturnClassesWithTheirNames_WhenNoOneHasReserved()
    {
        var gymClasses = await _reservationSystem.ListClasses();
        
        Assert.Equal(3, gymClasses.Count);
        Assert.Contains("classOne", gymClasses.Select(c => c.Name));
        Assert.Contains("classTwo", gymClasses.Select(c => c.Name));
    }

    [Fact]
    public async Task ListClasses_ShouldReturnClassesWithReservations_WhenTheyAreMade()
    {
        var gymClasses = await _reservationSystem.ListClasses();
        
        var classWithReservations = gymClasses.FirstOrDefault(c => c.Name == "classWithReservations");
        Assert.NotNull(classWithReservations);
        Assert.Equal(2, classWithReservations.Reservations.Count);
        Assert.Contains("Johnny Depp", classWithReservations.Reservations);
        Assert.Contains("Salvadore Dali", classWithReservations.Reservations);
    }
}