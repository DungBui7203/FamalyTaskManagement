using Microsoft.AspNetCore.Mvc;
using Web.Models.Task;
using Web.Services;

namespace Web.Controllers
{
    public class MyTaskController : Controller
    {
        private readonly ApiService _apiService;

        public MyTaskController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private void SetAuthToken()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                _apiService.SetAuthToken(token);
        }

        public async Task<IActionResult> Index(string? status = null)
        {
            SetAuthToken();
            var tasks = await _apiService.GetMyTasksAsync(status) ?? new List<TaskViewModel>();
            return View(new TaskListViewModel { Tasks = tasks });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(long id, string status)
        {
            SetAuthToken();
            var success = await _apiService.UpdateTaskStatusAsync(id, status);

            if (success)
                TempData["Success"] = "Task status updated!";
            else
                TempData["Error"] = "Failed to update task status";

            return RedirectToAction(nameof(Index));
        }
    }
}
