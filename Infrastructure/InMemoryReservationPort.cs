using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Domain;

namespace Infrastructure;

public class InMemoryReservationPort (IReadOnlyCollection<GymClass> classes): IReservationPort
{
    private Dictionary<ClassId, GymClass> _classes = classes.ToDictionary(c => c.ClassId);
    public Task<GymClass?> GetClass(ClassId? askedClassId)
    {
        return Task.FromResult(classes.FirstOrDefault(x => x.ClassId.Equals(askedClassId)));
    }

    public Task Save(GymClass classToSave)
    {
        _classes[classToSave.ClassId] = classToSave;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<GymClass>> ListClasses()
    {
        var list = _classes.Select(c => c.Value).ToList();
        var collection = new ReadOnlyCollection<GymClass>(list);
        return Task.FromResult<IReadOnlyCollection<GymClass>>(collection);
    }
}