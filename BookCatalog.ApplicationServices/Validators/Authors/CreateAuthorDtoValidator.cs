using BookCatalog.ApplicationServices.DTOs;
using FluentValidation;

namespace BookCatalog.ApplicationServices.Validators.Authors
{
    public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
    {
        public CreateAuthorDtoValidator()
        {
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
