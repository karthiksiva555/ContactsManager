using ContactsManager.Application.DTOs;
using ContactsManager.Application.Interfaces;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Web.Controllers;

[Route("[controller]")]
public class PersonController(IPersonService personService, ICountryService countryService) : Controller
{
    [Route("index")]
    [Route("/")]
    public IActionResult Index(string? searchBy, string? searchString = null, string sortBy = nameof(PersonResponse.PersonName), SortOrder sortOrder = SortOrder.Asc)
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            {nameof(PersonResponse.PersonName), "Person Name"},
            {nameof(PersonResponse.EmailAddress), "Email Address"},
            {nameof(PersonResponse.DateOfBirth), "Date Of Birth"},
            {nameof(PersonResponse.Gender), "Gender"},
            {nameof(PersonResponse.Country), "Country Name"}
        };
        // retain searchBy and searchString across page loads
        ViewBag.SearchBy = searchBy ?? nameof(PersonResponse.PersonName);
        ViewBag.SearchString = searchString ?? string.Empty;
        
        // Retain sortBy and sortOrder across page loads
        ViewBag.SortBy = sortBy;
        ViewBag.SortOrder = sortOrder.ToString();
        
        // Get filtered persons
        var filteredPersons = personService.GetFilteredPersons(searchBy ?? nameof(PersonResponse.PersonName), searchString);
        
        // sort the filtered persons
        var sortedPersons = personService.GetSortedPersons(filteredPersons, sortBy, sortOrder);
        
        return View(sortedPersons);
    }

    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
        var countries = countryService.GetAllCountries();
        ViewBag.Countries = countries;
        return View();        
    }

    [Route("create")]
    [HttpPost]
    public IActionResult Create(PersonAddRequest personTAdd)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = countryService.GetAllCountries();
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View();
        }

        personService.AddPerson(personTAdd);
        return RedirectToAction("Index", "Person");
    }
}