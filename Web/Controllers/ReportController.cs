using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportController(ApiService apiService, IHttpContextAccessor httpContextAccessor)
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
            // Kiểm tra đăng nhập
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Gọi song song 2 API
            var membersTask = _apiService.GetFamilyMembersAsync();
            var reportTask = _apiService.GetTaskReportAsync();

            await Task.WhenAll(membersTask, reportTask);

            var members = await membersTask;
            var report = await reportTask;

            // Nếu API lỗi → trả null → chuyển hướng login
            if (members == null || report == null)
            {
                TempData["Error"] = "Không thể tải báo cáo. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            var model = new ReportViewModel
            {
                Members = members,
                Report = report
            };

            return View(model);
        }
    }
}
