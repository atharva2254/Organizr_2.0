using Microsoft.AspNetCore.Mvc;
using Organizr.Models;
using Microsoft.EntityFrameworkCore;
using Organizr.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity;
using Microsoft.AspNetCore.Identity;

namespace Organizr.Controllers
{
    public class UserController : Controller
    {

        private readonly AppDBContext _context;

        public UserController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if(userId == null)
            {
                return RedirectToAction("Login");
            }

            
            var user = _context.Users.Include(u => u.Tasks).FirstOrDefault(u => u.Id == userId);
            if(user == null)
            {
                return NotFound();
            }

            var taskCount = user.Tasks?.Count();
            var completedTask = user.Tasks?.Count(t => t.IsCompleted);
            var Pending = user.Tasks?.Count(t => !t.IsCompleted);

               
            ViewBag.TaskCount = taskCount;
            ViewBag.CompletedTask = completedTask;
            ViewBag.PendingTask = Pending;

            return View(user);

        }

        public IActionResult Login()
        {
            if(HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            var sessionUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);

            if (sessionUser == null)
            {
                TempData["Error"] = "User does not exists";
                return View();
            }
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(sessionUser, sessionUser.PasswordHash, user.PasswordHash);

            if (result == PasswordVerificationResult.Failed)
            {
                TempData["Error"] = "Invalid email or password";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", sessionUser.Id);
            TempData["Success"] = "Login Successful";
            return RedirectToAction("Index");
        }

        public IActionResult LogoutUser()
        {
            if(HttpContext.Session.GetString("UserId") != null)
            {
                HttpContext.Session.Clear();
            }
            TempData["Success"] = "Logout Successfull";

            return RedirectToAction("Index", "Home");
            //return Content("Logout Successful");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Registration Successful. Please Login";
                return RedirectToAction("Index");
            }

            return View(user);
        }
    }
}
