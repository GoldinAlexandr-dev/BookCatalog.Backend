using BookCatalog.ApplicationServices.DTOs;
using FluentValidation;

namespace BookCatalog.ApplicationServices.Validators.Users
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID пользователя должен быть больше 0");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя обязательно для заполнения")
                .MinimumLength(3).WithMessage("Имя пользователя должно содержать минимум 3 символа")
                .MaximumLength(50).WithMessage("Имя пользователя не должно превышать 50 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен для заполнения")
                .MaximumLength(100).WithMessage("Email не должен превышать 100 символов")
                .EmailAddress().WithMessage("Неверный формат email");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Роль обязательна для заполнения")
                .Must(role => role == "User" || role == "Admin").WithMessage("Роль должна быть 'User' или 'Admin'");
        }
    }
}
