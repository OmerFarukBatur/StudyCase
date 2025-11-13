using Core.DTOs.AdminDtos;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AlturStudyCase.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var approvedLeaves = await _adminService.GetApprovedLeavesThisMonthAsync();
            var mostUsedType = await _adminService.GetMostUsedLeaveTypeAsync();
            var pendingRequests = await _adminService.GetPendingRequestsCountAsync();
            var totalEmployees = await _adminService.GetTotalEmployeesAsync();
            var thisWeekLeaves = await _adminService.GetThisWeekLeavesAsync();
            var rejectedLeavesThisMonth = await _adminService.GetRejectedLeavesThisMonthAsync();
            var averageLeaveDays = await _adminService.GetAverageLeaveDaysAsync();
            var upcomingLeaves = await _adminService.GetUpcomingLeavesAsync();
            var annualLeaveUsageRate = await _adminService.GetAnnualLeaveUsageRateAsync();

        var viewModel = new DashboardSummaryViewModel
            {
                ApprovedLeavesThisMonth = approvedLeaves,
                MostUsedLeaveType = mostUsedType,
                PendingRequestsCount = pendingRequests,
                AnnualLeaveUsageRate = annualLeaveUsageRate,
                AverageLeaveDays = averageLeaveDays,
                RejectedLeavesThisMonth = rejectedLeavesThisMonth,
                ThisWeekLeaves = thisWeekLeaves,
                TotalEmployees = totalEmployees,
                UpcomingLeaves = upcomingLeaves
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Roles()
        {
            return View(await _adminService.GetAllRole());
        }

        public async Task<IActionResult> Users()
        {
            return View(await _adminService.GetAllUser());
        }

        [HttpGet]
        public async Task<IActionResult> MonthlySummary(int? year, int? month)
        {
            year ??= DateTime.Now.Year;
            month ??= DateTime.Now.Month;

            var result = await _adminService.GetMonthlyLeaveSummaryAsync(year.Value, month.Value);
            ViewBag.SelectedYear = year;
            ViewBag.SelectedMonth = month;

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(int year, int month)
        {
            var result = await _adminService.GetMonthlyLeaveSummaryAsync(year, month);

            var csv = new StringBuilder();
            csv.AppendLine("Çalışan Adı,Yıl,Ay,Toplam Onaylı Gün");

            foreach (var item in result)
                csv.AppendLine($"{item.EmployeeFullName},{item.Year},{item.Month},{item.TotalApprovedDays}");

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"AylikIzinOzeti_{year}_{month}.csv");
        }
    }
}
