using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.config
{
    public class ArduinoSensorConfig : IEntityTypeConfiguration<ArduinoSensor>
    {
        public void Configure(EntityTypeBuilder<ArduinoSensor> builder)
        {
            // Clé primaire
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            // Colonnes obligatoires
            builder.Property(s => s.DefinitionOfEvent)
                   .HasMaxLength(200); // Exemple : tu peux mettre une limite
                                       // si nécessaire

            builder.Property(s => s.DigitalValue);

            builder.Property(s => s.AnanlogicValue)
                   .IsRequired();

            builder.Property(s => s.LastUpdated);

            builder.Property(s => s.Category);

            // Relation avec House (si House est bien  entité parent)
            builder.HasOne<House>()                // Chaque capteur appartient à une maison
                   .WithMany(h => h.ArduinoSensors) // Il faudra ajouter `ICollection<ArduinoSensor>` dans House
                   .HasForeignKey(s => s.HouseOwner)
                   .OnDelete(DeleteBehavior.Cascade);

            // Exemple d’index (si utile, comme pour Name dans House)
            builder.HasIndex(s => s.Category);
        }
    }

}

