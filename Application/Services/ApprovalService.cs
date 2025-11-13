using Core.DTOs;
using Core.DTOs.ApprovalDtos;
using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IApprovalRepository _approvalRepository;

        public ApprovalService(
            ILeaveRequestRepository leaveRequestRepository,
            IApprovalRepository approvalRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _approvalRepository = approvalRepository;
        }

        public async Task<PaginatedList<LeaveRequestApprovalDto>> GetPendingLeaveRequestsAsync(int pageIndex = 1, int pageSize = 20,
            DateTime? startDate = null, DateTime? endDate = null, int? employeeId = null, int? leaveTypeId = null)
        {
            var query = _leaveRequestRepository.Table
                .Where(lr => lr.LeaveStatusId == (int)LeaveStatus.Pending)
                .Include(lr => lr.User)
                .OrderBy(lr => lr.CreatedAt)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(lr => lr.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(lr => lr.EndDate <= endDate.Value);

            if (employeeId.HasValue)
                query = query.Where(lr => lr.UserId == employeeId.Value);

            if (leaveTypeId.HasValue)
                query = query.Where(lr => lr.LeaveTypeId == leaveTypeId.Value);

            var resultQuery = query.Select(lr => new LeaveRequestApprovalDto
            {
                Id = lr.Id,
                Title = lr.Title,
                EmployeeName = $"{lr.User.FirstName} {lr.User.LastName}",
                EmployeeEmail = lr.User.Email,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                LeaveType = ((LeaveType)lr.LeaveTypeId).ToString(),
                LeaveTypeId = (int)lr.LeaveTypeId,
                Reason = lr.Reason,
                CreatedAt = lr.CreatedAt,
                RowVersion = lr.RowVersion
            });

            return await PaginatedList<LeaveRequestApprovalDto>.CreateAsync(resultQuery, pageIndex, pageSize);
        }


        public async Task<Tuple<ApprovalCreateDto?, ResponseMessageDto>> GetLeaveRequestForEditAsync(int id)
        {
            var leaveRequest = await _leaveRequestRepository.Table
                .Include(lr => lr.User)
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.LeaveStatusId == (int)LeaveStatus.Pending);

            if (leaveRequest == null)
                return new Tuple<ApprovalCreateDto?, ResponseMessageDto>(null,new ResponseMessageDto
                {
                    Status = false,
                    Message = "İzin talebi bulunamadı veya zaten işlem görmüş."
                });

            return new Tuple<ApprovalCreateDto?, ResponseMessageDto>(new ApprovalCreateDto
            {
                LeaveRequestId = leaveRequest.Id,
                EmployeeId = leaveRequest.UserId,
                EmployeeFullName = $"{leaveRequest.User.FirstName} {leaveRequest.User.LastName}",
                Title = leaveRequest.Title,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                LeaveType = ((LeaveType)leaveRequest.LeaveTypeId).GetDisplayName(),
                LeaveTypeId = leaveRequest.LeaveTypeId ?? 0,
                Reason = leaveRequest.Reason,
                RowVersion = leaveRequest.RowVersion
            }, 
            new ResponseMessageDto
            {
                Status = true,
                Message = ""
            });
        }

        public async Task<ResponseMessageDto> ApproveOrRejectLeaveAsync(ApprovalCreateDto dto, int managerId)
        {
            try
            {
                var leaveRequest = await _leaveRequestRepository.GetByIdAsync(dto.LeaveRequestId);
                
                if (!leaveRequest.RowVersion.SequenceEqual(dto.RowVersion))
                    return new ResponseMessageDto()
                    {
                        Status = false,
                        Message = "Bu kayıt başka bir yönetici tarafından güncellenmiş. Lütfen sayfayı yenileyin."
                    };

                if (!dto.IsApproved && string.IsNullOrWhiteSpace(dto.Comments))
                    return new ResponseMessageDto()
                    {
                        Status = false,
                        Message = "Red işlemi için açıklama zorunludur."
                    };                

                leaveRequest.LeaveStatusId = dto.IsApproved ? (int)LeaveStatus.Approved : (int)LeaveStatus.Rejected;
                leaveRequest.UpdatedBy = managerId;

                var approval = new Approval
                {
                    LeaveRequestId = leaveRequest.Id,
                    ManagerId = managerId,
                    ApprovalActionId = dto.IsApproved ? (int)ApprovalAction.Approve : (int)ApprovalAction.Reject,
                    Comments = dto.Comments,
                    CreatedBy = managerId
                };

                _leaveRequestRepository.Update(leaveRequest);
                await _leaveRequestRepository.SaveAsync();

                await _approvalRepository.AddAsync(approval);
                await _approvalRepository.SaveAsync();

                return new ResponseMessageDto()
                {
                    Status = true,
                    Message ="İzin talebi onaylama işlemi başarıyla kaydedildi."
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ResponseMessageDto()
                {
                    Status = false,
                    Message = "Bu kayıt başka bir kullanıcı tarafından güncellenmiş.Lütfen sayfayı yenileyin."
                };
            }
            catch (Exception ex)
            {
                return new ResponseMessageDto()
                {
                    Status = false,
                    Message = $"İşlem sırasında hata oluştu: {ex.Message}"
                };
            }
        }
    }
}
