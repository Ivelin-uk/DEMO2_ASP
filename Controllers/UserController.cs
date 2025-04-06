using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    // Показва формата за регистрация
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}
