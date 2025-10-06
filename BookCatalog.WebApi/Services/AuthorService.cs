using AutoMapper;
using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Interfaces;
using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.ServiceInterfaces;
using BookCatalog.Domain.Entities;
using FluentValidation;

namespace BookCatalog.WebApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateAuthorDto> _createAuthorValidator;
        private readonly IValidator<UpdateAuthorDto> _updateAuthorValidator;

        public AuthorService(
            IAuthorRepository authorRepository,
            IBookRepository bookRepository,
            IMapper mapper,
            IValidator<CreateAuthorDto> createAuthorValidator,
            IValidator<UpdateAuthorDto> updateAuthorValidator)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _createAuthorValidator = createAuthorValidator;
            _updateAuthorValidator = updateAuthorValidator;
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
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Author ID must be greater than 0" } }
                });

            var author = await _authorRepository.GetAuthorWithDetailsAsync(id);
            if (author == null)
                throw new NotFoundException(nameof(Author), id);

            return _mapper.Map<AuthorDetailDto>(author);
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _createAuthorValidator.ValidateAsync(createAuthorDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            var author = _mapper.Map<Author>(createAuthorDto);
            var createdAuthor = await _authorRepository.AddAsync(author);

            var authorDto = _mapper.Map<AuthorDto>(createdAuthor);
            authorDto.BooksCount = 0;

            return authorDto;
        }

        public async Task<AuthorDto> UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _updateAuthorValidator.ValidateAsync(updateAuthorDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            var existingAuthor = await _authorRepository.GetByIdAsync(updateAuthorDto.Id);
            if (existingAuthor == null)
                throw new NotFoundException(nameof(Author), updateAuthorDto.Id);

            _mapper.Map(updateAuthorDto, existingAuthor);
            await _authorRepository.UpdateAsync(existingAuthor);

            var updatedAuthorDto = _mapper.Map<AuthorDto>(existingAuthor);
            updatedAuthorDto.BooksCount = existingAuthor.Books?.Count ?? 0;

            return updatedAuthorDto;
        }

        public async Task DeleteAuthorAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Author ID must be greater than 0" } }
                });

            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
                throw new NotFoundException(nameof(Author), id);

            if (author.Books?.Any() == true)
                throw new BusinessRuleException("Cannot delete author with existing books");

            await _authorRepository.DeleteAsync(author);
        }
    }
}
