using Core.DTOs.EmployeeDtos;

namespace Core.Interfaces.IServices
{
    public interface IEmployeeDashboardService
    {
        Task<EmployeeDashboardDto> GetEmployeeDashboardStatsAsync(int userId);
    }
}
