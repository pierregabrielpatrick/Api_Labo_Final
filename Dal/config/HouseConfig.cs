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
    public class HouseConfig : IEntityTypeConfiguration<House>
    {
        public void Configure(EntityTypeBuilder<House> builder)
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id).ValueGeneratedOnAdd();

            builder.Property(h => h.Name).IsRequired();

            builder.HasMany(h => h.Users)
                   .WithMany(u => u.Houses);

            builder.HasIndex(h => h.Name).IsUnique();
        }
    }
}
