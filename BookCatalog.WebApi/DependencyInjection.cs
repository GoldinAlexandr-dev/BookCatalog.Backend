using BookCatalog.ApplicationServices.ServiceInterfaces;
using BookCatalog.WebApi.Services;

namespace BookCatalog.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
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

