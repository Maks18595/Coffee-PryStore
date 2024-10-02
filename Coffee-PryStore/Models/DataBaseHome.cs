using Coffee_PryStore.Models;
using Microsoft.EntityFrameworkCore;

public class DataBaseHome : DbContext
{
    public DataBaseHome(DbContextOptions<DataBaseHome> options) : base(options)
    {
    }

    public DbSet<HomeDataModel> HomeDataModels { get; set; }
    public DbSet<User> Users { get; set; } // Додано для користувачів
}

