using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WifiWarriorAPI.Configurations.Entities;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    private const string UserRoleId = "27bf0aec-b07a-4539-9ca7-8f981f5fabf4";
    private const string AdministratorRoleId = "4d35ec33-2474-451f-b844-f5abeb9b8291";
    private const string ModeratorRoleId = "0a42e085-3a4f-4c59-a553-f91de96be068";

    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = UserRoleId,
                ConcurrencyStamp = "01d67b26-4971-4535-a139-06f75c9fedf9",
                Name = "User",
                NormalizedName = "USER"
            }, new IdentityRole
            {
                Id = AdministratorRoleId,
                ConcurrencyStamp = "ab53af3d-217b-471d-9803-0093b8aae081",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            }, new IdentityRole
            {
                Id = ModeratorRoleId,
                ConcurrencyStamp = "137c7d86-306e-4e52-9ff7-2ed8bd0c241c",
                Name = "Moderator",
                NormalizedName = "MODERATOR"
            });
    }
}