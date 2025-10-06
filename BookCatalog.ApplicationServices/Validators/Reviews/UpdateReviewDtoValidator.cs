using BookCatalog.ApplicationServices.DTOs;
using FluentValidation;

namespace BookCatalog.ApplicationServices.Validators.Reviews
{
    public class UpdateReviewDtoValidator : AbstractValidator<UpdateReviewDto>
    {
        public UpdateReviewDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID отзыва должен быть больше 0");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Текст отзыва обязателен для заполнения")
                .MaximumLength(2000).WithMessage("Текст отзыва не должен превышать 2000 символов")
                .MinimumLength(10).WithMessage("Текст отзыва должен содержать минимум 10 символов");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Рейтинг должен быть от 1 до 5");
        }
    }
}
