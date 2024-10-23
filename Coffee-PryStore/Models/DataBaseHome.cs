using Coffee_PryStore.Models;


using Microsoft.EntityFrameworkCore;

public class DataBaseHome : DbContext
{
    public DataBaseHome(DbContextOptions<DataBaseHome> options) : base(options)
    {
    }


    public DbSet<HomeDataModel> HomeDataModels { get; set; }

 
    public DbSet<User> Users { get; set; }

    public DbSet<Table> Table { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Table>()
            .Property(t => t.CofPrice)
            .HasColumnType("decimal(18,2)"); 
    }
    public DbSet<CartItem> CartItems { get; set; }
}

