using Core.DTOs;
using Core.DTOs.LeaveRequestDtos;
using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IUserRepository _userRepository;

        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, IApprovalRepository approvalRepository, IUserRepository userRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _approvalRepository = approvalRepository;
            _userRepository = userRepository;
        }

        public async Task<ResponseMessageDto> CreateLeaveRequestAsync(LeaveRequestCreateDto dto, int userId)
        {
            var overlap = await _leaveRequestRepository.Table.AnyAsync(lr =>
                lr.UserId == userId &&
                (lr.LeaveStatusId == (int)LeaveStatus.Pending || lr.LeaveStatusId == (int)LeaveStatus.Approved) &&
                lr.StartDate <= dto.EndDate && dto.StartDate <= lr.EndDate);

            if (overlap)
            {
                return new ResponseMessageDto()
                {
                    Status = false,
                    Message = "Seçilen tarihlerde başka bir izin zaten mevcut."
                };
            }

            var leaveRequest = new LeaveRequest
            {
                UserId = userId,
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                LeaveTypeId = dto.LeaveTypeId,
                LeaveStatusId = (int)LeaveStatus.Pending,
                Reason = dto.Reason,
                CreatedBy = userId
            };

            await _leaveRequestRepository.AddAsync(leaveRequest);
            await _leaveRequestRepository.SaveAsync();

            return new ResponseMessageDto()
            {
                Status = true,
                Message = "İzin talebi başarıyla oluşturuldu."
            };
        }

        public async Task<PaginatedList<LeaveRequestListDto>> GetUserLeaveRequestsAsync(int userId, int pageIndex = 1, int pageSize = 20,int? filter = null)
        {
            var query = _leaveRequestRepository.Table
                .Where(lr => lr.UserId == userId)
                .OrderByDescending(lr => lr.CreatedAt)
                .Select(lr => new LeaveRequestListDto
                {
                    Id = lr.Id,
                    Title = lr.Title,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    LeaveTypeId = lr.LeaveTypeId,
                    LeaveType = ((LeaveType)lr.LeaveTypeId).GetDisplayName(),
                    Reason = lr.Reason,
                    LeaveStatusId = lr.LeaveStatusId,
                    LeaveStatus = ((LeaveStatus)lr.LeaveStatusId).GetDisplayName(),
                    RejectionReason = lr.RejectionReason,
                    CreatedAt = lr.CreatedAt,
                    UpdatedAt = lr.UpdatedAt,
                    CreatedBy = lr.CreatedBy,
                    UpdatedBy = lr.UpdatedBy
                });

            if (filter.HasValue)
                query = query.Where(lr => lr.LeaveStatusId == filter.Value);

            return await PaginatedList<LeaveRequestListDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<LeaveRequestUpdateDto> GetLeaveRequestByIdAsync(int id)
        {
            var entity = await _leaveRequestRepository.Table.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return null;

            return new LeaveRequestUpdateDto
            {
                Id = entity.Id,
                Title = entity.Title,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                LeaveTypeId = entity.LeaveTypeId,
                Reason = entity.Reason,
                UserId = entity.UserId,
                RowVersion = entity.RowVersion
            };
        }

        public async Task<bool> UpdateLeaveRequestAsync(LeaveRequestUpdateDto dto)
        {
            var entity = await _leaveRequestRepository.Table
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Talep bulunamadı.");

            if (entity.UserId != dto.UserId)
                throw new Exception("Bu talebi güncelleme yetkiniz yok.");

            if (entity.LeaveStatusId != (int)LeaveStatus.Pending)
                throw new Exception("Sadece bekleyen talepler güncellenebilir.");

            bool overlap = await _leaveRequestRepository.Table.AnyAsync(x =>
                x.UserId == dto.UserId &&
                x.Id != dto.Id &&
                (x.LeaveStatusId == 1 || x.LeaveStatusId == 2) &&
                (
                    (dto.StartDate >= x.StartDate && dto.StartDate <= x.EndDate) ||
                    (dto.EndDate >= x.StartDate && dto.EndDate <= x.EndDate)
                )
            );

            if (overlap)
                throw new Exception("Bu tarih aralığında başka bir onaylı veya bekleyen izniniz var.");

            entity.Title = dto.Title;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.LeaveTypeId = dto.LeaveTypeId ?? entity.LeaveTypeId;
            entity.Reason = dto.Reason;
            entity.UpdatedAt = DateTime.Now;

            _leaveRequestRepository.Table.Entry(entity).OriginalValues["RowVersion"] = dto.RowVersion;

            try
            {
                _leaveRequestRepository.Update(entity);
                await _leaveRequestRepository.SaveAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Kayıt başka bir kullanıcı tarafından güncellendi. Lütfen sayfayı yenileyip tekrar deneyin.");
            }
        }

        public async Task DeleteLeaveRequestAsync(int id)
        {
            var entity = await _leaveRequestRepository.GetByIdAsync(id);
            if (entity != null)
            {
                await _leaveRequestRepository.RemoveAsync(id);
                await _leaveRequestRepository.SaveAsync();
            }
        }

        public async Task<PaginatedList<LeaveRequestApprovalDto>> GetEmployeeApprovalHistoryAsync(int userId, int pageIndex = 1, int pageSize = 20, int? filter = null)
        {
            var query = from lr in _leaveRequestRepository.Table
                        where lr.UserId == userId
                        join a in _approvalRepository.Table
                            on lr.Id equals a.LeaveRequestId
                        join m in _userRepository.Table
                            on a.ManagerId equals m.Id
                        select new LeaveRequestApprovalDto
                        {
                            Id = a.Id,
                            Title = lr.Title,
                            StartDate = lr.StartDate,
                            EndDate = lr.EndDate,
                            LeaveType = ((LeaveType)lr.LeaveTypeId).GetDisplayName(),
                            ApprovalAction = ((ApprovalAction)a.ApprovalActionId).GetDisplayName(),
                            ApprovalActionId = (int)a.ApprovalActionId,
                            ManagerName = $"{m.FirstName} {m.LastName}",
                            Comments = a.Comments,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt
                        };

            if (filter.HasValue)
            {
                query = query.Where(x => x.ApprovalActionId == filter.Value);
            }
            query = query.OrderByDescending(x => x.UpdatedAt);

            return await PaginatedList<LeaveRequestApprovalDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
