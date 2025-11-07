using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Web.Models.Profile;
using Web.Services;

namespace Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProfileController(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;

            // TỰ ĐỘNG GẮN TOKEN TỪ SESSION VÀO HttpClient
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _apiService.SetAuthToken(token);
            }
        }

        public async Task<IActionResult> Index()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Gọi song song 2 API
            var userTask = _apiService.GetMyProfileAsync();
            var familyTask = _apiService.GetMyFamilyAsync();

            await Task.WhenAll(userTask, familyTask);

            var user = await userTask;
            var family = await familyTask;

            // Nếu API lỗi (401, 500, v.v.) → trả null → chuyển hướng login
            if (user == null || family == null)
            {
                TempData["Error"] = "Không thể tải thông tin. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            var model = new ProfileViewModel
            {
                User = user,
                Family = family
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await BuildViewModel());
            }

            // Gắn lại token trước khi gọi API
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _apiService.SetAuthToken(token);
            }

            var success = await _apiService.ChangePasswordAsync(model);

            TempData[success ? "Success" : "Error"] = success
                ? "Đổi mật khẩu thành công!"
                : "Đổi mật khẩu thất bại. Vui lòng thử lại.";

            return RedirectToAction("Index");
        }
        private async Task<ProfileViewModel> BuildViewModel()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _apiService.SetAuthToken(token);
            }

            var userTask = _apiService.GetMyProfileAsync();
            var familyTask = _apiService.GetMyFamilyAsync();
            await Task.WhenAll(userTask, familyTask);

            return new ProfileViewModel
            {
                User = await userTask ?? new ProfileUserViewModel(),
                Family = await familyTask ?? new FamilyViewModel()
            };
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", await BuildViewModel());

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                _apiService.SetAuthToken(token);

            // DÙNG _apiService ĐỂ GỌI API → KHÔNG CẦN _httpClient
            var success = await _apiService.UpdateProfileAsync(model);

            TempData[success ? "Success" : "Error"] = success
                ? "Cập nhật hồ sơ thành công!"
                : "Cập nhật thất bại. Vui lòng thử lại.";

            return RedirectToAction("Index");
        }
    }
}
