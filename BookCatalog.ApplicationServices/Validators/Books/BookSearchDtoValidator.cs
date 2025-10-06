using BookCatalog.ApplicationServices.DTOs;
using FluentValidation;

namespace BookCatalog.ApplicationServices.Validators.Books
{
    public class BookSearchDtoValidator : AbstractValidator<BookSearchDto>
    {
        public BookSearchDtoValidator()
        {
            // Поиск по названию книги
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Поисковый запрос не должен превышать 100 символов")
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));

            // Фильтр по имени автора
            RuleFor(x => x.AuthorName)
                .MaximumLength(100).WithMessage("Имя автора не должно превышать 100 символов")
                .When(x => !string.IsNullOrEmpty(x.AuthorName));

            // Фильтр по названию жанра
            RuleFor(x => x.GenreName)
                .MaximumLength(50).WithMessage("Название жанра не должно превышать 50 символов")
                .When(x => !string.IsNullOrEmpty(x.GenreName));

            // Фильтр по минимальному году
            RuleFor(x => x.MinYear)
                .InclusiveBetween(1000, DateTime.Now.Year)
                .WithMessage($"Минимальный год должен быть между 1000 и {DateTime.Now.Year}")
                .When(x => x.MinYear.HasValue);

            // Фильтр по максимальному году
            RuleFor(x => x.MaxYear)
                .InclusiveBetween(1000, DateTime.Now.Year)
                .WithMessage($"Максимальный год должен быть между 1000 и {DateTime.Now.Year}")
                .When(x => x.MaxYear.HasValue);

            // Проверка корректности диапазона годов
            RuleFor(x => x.MaxYear)
                .GreaterThanOrEqualTo(x => x.MinYear)
                .WithMessage("Максимальный год должен быть больше или равен минимальному году")
                .When(x => x.MinYear.HasValue && x.MaxYear.HasValue);

            // Пагинация - страница
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Номер страницы должен быть больше 0");

            // Пагинация - размер страницы
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть между 1 и 100");
        }
    }
}
