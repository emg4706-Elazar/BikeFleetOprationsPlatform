using Processor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Processor.Data;

public class BikeFleetDbContext : DbContext
{
    public BikeFleetDbContext(
        DbContextOptions<BikeFleetDbContext> options)
        : base(options)
    {
    }


    public DbSet<StationEntity> Stations =>
        Set<StationEntity>();

    public DbSet<VehicleTypeEntity> VehicleTypes =>
        Set<VehicleTypeEntity>();
}
