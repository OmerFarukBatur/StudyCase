using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlturStudyCase.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeDashboardService _employeeDashboardService;

        public EmployeeController(IEmployeeDashboardService employeeDashboardService)
        {
            _employeeDashboardService = employeeDashboardService;
        }

        public async Task<IActionResult> Index()
        {
            int userId = UserId;
            var stats = await _employeeDashboardService.GetEmployeeDashboardStatsAsync(userId);
            return View(stats);
        }
    }
}