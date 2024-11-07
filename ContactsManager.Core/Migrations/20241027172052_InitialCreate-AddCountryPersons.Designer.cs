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
    [Migration("20241027172052_InitialCreate-AddCountryPersons")]
    partial class InitialCreateAddCountryPersons
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
                            CountryId = new Guid("2185c5cd-5b72-444c-9b76-40eee4115338"),
                            CountryName = "Canada"
                        },
                        new
                        {
                            CountryId = new Guid("9238fbc2-fdd4-4aa4-a755-d7023c4be627"),
                            CountryName = "India"
                        },
                        new
                        {
                            CountryId = new Guid("3d362278-f75c-4acb-9845-a5066e4f22a9"),
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

                    b.ToTable("Persons", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}