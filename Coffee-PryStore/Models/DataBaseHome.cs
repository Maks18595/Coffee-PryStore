using Coffee_PryStore.Models;


using Microsoft.EntityFrameworkCore;

public class DataBaseHome : DbContext
{
    public DataBaseHome(DbContextOptions<DataBaseHome> options) : base(options)
    {
    }

    // DbSet для моделі HomeDataModel
    public DbSet<HomeDataModel> HomeDataModels { get; set; }

    // DbSet для моделі User
    public DbSet<User> Users { get; set; }

    public DbSet<Table> Table { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Table>()
            .Property(t => t.CofPrice)
            .HasColumnType("decimal(18,2)"); // Вказуємо, що це десятковий тип з 18 знаками, 2 з яких після коми
    }

}

