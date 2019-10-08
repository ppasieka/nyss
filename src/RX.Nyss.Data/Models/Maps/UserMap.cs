﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RX.Nyss.Data.Concepts;

namespace RX.Nyss.Data.Models.Maps
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasOne(u => u.ApplicationLanguage);
            builder.HasDiscriminator(u => u.Role)
                .HasValue<SupervisorUser>(Roles.Supervisor)
                .HasValue<DataManagerUser>(Roles.DataManager);
            
            builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
            builder.Property(u => u.IdentityUserId);
            builder.Property(u => u.Role).HasMaxLength(50).IsRequired();
            builder.Property(u => u.EmailAddress).HasMaxLength(100).IsRequired();
            builder.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.Property(u => u.AdditionalPhoneNumber).HasMaxLength(20);
            builder.Property(u => u.Organization).HasMaxLength(100);
            builder.Property(u => u.IsFirstLogin).IsRequired();
        }
    }
}
