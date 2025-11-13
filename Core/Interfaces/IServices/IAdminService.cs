using Core.DTOs.AdminDtos;

namespace Core.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<List<RoleDto>> GetAllRole();
        Task<List<UserDto>> GetAllUser();
        Task<List<MonthlyLeaveSummaryDto>> GetMonthlyLeaveSummaryAsync(int year, int month);
        Task<int> GetApprovedLeavesThisMonthAsync();
        Task<MostUsedLeaveTypeDto> GetMostUsedLeaveTypeAsync();
        Task<int> GetPendingRequestsCountAsync();
        Task<int> GetTotalEmployeesAsync();
        Task<int> GetThisWeekLeavesAsync();
        Task<int> GetRejectedLeavesThisMonthAsync();
        Task<double> GetAverageLeaveDaysAsync();
        Task<int> GetUpcomingLeavesAsync();
        Task<double> GetAnnualLeaveUsageRateAsync();
    }
}
