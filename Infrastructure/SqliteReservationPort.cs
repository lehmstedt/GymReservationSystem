using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public class SqliteReservationPort(IOptions<DatabaseOptions> options) : DbContext, IReservationPort
{
    public DbSet<GymClassEntity> Classes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(options.Value.ConnectionString);
    }

    public async Task<GymClass?> GetClass(ClassId classId)
    {
        var entity = await Classes.FirstOrDefaultAsync(x => x.Id.Equals(classId.ToGuid()));
        if (entity == null)
        {
            return null;
        }
        return new GymClass(entity.Id, entity.Name, entity.Reservations);
    }

    public async Task Save(GymClass foundClass)
    {
        var entity = await Classes.FirstOrDefaultAsync(x => x.Id.Equals(foundClass.Id.ToGuid()));
        if (entity == null)
        {
            Add(new GymClassEntity()
            {
                Id = foundClass.Id.ToGuid(),
                Name = foundClass.Name,
                Reservations = foundClass.Reservations.ToList()
            });
            await SaveChangesAsync();
            return;
        }
        entity.Reservations = foundClass.Reservations.ToList();
        await SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<GymClass>> ListClasses()
    {
        var list = await Classes.ToListAsync();
        if (list.Count == 0)
        {
            Add(new GymClassEntity()
            {
                Id = Guid.NewGuid(),
                Name = "WOD",
                Reservations = []
            });
            Add(new GymClassEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Spinning",
                Reservations = []
            });
            await SaveChangesAsync();
        }
        return list.Select(l => new GymClass(l.Id, l.Name, l.Reservations)).ToList();
    }
}