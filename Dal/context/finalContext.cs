using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.context
{
    public class FinalContext : DbContext
    {
        public FinalContext(DbContextOptions options) : base(options)
        {
        }        

        public DbSet<User> Users => Set<User>();
        public DbSet<House> Houses => Set<House>();

        public DbSet<ArduinoSensor> ArduinoSensors => Set<ArduinoSensor>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinalContext).Assembly);
        }
    }
}
