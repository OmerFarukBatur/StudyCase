using Core.DTOs.UserDtos;
using FluentValidation;

namespace Application.Validators.UserValidations
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty().WithMessage("Ad alanı boş olamaz.")
                .MaximumLength(150).WithMessage("Ad en fazla 150 karakter olabilir.");

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty().WithMessage("Soyad alanı boş olamaz.")
                .MaximumLength(150).WithMessage("Soyad en fazla 150 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty().WithMessage("Email alanı zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Şifre zorunludur.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .MaximumLength(10).WithMessage("Şifre en fazla 10 karakter olabilir.")
                .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"\d").WithMessage("Şifre en az bir rakam içermelidir.");

            RuleFor(x => x.PasswordConfirm)
                .NotNull()
                .NotEmpty()
                .WithMessage("Şifre tekrar zorunludur.")
                .MaximumLength(10).WithMessage("Şifre en fazla 10 karakter olabilir.")
                .Must((dto, confirm) => string.Equals(dto.Password.Trim(), confirm.Trim(), StringComparison.Ordinal))
                    .WithMessage("Şifreler uyuşmuyor.");
        }
    }
}