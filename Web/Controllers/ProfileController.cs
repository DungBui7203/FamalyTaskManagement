using Microsoft.AspNetCore.Mvc;
using Web.Services;
using Web.Views;

namespace Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        private void SetAuthToken()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                _profileService.SetAuthToken(token);
        }

        public async Task<IActionResult> Index()
        {
            SetAuthToken();
            var profile = await _profileService.GetProfileAsync();

            if (profile == null)
                return RedirectToAction("Login", "Auth");

            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            SetAuthToken();
            var profile = await _profileService.GetProfileAsync();

            if (profile == null)
                return RedirectToAction("Login", "Auth");

            var model = new UpdateProfileViewModel
            {
                FullName = profile.FullName,
                Email = profile.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetAuthToken();
            var result = await _profileService.UpdateProfileAsync(model);

            if (result != null)
            {
                HttpContext.Session.SetString("UserName", result.FullName);
                TempData["Success"] = "Thông tin cá nhân đã được cập nhật!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Không thể cập nhật thông tin. Email có thể đã tồn tại.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetAuthToken();
            var (success, errorMessage) = await _profileService.ChangePasswordAsync(model);

            if (success)
            {
                TempData["Success"] = "Mật khẩu đã được thay đổi thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("CurrentPassword", errorMessage ?? "Mật khẩu hiện tại không đúng.");
            return View(model);
        }

    }
}
