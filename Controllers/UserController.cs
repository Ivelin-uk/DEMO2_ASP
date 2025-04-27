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

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // Проверка дали имейлът вече съществува
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Имейлът вече е регистриран.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "User");
            }

            return View(user);
        }

         // 👉 GET: /User/Register
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Проверка за съществуващ потребител с въведения имейл и парола
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                // Ако потребителят не е намерен, добавяме грешка в ModelState
                ModelState.AddModelError(string.Empty, "Грешен имейл или парола. Моля, опитайте отново.");
                return View();
            }

            // Съхраняване на името и ролята на потребителя в сесията
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetString("UserRole", user.Role); // Съхраняване на ролята

            // Пренасочване в зависимост от ролята
            return RedirectToAction("index", "Home"); 
        }

        // 👉 GET: /User/Logout
        public IActionResult Logout()
        {
            // Изчистване на сесията
            HttpContext.Session.Clear();

            // Пренасочване към страницата за логване
            return RedirectToAction("Login", "User");
        }

        // 👉 Списък с потребители
        public async Task<IActionResult> Users()
        {
             // Проверка дали потребителят е логнат
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var users = await _context.Users.ToListAsync();

            // Проверка дали има данни
            if (users == null || !users.Any())
            {
                return Content("Няма налични потребители в базата данни.");
            }

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Users","User");
        }

        // 👉 GET: /User/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest();
            }

            // Проверка дали имейлът вече е записан за друг потребител
            if (await _context.Users.AnyAsync(u => u.Email == updatedUser.Email && u.Id != updatedUser.Id))
            {
                ModelState.AddModelError("Email", "Имейлът вече е регистриран за друг потребител.");
                return View(updatedUser);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Users");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(u => u.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View(updatedUser);
        }

        
        public IActionResult Profil()
        {
            // Проверка дали потребителят е логнат
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Извличане на информация от сесията
            var userName = HttpContext.Session.GetString("UserName");
            var userRole = HttpContext.Session.GetString("UserRole");

            // Създаване на ViewData за предаване на данни към изгледа
            ViewData["UserName"] = userName;
            ViewData["UserRole"] = userRole;

            return View();
        }

    }
}
