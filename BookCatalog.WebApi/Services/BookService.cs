using AutoMapper;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.ServiceInterfaces;
using BookCatalog.Domain.Entities;

namespace BookCatalog.WebApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public BookService(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IGenreRepository genreRepository,
            IReviewRepository reviewRepository,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

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

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetBooksWithDetailsAsync();
            return await MapBooksToDtos(books);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetBookWithDetailsAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found");
            return await MapBookToDto(book);
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            var author = await _authorRepository.GetByIdAsync(createBookDto.AuthorId);
            if (author == null)
                throw new ArgumentException($"Author with ID {createBookDto.AuthorId} not found");

            var genres = await _genreRepository.GetGenresByIdsAsync(createBookDto.GenreIds);
            if (genres.Count() != createBookDto.GenreIds.Count)
                throw new ArgumentException("One or more genre IDs are invalid");

            var book = _mapper.Map<Book>(createBookDto);
            book.Author = author;
            book.Genres = genres.ToList();

            var createdBook = await _bookRepository.AddAsync(book);
            return await MapBookToDto(createdBook);
        }

        public async Task<BookDto> UpdateBookAsync(UpdateBookDto updateBookDto)
        {
            var existingBook = await _bookRepository.GetBookWithDetailsAsync(updateBookDto.Id);
            if (existingBook == null)
                throw new KeyNotFoundException($"Book with ID {updateBookDto.Id} not found");

            _mapper.Map(updateBookDto, existingBook);

            if (updateBookDto.GenreIds.Any())
            {
                var genres = await _genreRepository.GetGenresByIdsAsync(updateBookDto.GenreIds);
                existingBook.Genres = genres.ToList();
            }

            await _bookRepository.UpdateAsync(existingBook);
            return await MapBookToDto(existingBook);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found");
            await _bookRepository.DeleteAsync(book);
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            var books = await _bookRepository.SearchBooksAsync(searchTerm);
            return await MapBooksToDtos(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByAuthorNameAsync(string authorName)
        {
            var books = await _bookRepository.GetBooksByAuthorNameAsync(authorName);
            return await MapBooksToDtos(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByGenreNameAsync(string genreName)
        {
            var books = await _bookRepository.GetBooksByGenreNameAsync(genreName);
            return await MapBooksToDtos(books);
        }

        public async Task<BookSearchResultDto> SearchBooksAdvancedAsync(BookSearchDto searchDto)
        {
            var books = await _bookRepository.SearchBooksAdvancedAsync(searchDto);
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
