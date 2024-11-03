using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ContactsManager.Core.Entities;

public class ContactsDbContext(DbContextOptions<ContactsDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries { get; set; }

    public DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Country>().ToTable("countries");
        modelBuilder.Entity<Person>()
            .ToTable("persons")
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

    public List<Person> FunctionGetAllPersons()
    {
        return Persons.FromSqlRaw("SELECT person_id, person_name, date_of_birth, gender, email_address,country_id FROM GetAllPersons()").ToList();
    }

    public int InsertPerson(Person person)
    {
        var dateOfBirth = person.DateOfBirth.HasValue ? DateTime.SpecifyKind(person.DateOfBirth.Value, DateTimeKind.Utc) : default;
        return Database.ExecuteSqlInterpolated(
            $"CALL insert_person({person.PersonId}, {person.PersonName}, {dateOfBirth}, {person.Gender}, {person.EmailAddress}, {person.CountryId})");
    }
}