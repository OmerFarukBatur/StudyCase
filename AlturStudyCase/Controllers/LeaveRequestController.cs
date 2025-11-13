using Core.DTOs.LeaveRequestDtos;
using Core.Enums;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlturStudyCase.Controllers
{
    [Authorize(Roles = "Employee")]
    public class LeaveRequestController : BaseController
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        public async Task<IActionResult> Index(int? filter, int pageIndex = 1)
        {
            int userId = UserId;

            var leaveRequests = await _leaveRequestService
                .GetUserLeaveRequestsAsync(userId, pageIndex, 5,filter);

            ViewBag.SelectedStatus = (LeaveStatus?)filter;
            return View(leaveRequests);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            int userId = UserId;
            var result = await _leaveRequestService.CreateLeaveRequestAsync(dto, userId);

            if (!result.Status)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeaveRequestUpdateDto dto)
        {
            dto.UserId = UserId;

            if (!ModelState.IsValid)
                return View(dto);
            else
            {
                try
                {
                    await _leaveRequestService.UpdateLeaveRequestAsync(dto);
                    TempData["Success"] = "İzin talebi başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(dto);
                }
            }                
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _leaveRequestService.DeleteLeaveRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("ApprovalHistory")]
        public async Task<IActionResult> ApprovalHistory(int pageIndex = 1, int? filter = null)
        {
            int userId = UserId;

            var approvalHistory = await _leaveRequestService
                .GetEmployeeApprovalHistoryAsync(userId, pageIndex, 10, filter);

            ViewBag.SelectedStatus = (ApprovalAction?)filter;
            return View(approvalHistory);
        }
    }
}
