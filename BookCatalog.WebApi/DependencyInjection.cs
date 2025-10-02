using BookCatalog.Application.ServiceInterfaces;
using BookCatalog.Application.Services;
using BookCatalog.WebApi.Services;

namespace BookCatalog.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Регистрация сервисов
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}

