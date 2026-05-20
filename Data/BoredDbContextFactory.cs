using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BoredWeb.Data;

/// <summary>
/// Used by "dotnet ef" CLI tools at design time (migrations, scaffolding).
/// Not used at runtime — configure the DbContext in Program.cs for that.
/// </summary>
public class BoredDbContextFactory : IDesignTimeDbContextFactory<BoredDbContext>
{
    public BoredDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BoredDbContext>();

        optionsBuilder.UseNpgsql(
            "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=Bored;Pooling=true;CommandTimeout=120;Timeout=30"
        );

        return new BoredDbContext(optionsBuilder.Options);
    }
}
