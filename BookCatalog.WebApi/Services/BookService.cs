using AutoMapper;
using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Interfaces;
using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.ServiceInterfaces;
using BookCatalog.Domain.Entities;
using FluentValidation;

namespace BookCatalog.WebApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBookDto> _createBookValidator;
        private readonly IValidator<UpdateBookDto> _updateBookValidator;
        private readonly IValidator<BookSearchDto> _bookSearchValidator;

        public BookService(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IGenreRepository genreRepository,
            IReviewRepository reviewRepository,
            IMapper mapper,
            IValidator<CreateBookDto> createBookValidator,
            IValidator<UpdateBookDto> updateBookValidator,
            IValidator<BookSearchDto> bookSearchValidator)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _createBookValidator = createBookValidator;
            _updateBookValidator = updateBookValidator;
            _bookSearchValidator = bookSearchValidator;
        }

        // Вспомогательные методы должны быть ПЕРЕД методами интерфейса
        private async Task<BookDto> MapBookToDto(Book book)
        {
            var dto = _mapper.Map<BookDto>(book);
            dto.AuthorName = book.Author?.FullName ?? string.Empty;
            dto.Genres = book.Genres?.Select(g => g.Name).ToList() ?? new List<string>();
            dto.AverageRating = await _reviewRepository.GetAverageRatingAsync(book.Id);
            dto.ReviewsCount = await _reviewRepository.GetReviewsCountAsync(book.Id);
            return dto;
        }

        private async Task<IEnumerable<BookDto>> MapBooksToDtos(IEnumerable<Book> books)
        {
            var dtos = new List<BookDto>();
            foreach (var book in books)
            {
                dtos.Add(await MapBookToDto(book));
            }
            return dtos;
        }

        // Реализация методов интерфейса
        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetBooksWithDetailsAsync();
            return await MapBooksToDtos(books);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Book ID must be greater than 0" } }
                });

            var book = await _bookRepository.GetBookWithDetailsAsync(id);
            if (book == null)
                throw new NotFoundException(nameof(Book), id);

            return await MapBookToDto(book);
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _createBookValidator.ValidateAsync(createBookDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            // Проверка существования автора
            var author = await _authorRepository.GetByIdAsync(createBookDto.AuthorId);
            if (author == null)
                throw new NotFoundException(nameof(Author), createBookDto.AuthorId);

            // Проверка жанров
            if (createBookDto.GenreIds != null && createBookDto.GenreIds.Any())
            {
                var genres = await _genreRepository.GetGenresByIdsAsync(createBookDto.GenreIds);
                if (genres.Count() != createBookDto.GenreIds.Count)
                {
                    var invalidGenreIds = createBookDto.GenreIds.Except(genres.Select(g => g.Id));
                    throw new AppValidationException(new Dictionary<string, string[]>
                    {
                        { "GenreIds", new[] { $"Invalid genre IDs: {string.Join(", ", invalidGenreIds)}" } }
                    });
                }
            }

            var book = _mapper.Map<Book>(createBookDto);
            book.Author = author;

            if (createBookDto.GenreIds != null && createBookDto.GenreIds.Any())
            {
                var genres = await _genreRepository.GetGenresByIdsAsync(createBookDto.GenreIds);
                book.Genres = genres.ToList();
            }

            var createdBook = await _bookRepository.AddAsync(book);
            return await MapBookToDto(createdBook);
        }

        public async Task<BookDto> UpdateBookAsync(UpdateBookDto updateBookDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _updateBookValidator.ValidateAsync(updateBookDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            // Проверка существования книги
            var existingBook = await _bookRepository.GetBookWithDetailsAsync(updateBookDto.Id);
            if (existingBook == null)
                throw new NotFoundException(nameof(Book), updateBookDto.Id);

            // Проверка автора (если изменился)
            if (updateBookDto.AuthorId != existingBook.AuthorId)
            {
                var author = await _authorRepository.GetByIdAsync(updateBookDto.AuthorId);
                if (author == null)
                    throw new NotFoundException(nameof(Author), updateBookDto.AuthorId);
            }

            // Проверка жанров
            if (updateBookDto.GenreIds != null && updateBookDto.GenreIds.Any())
            {
                var genres = await _genreRepository.GetGenresByIdsAsync(updateBookDto.GenreIds);
                if (genres.Count() != updateBookDto.GenreIds.Count)
                {
                    var invalidGenreIds = updateBookDto.GenreIds.Except(genres.Select(g => g.Id));
                    throw new AppValidationException(new Dictionary<string, string[]>
                    {
                        { "GenreIds", new[] { $"Invalid genre IDs: {string.Join(", ", invalidGenreIds)}" } }
                    });
                }
            }

            _mapper.Map(updateBookDto, existingBook);

            // Обновление автора если изменился
            if (updateBookDto.AuthorId != existingBook.AuthorId)
            {
                var author = await _authorRepository.GetByIdAsync(updateBookDto.AuthorId);
                existingBook.Author = author;
            }

            // Обновление жанров
            if (updateBookDto.GenreIds != null && updateBookDto.GenreIds.Any())
            {
                var genres = await _genreRepository.GetGenresByIdsAsync(updateBookDto.GenreIds);
                existingBook.Genres = genres.ToList();
            }

            await _bookRepository.UpdateAsync(existingBook);
            return await MapBookToDto(existingBook);
        }

        public async Task DeleteBookAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Book ID must be greater than 0" } }
                });

            var book = await _bookRepository.GetBookWithDetailsAsync(id);
            if (book == null)
                throw new NotFoundException(nameof(Book), id);

            // Бизнес-правило: нельзя удалить книгу с отзывами
            if (book.Reviews?.Any() == true)
                throw new BusinessRuleException("Cannot delete a book that has reviews");

            await _bookRepository.DeleteAsync(book);
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "searchTerm", new[] { "Search term cannot be empty" } }
                });

            if (searchTerm.Length < 2)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "searchTerm", new[] { "Search term must be at least 2 characters long" } }
                });

            var books = await _bookRepository.SearchBooksAsync(searchTerm);
            return await MapBooksToDtos(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByAuthorNameAsync(string authorName)
        {
            if (string.IsNullOrWhiteSpace(authorName))
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "authorName", new[] { "Author name cannot be empty" } }
                });

            var books = await _bookRepository.GetBooksByAuthorNameAsync(authorName);
            return await MapBooksToDtos(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByGenreNameAsync(string genreName)
        {
            if (string.IsNullOrWhiteSpace(genreName))
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "genreName", new[] { "Genre name cannot be empty" } }
                });

            var books = await _bookRepository.GetBooksByGenreNameAsync(genreName);
            return await MapBooksToDtos(books);
        }

        public async Task<BookSearchResultDto> SearchBooksAdvancedAsync(BookSearchDto searchDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _bookSearchValidator.ValidateAsync(searchDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            var books = await _bookRepository.SearchBooksAdvancedAsync(_mapper.Map<BookSearch>(searchDto));
            var totalCount = books.Count();

            var pagedBooks = books
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            var bookDtos = await MapBooksToDtos(pagedBooks);

            return new BookSearchResultDto
            {
                Books = bookDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize)
            };
        }
    }
}
