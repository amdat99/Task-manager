using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Data;

namespace TaskManager.Controllers;

public class TaskController(ILogger<TaskController> logger, TaskManagerDBContext context) : Controller
{
    private readonly ILogger<TaskController> _logger = logger;

    public IActionResult Index()
    {
        var tasks = context.Task
            .Take(30)
            .OrderByDescending(t => t.TimeStamp)
            .ToList();
        return View(tasks);
    }

    [HttpGet]
    public IActionResult Tasks()
    {
        try 
        {
            //Add simple pagination
            string lastId = Request.Query["lastId"].FirstOrDefault() ?? "0";
            var tasks = context.Task
                .Where(t => t.Id > int.Parse(lastId))
                .Take(30)
                .OrderByDescending(t => t.TimeStamp)
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
        context.Task.Add(task);
        context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPatch]
    public IActionResult SetTaskCompleted(int id)
    {
        try
        {
            var task = context.Task.Find(id);
            if (task == null)
            {
                return StatusCode(404, new { success = false, message = "Task not found" });
            }
            task.Completed = true;
            context.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating item");
            return StatusCode(500, new { success = false, message = "Error updating task" });
        }
    }

    [HttpPatch]
    public IActionResult SetTaskNotCompleted(int id)
    {
        try
        {
            var task = context.Task.Find(id);
            if (task == null)
            {
                return StatusCode(404, new { success = false, message = "Task not found" });
            }
            task.Completed = false;
            context.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating item");
            return StatusCode(500, new { success = false, message = "Error updating task" });
        }
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        try
        {
            var task = context.Task.Find(id);
            if (task == null)
            {
                return StatusCode(404, new { success = false, message = "Task not found" });
            }
            context.Task.Remove(task);
            context.SaveChanges();
            return Json( new { success = true });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting item");
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
