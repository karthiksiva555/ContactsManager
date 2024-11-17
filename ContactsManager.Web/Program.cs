using ContactsManager.Application.Interfaces;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//DI Container
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddInfrastructure();

builder.Services.AddDbContext<ContactsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.MapControllers();
app.Run();