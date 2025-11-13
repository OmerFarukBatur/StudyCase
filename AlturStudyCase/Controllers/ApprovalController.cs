using Core.DTOs.ApprovalDtos;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlturStudyCase.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ApprovalController : BaseController
    {
        private readonly IApprovalService _approvalService;
        private readonly IUserRepository _userRepository;
        private readonly ILeaveRequestService _leaveRequestService;

        public ApprovalController(IApprovalService approvalService, IUserRepository userRepository, ILeaveRequestService leaveRequestService)
        {
            _approvalService = approvalService;
            _userRepository = userRepository;
            _leaveRequestService = leaveRequestService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1,DateTime? startDate = null, DateTime? endDate = null,int? employeeId = null, int? leaveTypeId = null)
        {
            var pendingRequests = await _approvalService.GetPendingLeaveRequestsAsync(pageIndex, 10, startDate, endDate, employeeId, leaveTypeId);

            ViewBag.Employees = await _userRepository.GetWhere(u => u.Role.Name == "Employee")
                .Select(u => new { u.Id, Name = $"{u.FirstName} {u.LastName}" })
                .ToListAsync();

            ViewBag.LeaveTypes = Enum.GetValues(typeof(LeaveType))
                .Cast<LeaveType>()
                .Select(lt => new { Id = (int)lt, Name = EnumHelper.GetDisplayName(lt) })
                .ToList();

            ViewBag.Filters = new { startDate, endDate, employeeId, leaveTypeId };

            return View(pendingRequests);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _approvalService.GetLeaveRequestForEditAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto.Item1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApprovalCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            int managerId = UserId;
            var result = await _approvalService.ApproveOrRejectLeaveAsync(dto, managerId);

            if (result.Status)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Index","Approval");
            }
            else
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }
        }
    }
}
