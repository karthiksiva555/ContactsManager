﻿// <auto-generated />
using System;
using ContactsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ContactsManager.Core.Migrations
{
    [DbContext(typeof(ContactsDbContext))]
    [Migration("20241031201539_ConvertCountrySeedToUseConstantIds")]
    partial class ConvertCountrySeedToUseConstantIds
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ContactsManager.Core.Entities.Country", b =>
                {
                    b.Property<Guid>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CountryName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("CountryId");

                    b.ToTable("Countries", (string)null);

                    b.HasData(
                        new
                        {
                            CountryId = new Guid("b85f5c5d-234b-49b5-b6f3-afe0b0b6196e"),
                            CountryName = "Canada"
                        },
                        new
                        {
                            CountryId = new Guid("a59c1f88-4c90-43e4-9b29-4bb0a104dd13"),
                            CountryName = "India"
                        },
                        new
                        {
                            CountryId = new Guid("3eb16cc8-0528-41eb-a2e4-77b06de04e49"),
                            CountryName = "United States of America"
                        });
                });

            modelBuilder.Entity("ContactsManager.Core.Entities.Person", b =>
                {
                    b.Property<Guid>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CountryId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int?>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("PersonName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("PersonId");

                    b.HasIndex("CountryId");

                    b.ToTable("Persons", (string)null);
                });

            modelBuilder.Entity("ContactsManager.Core.Entities.Person", b =>
                {
                    b.HasOne("ContactsManager.Core.Entities.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.Navigation("Country");
                });
#pragma warning restore 612, 618
        }
    }
}
