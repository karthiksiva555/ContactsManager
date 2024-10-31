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
        
        // seed the entities
        var countries = new List<Country>()
        {
            new (){CountryId = Guid.NewGuid(), CountryName = "Canada"},
            new (){CountryId = Guid.NewGuid(), CountryName = "India"},
            new (){CountryId = Guid.NewGuid(), CountryName = "United States of America"}
        };
        modelBuilder.Entity<Country>().HasData(countries);
    }
}