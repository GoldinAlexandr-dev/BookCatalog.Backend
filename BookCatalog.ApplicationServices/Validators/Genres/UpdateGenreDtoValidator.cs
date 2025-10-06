using BookCatalog.ApplicationServices.DTOs;
using FluentValidation;

namespace BookCatalog.ApplicationServices.Validators.Genres
{
    public class UpdateGenreDtoValidator : AbstractValidator<UpdateGenreDto>
    {
        public UpdateGenreDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID жанра должен быть больше 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название жанра обязательно для заполнения")
                .MaximumLength(50).WithMessage("Название жанра не должно превышать 50 символов")
                .MinimumLength(2).WithMessage("Название жанра должно содержать минимум 2 символа");
        }
    }
}
