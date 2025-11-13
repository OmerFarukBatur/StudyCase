using Core.DTOs.EmployeeDtos;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class EmployeeDashboardService : IEmployeeDashboardService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IApprovalRepository _approvalRepository;

        public EmployeeDashboardService(ILeaveRequestRepository leaveRequestRepository, IApprovalRepository approvalRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _approvalRepository = approvalRepository;
        }

        public async Task<EmployeeDashboardDto> GetEmployeeDashboardStatsAsync(int userId)
        {
            var oneYearAgo = DateTime.Now.AddYears(-1);

            var leaveRequests = await _leaveRequestRepository.Table
                .Where(lr => lr.UserId == userId && lr.CreatedAt >= oneYearAgo)
                .ToListAsync();

            var totalLeaveRequests = leaveRequests.Count;
            var approvedLeaves = leaveRequests.Count(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved);
            var rejectedLeaves = leaveRequests.Count(lr => lr.LeaveStatusId == (int)LeaveStatus.Rejected);
            var pendingLeaves = leaveRequests.Count(lr => lr.LeaveStatusId == (int)LeaveStatus.Pending);

            var totalApprovedDays = leaveRequests
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .Sum(lr => (lr.EndDate - lr.StartDate).Days + 1);

            var mostUsedLeaveType = leaveRequests
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .GroupBy(lr => lr.LeaveTypeId)
                .Select(g => new { LeaveTypeId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            return new EmployeeDashboardDto
            {
                TotalLeaveRequests = totalLeaveRequests,
                ApprovedLeaves = approvedLeaves,
                RejectedLeaves = rejectedLeaves,
                PendingLeaves = pendingLeaves,
                TotalApprovedDays = totalApprovedDays,
                MostUsedLeaveType = mostUsedLeaveType?.LeaveTypeId != null ?
                    ((LeaveType)mostUsedLeaveType.LeaveTypeId).GetDisplayName() : "Henüz yok"
            };
        }
    }
}
