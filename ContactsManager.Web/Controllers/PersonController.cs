using ContactsManager.Application.DTOs;
using ContactsManager.Application.Interfaces;
using ContactsManager.Core.Enums;
using ContactsManager.Web.Filters.Action;
using ContactsManager.Web.Filters.Authorization;
using ContactsManager.Web.Filters.Exception;
using ContactsManager.Web.Filters.Resource;
using ContactsManager.Web.Filters.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using SerilogTimings;

namespace ContactsManager.Web.Controllers;

[Route("[controller]")]
// Add the action filter at controller level
//[TypeFilter(typeof(LogAction))]
[TypeFilter(typeof(ResponseHeaderAddAction), Arguments = ["X-Controller-Name", "Person", 3])]
public class PersonController(IPersonService personService, ICountryService countryService, ILogger<PersonController> logger) : Controller
{
    private async Task<IEnumerable<SelectListItem>> GetCountriesForDropdownAsync()
    {
        var countries = await countryService.GetAllCountriesAsync();
        return countries.Select(country => new SelectListItem(country.CountryName, country.CountryId.ToString()));
    }
    
    [Route("index")]
    [Route("/")]
    [ServiceFilter(typeof(LogActionAsync))]
    [TypeFilter(typeof(ResponseHeaderAddAction), 
        Arguments = ["X-Action-Name", "Person.Index", 2])]
    // [TypeFilter(typeof(ValidateRequestHeaderAction), Arguments = ["x-app-name"])]
    [TypeFilter(typeof(AppNameResourceFilter))]
    [TypeFilter(typeof(SessionAuthorizationFilter))]
    [TypeFilter(typeof(XmlToJsonResultFilter))]
    [TypeFilter(typeof(HandleExceptionFilter))]
    //[TypeFilter(typeof(FormatResultFilter))]
    public async Task<IActionResult> IndexAsync(string? searchBy, string? searchString = null, string sortBy = nameof(PersonResponse.PersonName), SortOrder sortOrder = SortOrder.Asc)
    {
        logger.LogInformation("Calling the Index action method in PersonController");
        
        logger.LogDebug("Search By: {searchBy}, searchString: {searchString}, sortBy: {sortBy}", searchBy, searchString, sortBy);
        
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

        // IList<PersonResponse> filteredPersons;
        // using (Operation.Time("Fetching persons from database"))
        // {
        //     filteredPersons = await personService.GetFilteredPersonsAsync(searchBy ?? nameof(PersonResponse.PersonName), searchString);
        // }
        // Get filtered persons
        var filteredPersons = await personService.GetFilteredPersonsAsync(searchBy ?? nameof(PersonResponse.PersonName), searchString);
        
        // sort the filtered persons
        var sortedPersons = personService.GetSortedPersons(filteredPersons, sortBy, sortOrder);
        
        return View(sortedPersons);
    }

    [Route("create")]
    [HttpGet]
    public async Task<IActionResult> CreateAsync()
    {
        ViewBag.Countries = await GetCountriesForDropdownAsync();
        return View();        
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(PersonAddRequest personToAdd)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = await GetCountriesForDropdownAsync();
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View(personToAdd);
        }

        await personService.AddPersonAsync(personToAdd);
        return RedirectToAction("Index", "Person");
    }

    [Route("[action]/{personId:guid}")]
    [HttpGet]
    public async Task<IActionResult> EditAsync(Guid personId)
    {
        var person = await personService.GetPersonByIdAsync(personId);
        if (person == null)
        {
            return RedirectToAction("Index", "Person");
        }
        ViewBag.Countries = await GetCountriesForDropdownAsync();
        return View(person.ToPersonUpdateRequest());
    }

    // This route parameter is required even though the action method parameter is different
    [Route("[action]/{personId:guid}")]
    [HttpPost]
    public async Task<IActionResult> EditAsync(PersonUpdateRequest personUpdateRequest)
    {
        var personResponse = await personService.GetPersonByIdAsync(personUpdateRequest.PersonId);
        if (personResponse == null)
        {
            return RedirectToAction("Index", "Person");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Countries = await GetCountriesForDropdownAsync();
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View();
        }
        
        await personService.UpdatePersonAsync(personUpdateRequest);
        return RedirectToAction("Index", "Person");
    }

    [Route("[action]/{personId:guid}")]
    [HttpGet]
    public async Task<IActionResult> DeleteAsync(Guid personId)
    {
        var person = await personService.GetPersonByIdAsync(personId);
        if (person == null)
        {
            return RedirectToAction("Index", "Person");
        }

        return View(person);
    }

    [Route("[action]/{personId:guid}")]
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(PersonUpdateRequest personUpdateRequest)
    {
        var person = await personService.GetPersonByIdAsync(personUpdateRequest.PersonId);
        if (person == null)
        {
            return RedirectToAction("Index", "Person");
        }
        
        await personService.DeletePersonAsync(personUpdateRequest.PersonId);
        return RedirectToAction("Index", "Person");
    }
}