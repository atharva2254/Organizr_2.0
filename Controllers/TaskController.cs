using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizr.Data;
using Organizr.Models;

namespace Organizr.Controllers
{
    public class TaskController : Controller
    {
        private readonly AppDBContext _context;

        public TaskController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tasks = await _context.TaskItems.Include(u => u.User).ToListAsync();
            return View(tasks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if(task == null)
            {
                return NotFound();
            }

            return View(task);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem task)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            task.UserId = userId.Value;

            if (ModelState.IsValid)
            {
                _context.TaskItems.Add(task);
                await _context.SaveChangesAsync();
                TempData["Info"] = "Task Added";
                return RedirectToAction("Index", "User");
            }

            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if(task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskItem task)
        {
            var existingTask = await _context.TaskItems.FindAsync(task.Id);
            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.IsCompleted = task.IsCompleted;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Task Updated";
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Task Deleted Successfully";

            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if(task== null)
            {
                return NotFound();
            }

            task.IsCompleted = !task.IsCompleted;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "User");
        }
    }
}
