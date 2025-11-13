using Core.DTOs;
using Core.DTOs.LeaveRequestDtos;
using Core.Helpers;

namespace Core.Interfaces.IServices
{
    public interface ILeaveRequestService
    {
        Task<ResponseMessageDto> CreateLeaveRequestAsync(LeaveRequestCreateDto dto, int userId);
        Task<PaginatedList<LeaveRequestListDto>> GetUserLeaveRequestsAsync(int userId, int pageIndex = 1, int pageSize = 20,int? filter = null);
        Task<LeaveRequestUpdateDto> GetLeaveRequestByIdAsync(int id);
        Task<bool> UpdateLeaveRequestAsync(LeaveRequestUpdateDto dto);
        Task DeleteLeaveRequestAsync(int id);

        Task<PaginatedList<LeaveRequestApprovalDto>> GetEmployeeApprovalHistoryAsync(int userId, int pageIndex = 1, int pageSize = 20, int? filter = null);
    }
}
