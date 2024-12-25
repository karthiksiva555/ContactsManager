using ContactsManager.Application.DTOs;
using ContactsManager.Core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Web.Controllers;

[AllowAnonymous]
[Route("[controller]/[action]")]
public class AccountController(UserManager<User> userManager) : Controller
{   
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return View(registerUserDto);
        } 
        
        User user = new() { FullName = registerUserDto.FullName, Email = registerUserDto.Email, UserName = registerUserDto.UserName };
        var result = await userManager.CreateAsync(user, registerUserDto.Password);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Person");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(registerUserDto);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}