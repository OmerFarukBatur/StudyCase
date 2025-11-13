using Core.DTOs.LeaveRequestDtos;
using FluentValidation;

namespace Application.Validators.LeaveRequestValidations
{
    public class LeaveRequestCreateValidator : AbstractValidator<LeaveRequestCreateDto>
    {
        public LeaveRequestCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık zorunludur.")
                .MaximumLength(200);

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz.")
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-7))
                .WithMessage("İzin başlangıç tarihi bugünden en fazla 7 gün geriye gidebilir.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Bitiş tarihi başlangıç tarihinden önce olamaz.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("İzin nedeni zorunludur.")
                .MaximumLength(500);
        }
    }
}