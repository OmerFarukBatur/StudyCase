using Core.DTOs.LeaveRequestDtos;
using FluentValidation;

namespace Application.Validators.LeaveRequestValidations
{
    public class LeaveRequestUpdateValidator : AbstractValidator<LeaveRequestUpdateDto>
    {
        public LeaveRequestUpdateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık zorunludur.")
                .MaximumLength(200);

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate).WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz.")
                .Must(dt => dt.Date >= DateTime.Today.AddDays(-7).Date).WithMessage("Başlangıç tarihi bugünden en fazla 7 gün geriye gidebilir.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Bitiş tarihi başlangıç tarihinden önce olamaz.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("İzin nedeni zorunludur.")
                .MaximumLength(500);

            RuleFor(x => x.RowVersion).NotNull().WithMessage("Geçerlilik doğrulaması için RowVersion gerekli.").When(x => x.RowVersion != null);
        }
    }
}
