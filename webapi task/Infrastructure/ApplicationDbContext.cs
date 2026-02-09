using Microsoft.EntityFrameworkCore;

namespace webapi_task.Infrastructure;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Value> Values { get; set; }
    public DbSet<Result> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Value>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.Property(v => v.FileName).IsRequired().HasMaxLength(256);

            entity.Property(v => v.Date).IsRequired();

            entity.Property(v => v.ExecutionTime).IsRequired();

            entity.Property(v => v.MeasurementValue).IsRequired();

            entity.HasIndex(v => v.FileName);   // Поиск по имени файла
            entity.HasIndex(v => v.Date);       // Сортировка по дате

        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.Property(v => v.FileName).IsRequired().HasMaxLength(256);

            entity.HasIndex(r => r.FileName);
        });
    }
}

