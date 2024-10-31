using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Core.Entities;

public class ContactsDbContext(DbContextOptions<ContactsDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries { get; set; }

    public DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>()
            .ToTable("Persons")
            .HasOne(p => p.Country)
            .WithMany()
            .HasForeignKey(p => p.CountryId);

        modelBuilder.Entity<Person>()
            .Property(p => p.DateOfBirth)
            .HasConversion(v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
        
        // seed the entities
        var countries = new List<Country>()
        {
            new (){CountryId = Guid.Parse("b85f5c5d-234b-49b5-b6f3-afe0b0b6196e"), CountryName = "Canada"},
            new (){CountryId = Guid.Parse("a59c1f88-4c90-43e4-9b29-4bb0a104dd13"), CountryName = "India"},
            new (){CountryId = Guid.Parse("3eb16cc8-0528-41eb-a2e4-77b06de04e49"), CountryName = "United States of America"}
        };
        modelBuilder.Entity<Country>().HasData(countries);
    }
}