using Microsoft.EntityFrameworkCore;
using WavePowerApp.Models;

namespace WavePowerApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<WaveCalculation> WaveCalculations => Set<WaveCalculation>();
    }
}