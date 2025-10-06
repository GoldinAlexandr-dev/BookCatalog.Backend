using BookCatalog.ApplicationServices.ServiceInterfaces;
using BookCatalog.PersistenceServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookCatalog.PersistenceServices
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
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

