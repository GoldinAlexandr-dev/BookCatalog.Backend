using BookCatalog.Application.DTOs;
using FluentValidation;

namespace BookCatalog.Application.Validators.Reviews
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Текст отзыва обязателен для заполнения")
                .MaximumLength(2000).WithMessage("Текст отзыва не должен превышать 2000 символов")
                .MinimumLength(10).WithMessage("Текст отзыва должен содержать минимум 10 символов");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Рейтинг должен быть от 1 до 5");

            RuleFor(x => x.BookId)
                .GreaterThan(0).WithMessage("ID книги должен быть больше 0");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("ID пользователя должен быть больше 0");
        }
    }
}
