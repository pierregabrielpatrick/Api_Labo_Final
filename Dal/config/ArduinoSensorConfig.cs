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

          

            // CORRECTION : Relation avec House
            // Il faut d'abord ajouter une propriété HouseId dans ArduinoSensor
            builder.Property(s => s.HouseId)
                   .IsRequired(); // Clé étrangère obligatoire

            builder.HasOne(s => s.HouseOwner)           // Navigation property vers House
                   .WithMany(h => h.ArduinoSensors)     // House a plusieurs ArduinoSensors
                   .HasForeignKey(s => s.HouseId)       // Utilise HouseId comme clé étrangère
                   .OnDelete(DeleteBehavior.Cascade);

            // Index sur la clé étrangère (recommandé pour les performances)
            builder.HasIndex(s => s.HouseId);

            // Index sur Category si nécessaire
            builder.HasIndex(s => s.Category);
        }
    }

}

