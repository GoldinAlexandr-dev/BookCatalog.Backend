using BookCatalog.Application.DTOs;
using FluentValidation;

namespace BookCatalog.Application.Validators.Books
{
    public class BookDtoValidator : AbstractValidator<BookDto>
    {
        public BookDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID книги должен быть больше 0");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название книги обязательно для заполнения")
                .MaximumLength(200).WithMessage("Название книги не должно превышать 200 символов");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание книги обязательно для заполнения")
                .MaximumLength(2000).WithMessage("Описание книги не должно превышать 2000 символов");

            RuleFor(x => x.PublicationYear)
                .InclusiveBetween(1000, DateTime.Now.Year + 1)
                .WithMessage($"Год публикации должен быть между 1000 и {DateTime.Now.Year + 1}");

            RuleFor(x => x.CoverImageUrl)
                .NotEmpty().WithMessage("URL обложки книги обязателен для заполнения")
                .MaximumLength(500).WithMessage("URL обложки не должен превышать 500 символов")
                .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));

            RuleFor(x => x.AuthorName)
                .NotEmpty().WithMessage("Имя автора обязательно для заполнения")
                .MaximumLength(100).WithMessage("Имя автора не должно превышать 100 символов");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("ID автора должен быть больше 0");

            RuleFor(x => x.Genres)
                .NotNull().WithMessage("Список жанров не может быть null")
                .Must(genres => genres == null || genres.All(g => !string.IsNullOrEmpty(g)))
                .WithMessage("Названия жанров не могут быть пустыми");

            RuleFor(x => x.AverageRating)
                .InclusiveBetween(0, 5).WithMessage("Средний рейтинг должен быть между 0 и 5");

            RuleFor(x => x.ReviewsCount)
                .GreaterThanOrEqualTo(0).WithMessage("Количество отзывов не может быть отрицательным");
        }
    }
}
