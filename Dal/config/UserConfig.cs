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
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.HasIndex(u => u.Username).IsUnique();
            builder.Property(u => u.Username).IsRequired();

            builder.Property(u => u.Password).IsRequired();

            builder.Property(u => u.Role).IsRequired().HasConversion<string>();

            builder.HasMany(u => u.Houses)
                   .WithMany(h => h.Users);
        }
    }
}
