using Microsoft.AspNetCore.Mvc;
using Organizr.Models;
using Microsoft.EntityFrameworkCore;
using Organizr.Data;

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

            var taskCount = user.Tasks.Count();
            var completedTask = user.Tasks.Count(t => t.IsCompleted);
            var Pending = user.Tasks.Count(t => !t.IsCompleted);

               
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
            var sessionUser = _context.Users.Where(u=> u.Email == user.Email && u.PasswordHash == user.PasswordHash).FirstOrDefault();
            if(sessionUser != null)
            {
                HttpContext.Session.SetInt32("UserId", sessionUser.Id);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Invalid email or username";
            }
            return View();
        }

        public IActionResult LogoutUser()
        {
            if(HttpContext.Session.GetString("UserId") != null)
            {
                HttpContext.Session.Clear();
            }
            TempData["LogoutMessage"] = "Logout Successfull";

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
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Registration Successful. Please Login";
                return RedirectToAction("Index");
            }

            return View(user);
        }
    }
}
