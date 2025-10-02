using BookCatalog.Domain.Entities;

namespace BookCatalog.Persistence.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Создаем БД, если её ещё нет (для разработки)
            context.Database.EnsureCreated();

            // Проверяем, есть ли уже данные в БД
            if (context.Books.Any() || context.Authors.Any() || context.Genres.Any())
            {
                return; // БД уже инициализирована
            }

            // Создаем жанры
            var genres = new List<Genre>
            {
                new Genre { Name = "Фантастика" },
                new Genre { Name = "Детектив" },
                new Genre { Name = "Роман" },
                new Genre { Name = "Фэнтези" },
                new Genre { Name = "Приключения" },
                new Genre { Name = "Научная литература" },
                new Genre { Name = "Биография" }
            };

            context.Genres.AddRange(genres);
            context.SaveChanges();

            // Создаем авторов
            var authors = new List<Author>
            {
                new Author
                {
                    FullName = "Айзек Азимов",
                    Biography = "Американский писатель-фантаст, популяризатор науки."
                },
                new Author
                {
                    FullName = "Артур Конан Дойл",
                    Biography = "Шотландский писатель, создатель Шерлока Холмса."
                },
                new Author
                {
                    FullName = "Лев Толстой",
                    Biography = "Русский писатель, мыслитель, философ."
                },
                new Author
                {
                    FullName = "Джон Толкин",
                    Biography = "Английский писатель, лингвист, филолог."
                }
            };

            context.Authors.AddRange(authors);
            context.SaveChanges();

            // Создаем книги
            var books = new List<Book>
            {
                new Book
                {
                    Title = "Я, робот",
                    Description = "Сборник научно-фантастических рассказов о роботах.",
                    PublicationYear = 1950,
                    CoverImageUrl = "/images/i-robot.jpg",
                    AuthorId = authors[0].Id, // Айзек Азимов
                    Genres = new List<Genre> { genres[0], genres[4] } // Фантастика, Приключения
                },
                new Book
                {
                    Title = "Приключения Шерлока Холмса",
                    Description = "Сборник детективных рассказов о знаменитом сыщике.",
                    PublicationYear = 1892,
                    CoverImageUrl = "/images/sherlock-holmes.jpg",
                    AuthorId = authors[1].Id, // Артур Конан Дойл
                    Genres = new List<Genre> { genres[1] } // Детектив
                },
                new Book
                {
                    Title = "Война и мир",
                    Description = "Роман-эпопея, описывающий русское общество в эпоху войн против Наполеона.",
                    PublicationYear = 1869,
                    CoverImageUrl = "/images/war-and-peace.jpg",
                    AuthorId = authors[2].Id, // Лев Толстой
                    Genres = new List<Genre> { genres[2], genres[5] } // Роман, Научная литература
                },
                new Book
                {
                    Title = "Властелин колец",
                    Description = "Эпическая фэнтезийная трилогия о Средиземье.",
                    PublicationYear = 1954,
                    CoverImageUrl = "/images/lord-of-rings.jpg",
                    AuthorId = authors[3].Id, // Джон Толкин
                    Genres = new List<Genre> { genres[3], genres[4] } // Фэнтези, Приключения
                },
                new Book
                {
                    Title = "Основание",
                    Description = "Цикл научно-фантастических романов о Галактической империи.",
                    PublicationYear = 1951,
                    CoverImageUrl = "/images/foundation.jpg",
                    AuthorId = authors[0].Id, // Айзек Азимов
                    Genres = new List<Genre> { genres[0], genres[5] } // Фантастика, Научная литература
                }
            };

            context.Books.AddRange(books);
            context.SaveChanges();

            // Создаем тестовых пользователей
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@bookcatalog.ru",
                    PasswordHash = "admin123", // В реальном приложении - хешировать!
                    Role = "Admin"
                },
                new User
                {
                    Username = "reader",
                    Email = "reader@bookcatalog.ru",
                    PasswordHash = "reader123",
                    Role = "User"
                },
                new User
                {
                    Username = "reviewer",
                    Email = "reviewer@bookcatalog.ru",
                    PasswordHash = "reviewer123",
                    Role = "User"
                }
            };
            context.Users.AddRange(users);
            context.SaveChanges();


            // Создаем несколько отзывов
            var reviews = new List<Review>
            {
            new Review
            {
                Text = "Отличная книга! Очень понравились рассуждения о трех законах робототехники.",
                Rating = 5,
                BookId = books[0].Id,  // Я, робот
                UserId = users[0].Id,  // admin
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Text = "Классика детективного жанра. Шерлок Холмс - гениальный персонаж.",
                Rating = 4,
                BookId = books[1].Id,  // Приключения Шерлока Холмса
                UserId = users[1].Id,  // reader
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Review
            {
                Text = "Эпическое произведение! Масштабно и глубоко.",
                Rating = 5,
                BookId = books[2].Id,  // Война и мир  
                UserId = users[2].Id,  // reviewer
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
            };

            context.Reviews.AddRange(reviews);
            context.SaveChanges();
        }
    }

}
