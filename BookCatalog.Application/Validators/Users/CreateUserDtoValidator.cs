using BookCatalog.Application.DTOs;
using FluentValidation;

namespace BookCatalog.Application.Validators.Users
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя обязательно для заполнения")
                .MinimumLength(3).WithMessage("Имя пользователя должно содержать минимум 3 символа")
                .MaximumLength(50).WithMessage("Имя пользователя не должно превышать 50 символов")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя может содержать только буквы, цифры и подчеркивания");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен для заполнения")
                .MaximumLength(100).WithMessage("Email не должен превышать 100 символов")
                .EmailAddress().WithMessage("Неверный формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен для заполнения")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов")
                .MaximumLength(100).WithMessage("Пароль не должен превышать 100 символов");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Роль обязательна для заполнения")
                .Must(role => role == "User" || role == "Admin").WithMessage("Роль должна быть 'User' или 'Admin'");
        }
    }
}
