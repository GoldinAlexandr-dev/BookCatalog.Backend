using BookCatalog.Application.Exceptions;
using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDetailDto>> GetAuthor(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto)
        {
            var author = await _authorService.CreateAuthorAsync(createAuthorDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuthorDto>> UpdateAuthor(int id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            // Проверяем соответствие ID в route и в теле запроса
            if (id != updateAuthorDto.Id)
            {
                // Возвращаем ошибку валидации через исключение
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "ID in route does not match ID in request body" } }
                });
            }

            var updatedAuthor = await _authorService.UpdateAuthorAsync(updateAuthorDto);
            return Ok(updatedAuthor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
    }
}
