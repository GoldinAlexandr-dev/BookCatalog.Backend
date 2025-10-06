using AutoMapper;
using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Interfaces;
using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.ServiceInterfaces;
using BookCatalog.Domain.Entities;
using FluentValidation;

namespace BookCatalog.PersistenceServices.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateGenreDto> _createGenreValidator;
        private readonly IValidator<UpdateGenreDto> _updateGenreValidator;

        public GenreService(
            IGenreRepository genreRepository,
            IMapper mapper,
            IValidator<CreateGenreDto> createGenreValidator,
            IValidator<UpdateGenreDto> updateGenreValidator)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
            _createGenreValidator = createGenreValidator;
            _updateGenreValidator = updateGenreValidator;
        }

        public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            var genreDtos = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreDto = _mapper.Map<GenreDto>(genre);
                genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(genre.Id);
                genreDtos.Add(genreDto);
            }

            return genreDtos.OrderBy(g => g.Name);
        }

        public async Task<GenreDetailDto> GetGenreByIdAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Genre ID must be greater than 0" } }
                });

            var genre = await _genreRepository.GetGenreWithBooksAsync(id);
            if (genre == null)
                throw new NotFoundException(nameof(Genre), id);

            return _mapper.Map<GenreDetailDto>(genre);
        }

        public async Task<GenreDto> CreateGenreAsync(CreateGenreDto createGenreDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _createGenreValidator.ValidateAsync(createGenreDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            // Проверяем уникальность имени
            var existingGenre = await _genreRepository.GetByNameAsync(createGenreDto.Name);
            if (existingGenre != null)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "Genre with this name already exists" } }
                });

            var genre = _mapper.Map<Genre>(createGenreDto);
            var createdGenre = await _genreRepository.AddAsync(genre);

            var genreDto = _mapper.Map<GenreDto>(createdGenre);
            genreDto.BooksCount = 0;

            return genreDto;
        }

        public async Task<GenreDto> UpdateGenreAsync(UpdateGenreDto updateGenreDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _updateGenreValidator.ValidateAsync(updateGenreDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            var existingGenre = await _genreRepository.GetByIdAsync(updateGenreDto.Id);
            if (existingGenre == null)
                throw new NotFoundException(nameof(Genre), updateGenreDto.Id);

            // Проверяем уникальность имени
            var genreWithSameName = await _genreRepository.GetByNameAsync(updateGenreDto.Name);
            if (genreWithSameName != null && genreWithSameName.Id != updateGenreDto.Id)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "Genre with this name already exists" } }
                });

            _mapper.Map(updateGenreDto, existingGenre);
            await _genreRepository.UpdateAsync(existingGenre);

            var genreDto = _mapper.Map<GenreDto>(existingGenre);
            genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(existingGenre.Id);

            return genreDto;
        }

        public async Task DeleteGenreAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Genre ID must be greater than 0" } }
                });

            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                throw new NotFoundException(nameof(Genre), id);

            if (await _genreRepository.IsGenreInUseAsync(id))
                throw new BusinessRuleException("Cannot delete genre that is used by books");

            await _genreRepository.DeleteAsync(genre);
        }

        public async Task<bool> IsGenreInUseAsync(int genreId)
        {
            return await _genreRepository.IsGenreInUseAsync(genreId);
        }

        public async Task<IEnumerable<GenreDto>> GetPopularGenresAsync(int count)
        {
            if (count <= 0 || count > 20)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "count", new[] { "Count must be between 1 and 20" } }
                });

            var genres = await _genreRepository.GetPopularGenresAsync(count);
            var genreDtos = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreDto = _mapper.Map<GenreDto>(genre);
                genreDto.BooksCount = genre.Books?.Count ?? 0;
                genreDtos.Add(genreDto);
            }

            return genreDtos;
        }

        public async Task<GenreDto> GetGenreByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "name", new[] { "Genre name cannot be empty" } }
                });

            var genre = await _genreRepository.GetByNameAsync(name);
            if (genre == null)
                throw new NotFoundException(nameof(Genre), name);

            var genreDto = _mapper.Map<GenreDto>(genre);
            genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(genre.Id);

            return genreDto;
        }

        public async Task<IEnumerable<GenreDto>> SearchGenresAsync(string searchTerm)
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

            var genres = await _genreRepository.SearchGenresAsync(searchTerm);
            var genreDtos = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreDto = _mapper.Map<GenreDto>(genre);
                genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(genre.Id);
                genreDtos.Add(genreDto);
            }

            return genreDtos;
        }
    }


    //public class GenreService : IGenreService
    //{
    //    private readonly IGenreRepository _genreRepository;
    //    private readonly IMapper _mapper;

    //    public GenreService(IGenreRepository genreRepository, IMapper mapper)
    //    {
    //        _genreRepository = genreRepository;
    //        _mapper = mapper;
    //    }

    //    public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
    //    {
    //        var genres = await _genreRepository.GetAllAsync();
    //        var genreDtos = new List<GenreDto>();

    //        foreach (var genre in genres)
    //        {
    //            var genreDto = _mapper.Map<GenreDto>(genre);
    //            genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(genre.Id);
    //            genreDtos.Add(genreDto);
    //        }

    //        return genreDtos.OrderBy(g => g.Name);
    //    }

    //    public async Task<GenreDetailDto> GetGenreByIdAsync(int id)
    //    {
    //        var genre = await _genreRepository.GetGenreWithBooksAsync(id);
    //        if (genre == null)
    //            throw new KeyNotFoundException($"Genre with ID {id} not found");

    //        return _mapper.Map<GenreDetailDto>(genre);
    //    }

    //    public async Task<GenreDto> CreateGenreAsync(CreateGenreDto createGenreDto)
    //    {
    //        // Проверяем уникальность имени
    //        var existingGenre = await _genreRepository.GetByNameAsync(createGenreDto.Name);
    //        if (existingGenre != null)
    //            throw new ArgumentException("Genre with this name already exists");

    //        var genre = _mapper.Map<Genre>(createGenreDto);
    //        var createdGenre = await _genreRepository.AddAsync(genre);

    //        var genreDto = _mapper.Map<GenreDto>(createdGenre);
    //        genreDto.BooksCount = 0;

    //        return genreDto;
    //    }

    //    public async Task<GenreDto> UpdateGenreAsync(UpdateGenreDto updateGenreDto)
    //    {
    //        var existingGenre = await _genreRepository.GetByIdAsync(updateGenreDto.Id);
    //        if (existingGenre == null)
    //            throw new KeyNotFoundException($"Genre with ID {updateGenreDto.Id} not found");

    //        // Проверяем уникальность имени
    //        var genreWithSameName = await _genreRepository.GetByNameAsync(updateGenreDto.Name);
    //        if (genreWithSameName != null && genreWithSameName.Id != updateGenreDto.Id)
    //            throw new ArgumentException("Genre with this name already exists");

    //        _mapper.Map(updateGenreDto, existingGenre);
    //        await _genreRepository.UpdateAsync(existingGenre);

    //        var genreDto = _mapper.Map<GenreDto>(existingGenre);
    //        genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(existingGenre.Id);

    //        return genreDto;
    //    }

    //    public async Task DeleteGenreAsync(int id)
    //    {
    //        var genre = await _genreRepository.GetByIdAsync(id);
    //        if (genre == null)
    //            throw new KeyNotFoundException($"Genre with ID {id} not found");

    //        if (await _genreRepository.IsGenreInUseAsync(id))
    //            throw new InvalidOperationException("Cannot delete genre that is used by books");

    //        await _genreRepository.DeleteAsync(genre);
    //    }

    //    public async Task<bool> IsGenreInUseAsync(int genreId)
    //    {
    //        return await _genreRepository.IsGenreInUseAsync(genreId);
    //    }

    //    public async Task<IEnumerable<GenreDto>> GetPopularGenresAsync(int count)
    //    {
    //        var genres = await _genreRepository.GetPopularGenresAsync(count);
    //        var genreDtos = new List<GenreDto>();

    //        foreach (var genre in genres)
    //        {
    //            var genreDto = _mapper.Map<GenreDto>(genre);
    //            genreDto.BooksCount = genre.Books?.Count ?? 0;
    //            genreDtos.Add(genreDto);
    //        }

    //        return genreDtos;
    //    }

    //    public async Task<GenreDto> GetGenreByNameAsync(string name)
    //    {
    //        var genre = await _genreRepository.GetByNameAsync(name);
    //        if (genre == null)
    //            throw new KeyNotFoundException($"Genre with name '{name}' not found");

    //        var genreDto = _mapper.Map<GenreDto>(genre);
    //        genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(genre.Id);

    //        return genreDto;
    //    }

    //    public async Task<IEnumerable<GenreDto>> SearchGenresAsync(string searchTerm)
    //    {
    //        var genres = await _genreRepository.SearchGenresAsync(searchTerm);
    //        var genreDtos = new List<GenreDto>();

    //        foreach (var genre in genres)
    //        {
    //            var genreDto = _mapper.Map<GenreDto>(genre);
    //            genreDto.BooksCount = await _genreRepository.GetBooksCountAsync(genre.Id);
    //            genreDtos.Add(genreDto);
    //        }

    //        return genreDtos;
    //    }
    //}
}
