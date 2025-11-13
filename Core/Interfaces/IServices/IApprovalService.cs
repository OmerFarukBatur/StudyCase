using Core.DTOs;
using Core.DTOs.ApprovalDtos;
using Core.Helpers;

namespace Core.Interfaces.IServices
{
    public interface IApprovalService
    {
        Task<PaginatedList<LeaveRequestApprovalDto>> GetPendingLeaveRequestsAsync(int pageIndex = 1, int pageSize = 20,
        DateTime? startDate = null, DateTime? endDate = null, int? employeeId = null, int? leaveTypeId = null);
        Task<Tuple<ApprovalCreateDto?, ResponseMessageDto>> GetLeaveRequestForEditAsync(int id);
        Task<ResponseMessageDto> ApproveOrRejectLeaveAsync(ApprovalCreateDto dto, int managerId);        
    }
}
