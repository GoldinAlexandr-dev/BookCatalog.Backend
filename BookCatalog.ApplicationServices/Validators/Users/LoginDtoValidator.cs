using BookCatalog.ApplicationServices.DTOs;
using FluentValidation;

namespace BookCatalog.ApplicationServices.Validators.Users
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
