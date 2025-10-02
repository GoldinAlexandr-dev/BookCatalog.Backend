using BookCatalog.Application.DTOs;
using FluentValidation;

namespace BookCatalog.Application.Validators.Books
{
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название книги обязательно для заполнения")
                .MaximumLength(200).WithMessage("Название книги не должно превышать 200 символов")
                .MinimumLength(2).WithMessage("Название книги должно содержать минимум 2 символа");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание книги обязательно для заполнения")
                .MaximumLength(2000).WithMessage("Описание книги не должно превышать 2000 символов")
                .MinimumLength(10).WithMessage("Описание книги должно содержать минимум 10 символов");

            RuleFor(x => x.PublicationYear)
                .InclusiveBetween(1000, DateTime.Now.Year + 1)
                .WithMessage($"Год публикации должен быть между 1000 и {DateTime.Now.Year + 1}");

            RuleFor(x => x.CoverImageUrl)
                .NotEmpty().WithMessage("URL обложки книги обязателен для заполнения")
                .MaximumLength(500).WithMessage("URL обложки не должен превышать 500 символов")
                .Must(BeAValidUrl).WithMessage("URL обложки должен быть действительным URL-адресом")
                .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("ID автора должен быть больше 0");

            RuleFor(x => x.GenreIds)
                .NotNull().WithMessage("Список жанров не может быть null")
                .Must(ids => ids != null && ids.Count > 0).WithMessage("Книга должна иметь хотя бы один жанр")
                .Must(ids => ids != null && ids.Count <= 5).WithMessage("Книга не может иметь более 5 жанров")
                .Must(ids => ids == null || ids.All(id => id > 0)).WithMessage("Все ID жанров должны быть больше 0")
                .Must(ids => ids == null || ids.Distinct().Count() == ids.Count).WithMessage("Жанры не должны повторяться");
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
