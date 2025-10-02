using AutoMapper;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.ServiceInterfaces;
using BookCatalog.Domain.Entities;

namespace BookCatalog.WebApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAuthorsWithBooksAsync();
            var authorDtos = new List<AuthorDto>();

            foreach (var author in authors)
            {
                var authorDto = _mapper.Map<AuthorDto>(author);
                authorDto.BooksCount = author.Books?.Count ?? 0;
                authorDtos.Add(authorDto);
            }

            return authorDtos;
        }

        public async Task<AuthorDetailDto> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepository.GetAuthorWithDetailsAsync(id);
            if (author == null)
                throw new KeyNotFoundException($"Author with ID {id} not found");

            return _mapper.Map<AuthorDetailDto>(author);
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            var author = _mapper.Map<Author>(createAuthorDto);
            var createdAuthor = await _authorRepository.AddAsync(author);

            var authorDto = _mapper.Map<AuthorDto>(createdAuthor);
            authorDto.BooksCount = 0;

            return authorDto;
        }

        public async Task<AuthorDto> UpdateAuthorAsync(AuthorDto authorDto)
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(authorDto.Id);
            if (existingAuthor == null)
                throw new KeyNotFoundException($"Author with ID {authorDto.Id} not found");

            _mapper.Map(authorDto, existingAuthor);
            await _authorRepository.UpdateAsync(existingAuthor);

            var updatedAuthorDto = _mapper.Map<AuthorDto>(existingAuthor);
            updatedAuthorDto.BooksCount = existingAuthor.Books?.Count ?? 0;

            return updatedAuthorDto;
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
                throw new KeyNotFoundException($"Author with ID {id} not found");

            if (author.Books?.Any() == true)
                throw new InvalidOperationException("Cannot delete author with existing books");

            await _authorRepository.DeleteAsync(author);
        }
    }
}
