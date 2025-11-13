using Core.DTOs.AdminDtos;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public AdminService(IRoleRepository roleRepository, IUserRepository userRepository, ILeaveRequestRepository leaveRequestRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<List<RoleDto>> GetAllRole()
        {
            return await _roleRepository.Table.Select(x=> new RoleDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }

        public async Task<List<UserDto>> GetAllUser()
        {
            return await _userRepository.Table.Select(x => new UserDto
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                IsActive = x.IsActive,
                RoleId = x.RoleId,
                RoleName = x.Role.Name
            }).ToListAsync();
        }

        public async Task<List<MonthlyLeaveSummaryDto>> GetMonthlyLeaveSummaryAsync(int year, int month)
        {
            var leaveRequests = await _leaveRequestRepository.Table
                .Include(lr => lr.User)
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .Where(lr => lr.StartDate.Year == year && lr.StartDate.Month == month)
                .Select(lr => new
                {
                    lr.UserId,
                    lr.User.FirstName,
                    lr.User.LastName,
                    lr.StartDate,
                    lr.EndDate
                })
                .ToListAsync();

            var result = leaveRequests
                .GroupBy(lr => new { lr.UserId, lr.FirstName, lr.LastName })
                .Select(g => new MonthlyLeaveSummaryDto
                {
                    EmployeeId = g.Key.UserId,
                    EmployeeFullName = $"{g.Key.FirstName} {g.Key.LastName}",
                    Year = year,
                    Month = month,
                    TotalApprovedDays = g.Sum(x => (int)(x.EndDate - x.StartDate).TotalDays + 1)
                })
                .ToList();

            return result;
        }

        public async Task<int> GetApprovedLeavesThisMonthAsync()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .Where(lr => lr.StartDate >= firstDayOfMonth && lr.StartDate <= lastDayOfMonth)
                .CountAsync();
        }

        public async Task<MostUsedLeaveTypeDto> GetMostUsedLeaveTypeAsync()
        {
            var result = await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .GroupBy(lr => lr.LeaveTypeId)
                .Select(g => new
                {
                    LeaveTypeId = g.Key,
                    UsageCount = g.Count()
                })
                .OrderByDescending(x => x.UsageCount)
                .FirstOrDefaultAsync();

            if (result == null)
                return new MostUsedLeaveTypeDto { TypeName = "Veri Yok", UsageCount = 0 };


            return new MostUsedLeaveTypeDto
            {
                TypeName = ((LeaveType)result.LeaveTypeId).GetDisplayName(),
                UsageCount = result.UsageCount
            };
        }

        public async Task<int> GetPendingRequestsCountAsync()
        {
            return await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Pending)
                .CountAsync();
        }

        public async Task<double> GetAverageLeaveDaysAsync()
        {
            var leaveRequests = await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .Select(lr => new { lr.StartDate, lr.EndDate })
                .ToListAsync();

            if (!leaveRequests.Any())
                return 0;

            var average = leaveRequests
                .Average(lr => (lr.EndDate - lr.StartDate).TotalDays + 1);

            return Math.Round(average, 1);
        }

        public async Task<double> GetAnnualLeaveUsageRateAsync()
        {
            var leaveRequests = await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .Where(lr => lr.StartDate.Year == DateTime.Today.Year)
                .Select(lr => new { lr.StartDate, lr.EndDate })
                .ToListAsync();

            var totalEmployees = await GetTotalEmployeesAsync();

            if (totalEmployees == 0)
                return 0;

            var totalAnnualLeaveDays = leaveRequests
                .Sum(lr => (lr.EndDate - lr.StartDate).TotalDays + 1);

            var maxPossibleLeaves = totalEmployees * 20;

            if (maxPossibleLeaves == 0)
                return 0;

            var usageRate = (totalAnnualLeaveDays / maxPossibleLeaves) * 100;
            return Math.Round(usageRate, 1);
        }

        public async Task<int> GetThisWeekLeavesAsync()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);

            return await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .Where(lr => lr.StartDate >= startOfWeek && lr.StartDate <= endOfWeek)
                .CountAsync();
        }

        public async Task<int> GetUpcomingLeavesAsync()
        {
            var today = DateTime.Today;
            var nextWeek = today.AddDays(7);

            return await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Approved)
                .Where(lr => lr.StartDate >= today && lr.StartDate <= nextWeek)
                .CountAsync();
        }

        public async Task<int> GetRejectedLeavesThisMonthAsync()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return await _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Rejected)
                .Where(lr => lr.StartDate >= firstDayOfMonth && lr.StartDate <= lastDayOfMonth)
                .CountAsync();
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            return await _userRepository.Table
                .Where(u => u.IsActive)
                .CountAsync();
        }
    }
}
