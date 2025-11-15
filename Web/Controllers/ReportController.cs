using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.Report;
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

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                _apiService.SetAuthToken(token);
        }

        public async Task<IActionResult> Index()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var statsTask = _apiService.GetMemberTaskStatsAsync();
            var overdueTask = _apiService.GetOverdueTasksAsync();

            await Task.WhenAll(statsTask, overdueTask);

            var stats = await statsTask;
            var overdue = await overdueTask;

            if (stats == null || overdue == null)
            {
                TempData["Error"] = "Không thể tải báo cáo. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            return View(new ReportViewModel { MemberStats = stats, OverdueGroups = overdue });
        }
    }
}
