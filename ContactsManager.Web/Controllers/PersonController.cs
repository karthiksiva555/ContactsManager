using ContactsManager.Application.DTOs;
using ContactsManager.Application.Interfaces;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContactsManager.Web.Controllers;

[Route("[controller]")]
public class PersonController(IPersonService personService, ICountryService countryService) : Controller
{
    private IEnumerable<SelectListItem> GetCountriesForDropdown()
    {
        var countries = countryService.GetAllCountries();
        return countries.Select(country => new SelectListItem(country.CountryName, country.CountryId.ToString()));
    }
    
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
        ViewBag.Countries = GetCountriesForDropdown();
        return View();        
    }

    [Route("create")]
    [HttpPost]
    public IActionResult Create(PersonAddRequest personTAdd)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = GetCountriesForDropdown();
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View();
        }

        personService.AddPerson(personTAdd);
        return RedirectToAction("Index", "Person");
    }

    [Route("[action]/{personId:guid}")]
    [HttpGet]
    public IActionResult Edit(Guid personId)
    {
        var person = personService.GetPersonById(personId);
        if (person == null)
        {
            return RedirectToAction("Index", "Person");
        }
        ViewBag.Countries = GetCountriesForDropdown();
        return View(person.ToPersonUpdateRequest());
    }

    // This route parameter is required even though the action method parameter is different
    [Route("[action]/{personId:guid}")]
    [HttpPost]
    public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
    {
        var personResponse = personService.GetPersonById(personUpdateRequest.PersonId);
        if (personResponse == null)
        {
            return RedirectToAction("Index", "Person");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Countries = GetCountriesForDropdown();
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View();
        }
        
        personService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index", "Person");
    }
}