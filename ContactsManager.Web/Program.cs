using ContactsManager.Application.Interfaces;
using ContactsManager.Application.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//DI Container
builder.Services.AddSingleton<ICountryService, CountryService>();
builder.Services.AddSingleton<IPersonService, PersonService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.MapControllers();
app.Run();