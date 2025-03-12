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
        var result = await _reservationSystem.Reserve(_classOne.Id, "John Doe");
        Assert.Equal(ReservationResult.Ok, result);
        
        var gymClass = await _reservationPort.GetClass(_classOne.Id);
        Assert.Equal("John Doe", gymClass?.Reservations[0]);
    }

    [Fact]
    public async Task ReserveAClass_ShouldAllowMultiplePersonsInSameClass()
    {
        await _reservationSystem.Reserve(_classOne.Id, "John Doe");
        await _reservationSystem.Reserve(_classOne.Id, "Jane Doe");
        
        var gymClass = await _reservationPort.GetClass(_classOne.Id);
        Assert.NotNull(gymClass);
        Assert.Contains("John Doe", gymClass.Reservations);
        Assert.Contains("Jane Doe", gymClass.Reservations);
    }

    [Fact]
    public async Task ReserveAClass_ShouldAllowAPersonToRegisterToTwoClasses()
    {
        await _reservationSystem.Reserve(_classOne.Id, "John Doe");
        await _reservationSystem.Reserve(_classTwo.Id, "John Doe");
        
        var classOne = await _reservationPort.GetClass(_classOne.Id);
        Assert.NotNull(classOne);
        Assert.Contains("John Doe", classOne.Reservations);
        Assert.Single(classOne.Reservations);
        
        var classTwo = await _reservationPort.GetClass(_classTwo.Id);
        Assert.NotNull(classTwo);
        Assert.Contains("John Doe", classTwo.Reservations);
        Assert.Single(classTwo.Reservations);
    }

    [Fact]
    public async Task ReserveAClass_ShouldAllowAPersonToReserveOneClassAndAnotherPersonAnotherOne()
    {
        await _reservationSystem.Reserve(_classOne.Id, "John Doe");
        await _reservationSystem.Reserve(_classTwo.Id, "Jane Doe");
        
        var classOne = await _reservationPort.GetClass(_classOne.Id);
        Assert.NotNull(classOne);
        Assert.Contains("John Doe", classOne.Reservations);
        Assert.Single(classOne.Reservations);
        
        var classTwo = await _reservationPort.GetClass(_classTwo.Id);
        Assert.NotNull(classTwo);
        Assert.Contains("Jane Doe", classTwo.Reservations);
        Assert.Single(classTwo.Reservations);
    }

    [Fact]
    public async Task ListClasses_ShouldReturnClassesWithTheirNames_WhenNoOneHasReserved()
    {
        var gymClasses = await _reservationSystem.ListClasses();
        
        Assert.Equal(3, gymClasses.Length);
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

    [Fact]
    public async Task ListClasses_ShouldReturnClassesWithClassesIds()
    {
        var gymClasses = await _reservationSystem.ListClasses();
        
        var classOne = gymClasses.FirstOrDefault(c => c.Name == "classOne");
        Assert.Equal(_classOne.Id, classOne?.Id);
    }

    [Fact]
    public async Task ListClasses_ShouldReturnClassesWithDifferentIds()
    {
        var gymClasses = await _reservationSystem.ListClasses();
        Assert.NotEqual(gymClasses[0].Id, gymClasses[1].Id);
    }

    [Fact]
    public async Task ListReservations_ShouldReturnAllReservationsWhenTheyAreMade()
    {
        var reservations = await _reservationSystem.ListReservations(_classWithReservations.Id);
        
        Assert.Contains("Johnny Depp", reservations);
        Assert.Contains("Salvadore Dali", reservations);
    }
}