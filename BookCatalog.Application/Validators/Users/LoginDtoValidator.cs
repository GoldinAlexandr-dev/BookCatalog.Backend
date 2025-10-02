using BookCatalog.Application.DTOs;
using FluentValidation;

namespace BookCatalog.Application.Validators.Users
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя обязательно для заполнения");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен для заполнения");
        }
    }
}
