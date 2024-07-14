using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Data;

namespace TaskManager.Controllers;

public class TaskController(ILogger<TaskController> logger, TaskManagerDBContext context) : Controller
{
    private readonly ILogger<TaskController> _logger = logger;
    private readonly TaskManagerDBContext _context = context;

    public IActionResult Index()
    {
        var tasks = _context.Task
            .OrderByDescending(t => t.TimeStamp)
            .Take(30)
            .ToList();
        return View(tasks);
    }

    [HttpGet]
    public IActionResult Tasks()
    {
        try 
        {
            string lastIdString = Request.Query["lastId"].FirstOrDefault() ?? "0";
            int lastId = int.TryParse(lastIdString, out int parsedId) ? parsedId : 0;
            var tasks = _context.Task
                .Where(t => t.Id > lastId)
                .OrderByDescending(t => t.TimeStamp)
                .Take(30)
                .ToList();
            return Json(tasks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting tasks");
            return StatusCode(500, new { success = false, message = "Error getting tasks" });
        }
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View(new TaskModel());
    }

    [HttpPost]
    public IActionResult Add(TaskModel task)
    {
        if (!ModelState.IsValid)
        {
            return View("Add", task);
        }

        _context.Task.Add(task);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPatch]
    public IActionResult SetTaskCompleted(int id)
    {
        return SetTaskCompletionStatus(id, true);
    }

    [HttpPatch]
    public IActionResult SetTaskNotCompleted(int id)
    {
        return SetTaskCompletionStatus(id, false);
    }

    private IActionResult SetTaskCompletionStatus(int id, bool isCompleted)
    {
        try
        {
            var task = _context.Task.Find(id);
            if (task == null)
            {
                return StatusCode(404, new { success = false, message = "Task not found" });
            }

            task.Completed = isCompleted;
            _context.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating task");
            return StatusCode(500, new { success = false, message = "Error updating task" });
        }
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        try
        {
            var task = _context.Task.Find(id);
            if (task == null)
            {
                return StatusCode(404, new { success = false, message = "Task not found" });
            }

            _context.Task.Remove(task);
            _context.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting task");
            return StatusCode(500, new { success = false, message = "Error deleting task" });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
