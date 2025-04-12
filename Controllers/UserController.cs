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
                return RedirectToAction("Users", "User");
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
    }
}
