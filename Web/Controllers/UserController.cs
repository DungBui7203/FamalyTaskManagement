using Microsoft.AspNetCore.Mvc;
using Web.Models.Task;
using Web.Models.User;
using Web.Services;

namespace Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApiService _apiService;

        public UserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private void SetAuthToken()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                _apiService.SetAuthToken(token);
        }

        public async Task<IActionResult> Index()
        {
            SetAuthToken();
            var users = await _apiService.GetUsersAsync() ?? new List<UserViewModel>();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetAuthToken();
            var result = await _apiService.CreateUserAsync(model);

            if (result != null)
            {
                TempData["Success"] = "Member added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to create member");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            SetAuthToken();
            var user = await _apiService.GetUserAsync(id);

            if (user == null)
                return NotFound();

            var model = new UpdateUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = user.Role
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetAuthToken();
            var success = await _apiService.UpdateUserAsync(model.Id, model);

            if (success)
            {
                TempData["Success"] = "Member updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to update member");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            SetAuthToken();
            var success = await _apiService.DeleteUserAsync(id);

            if (success)
                TempData["Success"] = "Member removed successfully!";
            else
                TempData["Error"] = "Failed to remove member";

            return RedirectToAction(nameof(Index));
        }
    }
}
