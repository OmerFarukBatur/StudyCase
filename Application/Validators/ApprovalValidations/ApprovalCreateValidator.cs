using Core.DTOs.ApprovalDtos;
using Core.Enums;
using FluentValidation;

namespace Application.Validators.ApprovalValidations
{
    public class ApprovalCreateValidator : AbstractValidator<ApprovalCreateDto>
    {
        public ApprovalCreateValidator()
        {
            RuleFor(x => x.ApprovalActionId)
                .Must(id => Enum.IsDefined(typeof(ApprovalAction), id) && id != 0)
                .WithMessage("Lütfen geçerli bir karar seçiniz.");

            When(x => x.ApprovalActionId == (int)ApprovalAction.Reject, () =>
            {
                RuleFor(x => x.Comments)
                    .NotEmpty().WithMessage("Red işlemi için açıklama zorunludur.")
                    .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
            });

            When(x => x.ApprovalActionId == (int)ApprovalAction.Approve, () =>
            {
                RuleFor(x => x.Comments)
                    .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
            });

            RuleFor(x => x.RowVersion)
                .NotNull().WithMessage("Kayıt doğrulama bilgisi eksik. Lütfen sayfayı yenileyin.")
                .Must(rv => rv != null && rv.Length > 0)
                .WithMessage("Kayıt doğrulama bilgisi eksik. Lütfen sayfayı yenileyin.");

            RuleFor(x => x.LeaveRequestId)
                .GreaterThan(0).WithMessage("İzin talebi bilgisi eksik.");
        }
    }
}
