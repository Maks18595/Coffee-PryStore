using Microsoft.EntityFrameworkCore;

namespace Coffee_PryStore.Models
{
    public class DataBaseHome(DbContextOptions<DataBaseHome> options) : DbContext(options)
    {
        //public DbSet<HomeDataModel> HomeDataModels { get; set; }


        public DbSet<User> Users { get; set; }

        public DbSet<Table> Table { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .Property(t => t.CofPrice)
                .HasColumnType("decimal(18,2)");
        }
        public DbSet<Basket> Basket { get; set; }


        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        
    }
}