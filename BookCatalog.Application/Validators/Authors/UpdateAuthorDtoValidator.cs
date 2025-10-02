using BookCatalog.Application.DTOs;
using FluentValidation;

namespace BookCatalog.Application.Validators.Authors
{
    public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
    {
        public UpdateAuthorDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID автора должен быть больше 0");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Имя автора обязательно для заполнения")
                .MaximumLength(100).WithMessage("Имя автора не должно превышать 100 символов")
                .MinimumLength(2).WithMessage("Имя автора должно содержать минимум 2 символа");

            RuleFor(x => x.Biography)
                .NotEmpty().WithMessage("Биография автора обязательна для заполнения")
                .MaximumLength(2000).WithMessage("Биография не должна превышать 2000 символов")
                .MinimumLength(10).WithMessage("Биография должна содержать минимум 10 символов");
        }
    }
}
