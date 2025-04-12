using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Data;         // ✅ това ти дава достъп до AppDbContext
using MyMvcApp.Models;       // ✅ това ти дава достъп до User
using Microsoft.EntityFrameworkCore;

namespace MyMvcApp.Controllers
{
    public class UserController : Controller
    {
        // ✅ ТУК ДЕФИНИРАМЕ _context
        private readonly AppDbContext _context;

        // ✅ ТУК ГО ПОЛУЧАВАМЕ чрез Dependency Injection
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // 👉 GET: /User/Register
        public IActionResult Register()
        {
            return View();
        }

        // 👉 POST: /User/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Users","User");
            }

            return View(user);
        }

        // 👉 Списък с потребители
       public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();

            // Проверка дали има данни
            if (users == null || !users.Any())
            {
                return Content("Няма налични потребители в базата данни.");
            }

            return View(users);
        }
    }
}
