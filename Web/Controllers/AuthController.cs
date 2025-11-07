using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Thêm debug
            Console.WriteLine($"Login attempt: {model.Email}");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiService.LoginAsync(model);
                Console.WriteLine($"API Result: {result != null}");

                if (result != null)
                {
                    HttpContext.Session.SetString("Token", result.Token);
                    HttpContext.Session.SetString("UserName", result.FullName);
                    HttpContext.Session.SetString("Role", result.Role);
                    HttpContext.Session.SetString("UserId", result.UserId.ToString());

                    _apiService.SetAuthToken(result.Token);
                    if (result.Role == "Parent")
                        return RedirectToAction("Index", "Task");
                    else
                        return RedirectToAction("Index", "MyTask");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", $"Connection error: {ex.Message}");
                return View(model);
            }

            ModelState.AddModelError("", "Invalid login credentials");
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiService.RegisterAsync(model);

                if (result != null)
                {
                    HttpContext.Session.SetString("Token", result.Token);
                    HttpContext.Session.SetString("UserName", result.FullName);
                    HttpContext.Session.SetString("Role", result.Role);
                    HttpContext.Session.SetString("UserId", result.UserId.ToString());

                    _apiService.SetAuthToken(result.Token);
                    return RedirectToAction("Index", "Task");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration error: {ex.Message}");
                return View(model);
            }

            ModelState.AddModelError("", "Email already exists");
            return View(model);
        }
    }
}
