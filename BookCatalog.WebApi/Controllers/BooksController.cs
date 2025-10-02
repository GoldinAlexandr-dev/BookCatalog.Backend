using BookCatalog.Application.DTOs;
using BookCatalog.Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all books");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                return Ok(book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting book with ID {BookId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            try
            {
                var book = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (id != updateBookDto.Id)
                return BadRequest("ID mismatch");

            try
            {
                await _bookService.UpdateBookAsync(updateBookDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book with ID {BookId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID {BookId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/books/search?searchTerm=война
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string searchTerm)
        {
            try
            {
                var books = await _bookService.SearchBooksAsync(searchTerm);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books with term {SearchTerm}", searchTerm);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/books/by-author?authorName=Толстой
        [HttpGet("by-author")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByAuthor([FromQuery] string authorName)
        {
            try
            {
                var books = await _bookService.GetBooksByAuthorNameAsync(authorName);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting books by author {AuthorName}", authorName);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/books/by-genre?genreName=Фантастика
        [HttpGet("by-genre")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByGenre([FromQuery] string genreName)
        {
            try
            {
                var books = await _bookService.GetBooksByGenreNameAsync(genreName);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting books by genre {GenreName}", genreName);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/books/advanced-search?searchTerm=война&authorName=Толстой&genreName=Роман&minYear=1800&maxYear=1900
        [HttpGet("advanced-search")]
        public async Task<ActionResult<BookSearchResultDto>> AdvancedSearch([FromQuery] BookSearchDto searchDto)
        {
            try
            {
                var result = await _bookService.SearchBooksAdvancedAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing advanced search");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
