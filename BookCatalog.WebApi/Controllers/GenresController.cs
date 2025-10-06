using BookCatalog.Application.Exceptions;
using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.PersistenceServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDetailDto>> GetGenre(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            return Ok(genre);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<GenreDto>> GetGenreByName(string name)
        {
            var genre = await _genreService.GetGenreByNameAsync(name);
            return Ok(genre);
        }

        [HttpGet("popular/{count}")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetPopularGenres(int count = 5)
        {
            var genres = await _genreService.GetPopularGenresAsync(count);
            return Ok(genres);
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> SearchGenres(string searchTerm)
        {
            var genres = await _genreService.SearchGenresAsync(searchTerm);
            return Ok(genres);
        }

        [HttpGet("{id}/in-use")]
        public async Task<ActionResult<bool>> IsGenreInUse(int id)
        {
            var isInUse = await _genreService.IsGenreInUseAsync(id);
            return Ok(isInUse);
        }

        [HttpPost]
        public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] CreateGenreDto createGenreDto)
        {
            var genre = await _genreService.CreateGenreAsync(createGenreDto);
            return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GenreDto>> UpdateGenre(int id, [FromBody] UpdateGenreDto updateGenreDto)
        {
            if (id != updateGenreDto.Id)
            {
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "ID in route does not match ID in request body" } }
                });
            }

            var updatedGenre = await _genreService.UpdateGenreAsync(updateGenreDto);
            return Ok(updatedGenre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
    }

    //[ApiController]
    //[Route("api/[controller]")]
    //public class GenresController : ControllerBase
    //{
    //    private readonly IGenreService _genreService;
    //    private readonly ILogger<GenresController> _logger;

    //    public GenresController(IGenreService genreService, ILogger<GenresController> logger)
    //    {
    //        _genreService = genreService;
    //        _logger = logger;
    //    }

    //    [HttpGet]
    //    public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
    //    {
    //        try
    //        {
    //            var genres = await _genreService.GetAllGenresAsync();
    //            return Ok(genres);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error occurred while getting genres");
    //            return StatusCode(500, "Internal server error");
    //        }
    //    }

    //    [HttpGet("{id}")]
    //    public async Task<ActionResult<GenreDetailDto>> GetGenre(int id)
    //    {
    //        try
    //        {
    //            var genre = await _genreService.GetGenreByIdAsync(id);
    //            return Ok(genre);
    //        }
    //        catch (KeyNotFoundException ex)
    //        {
    //            return NotFound(ex.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error occurred while getting genre with ID {GenreId}", id);
    //            return StatusCode(500, "Internal server error");
    //        }
    //    }

    //    [HttpGet("popular/{count}")]
    //    public async Task<ActionResult<IEnumerable<GenreDto>>> GetPopularGenres(int count = 5)
    //    {
    //        try
    //        {
    //            var genres = await _genreService.GetPopularGenresAsync(count);
    //            return Ok(genres);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error occurred while getting popular genres");
    //            return StatusCode(500, "Internal server error");
    //        }
    //    }

    //    [HttpPost]
    //    public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto createGenreDto)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //                return BadRequest(ModelState);

    //            var genre = await _genreService.CreateGenreAsync(createGenreDto);
    //            return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    //        }
    //        catch (ArgumentException ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error occurred while creating genre");
    //            return StatusCode(500, "Internal server error");
    //        }
    //    }

    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDto updateGenreDto)
    //    {
    //        try
    //        {
    //            if (id != updateGenreDto.Id)
    //                return BadRequest("ID mismatch");

    //            if (!ModelState.IsValid)
    //                return BadRequest(ModelState);

    //            await _genreService.UpdateGenreAsync(updateGenreDto);
    //            return NoContent();
    //        }
    //        catch (KeyNotFoundException ex)
    //        {
    //            return NotFound(ex.Message);
    //        }
    //        catch (ArgumentException ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error occurred while updating genre with ID {GenreId}", id);
    //            return StatusCode(500, "Internal server error");
    //        }
    //    }

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> DeleteGenre(int id)
    //    {
    //        try
    //        {
    //            await _genreService.DeleteGenreAsync(id);
    //            return NoContent();
    //        }
    //        catch (KeyNotFoundException ex)
    //        {
    //            return NotFound(ex.Message);
    //        }
    //        catch (InvalidOperationException ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error occurred while deleting genre with ID {GenreId}", id);
    //            return StatusCode(500, "Internal server error");
    //        }
    //    }
    //}
}
