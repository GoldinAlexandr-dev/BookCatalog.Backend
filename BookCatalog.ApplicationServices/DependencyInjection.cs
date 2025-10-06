using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.Validators.Authors;
using BookCatalog.ApplicationServices.Validators.Books;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookCatalog.ApplicationServices
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Регистрация всех валидаторов из сборки
            services.AddValidatorsFromAssemblyContaining<CreateBookDtoValidator>();

            // ИЛИ явная регистрация каждого валидатора
            services.AddScoped<IValidator<CreateBookDto>, CreateBookDtoValidator>();
            services.AddScoped<IValidator<UpdateBookDto>, UpdateBookDtoValidator>();
            services.AddScoped<IValidator<BookDto>, BookDtoValidator>();
            services.AddScoped<IValidator<BookSearchDto>, BookSearchDtoValidator>();
            // Authors
            services.AddScoped<IValidator<CreateAuthorDto>, CreateAuthorDtoValidator>();
            services.AddScoped<IValidator<UpdateAuthorDto>, UpdateAuthorDtoValidator>();

            return services;
        }
    }
}
