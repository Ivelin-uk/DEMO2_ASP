using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    // Показва формата за регистрация
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            return View("Success", user);
        }

        return View(user);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}
