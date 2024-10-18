using ContactsManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Web.Controllers;

[Route("api/[controller]")]
public class PersonController(IPersonService personService) : Controller
{
    [Route("index")]
    public IActionResult Index()
    {
        var persons = personService.GetAllPersons();
        return View(persons);
    }
}