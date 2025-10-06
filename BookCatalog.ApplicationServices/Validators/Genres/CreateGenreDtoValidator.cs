using BookCatalog.ApplicationServices.DTOs;
using FluentValidation;

namespace BookCatalog.ApplicationServices.Validators.Genres
{
    public class CreateGenreDtoValidator : AbstractValidator<CreateGenreDto>
    {
        public CreateGenreDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название жанра обязательно для заполнения")
                .MaximumLength(50).WithMessage("Название жанра не должно превышать 50 символов")
                .MinimumLength(2).WithMessage("Название жанра должно содержать минимум 2 символа")
                .Matches(@"^[a-zA-Zа-яА-ЯёЁ\s\-]+$").WithMessage("Название жанра может содержать только буквы, пробелы и дефисы");
        }
    }
}
