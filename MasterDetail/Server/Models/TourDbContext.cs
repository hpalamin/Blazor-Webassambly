using MasterDetail.Shared;
using Microsoft.EntityFrameworkCore;

namespace MasterDetail.Server.Models
{
    public class TourDbContext : DbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options) {}

        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<Spot> Spots { get; set; } = default!;
        public DbSet<BookingEntry> BookingEntries { get; set; } = default!;
    }
}
