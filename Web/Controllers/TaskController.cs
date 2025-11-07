using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.Task;
using Web.Services;

namespace Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApiService _apiService;

        public TaskController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private void SetAuthToken()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                _apiService.SetAuthToken(token);
        }

        public async Task<IActionResult> Index(string? status = null, long? assigneeId = null)
        {
            SetAuthToken();

            var tasks = await _apiService.GetTasksAsync(status, assigneeId) ?? new List<TaskViewModel>();
            var members = await _apiService.GetFamilyMembersAsync() ?? new List<UserViewModel>();

            var viewModel = new TaskListViewModel
            {
                Tasks = tasks,
                FamilyMembers = members,
                StatusFilter = status,
                AssigneeFilter = assigneeId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetAuthToken();

            var members = await _apiService.GetFamilyMembersAsync() ?? new List<UserViewModel>();
            ViewBag.FamilyMembers = members;

            return View(new TaskViewModel { DueDate = DateTime.Now.AddDays(1) });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetAuthToken();
                var members = await _apiService.GetFamilyMembersAsync() ?? new List<UserViewModel>();
                ViewBag.FamilyMembers = members;
                return View(model);
            }

            SetAuthToken();
            var result = await _apiService.CreateTaskAsync(model);

            if (result != null)
            {
                TempData["Success"] = "Task created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to create task");
            return View(model);
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

        public async Task<IActionResult> MyTasks()
        {
            SetAuthToken();
            var tasks = await _apiService.GetMyTasksAsync() ?? new List<TaskViewModel>();
            return View("Index", new TaskListViewModel { Tasks = tasks });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTask(long id)
        {
            SetAuthToken();
            var success = await _apiService.VerifyTaskAsync(id);

            if (success)
                TempData["Success"] = "Task verified successfully!";
            else
                TempData["Error"] = "Failed to verify task";

            return RedirectToAction(nameof(Index));
        }

    }
}
