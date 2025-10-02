using BookCatalog.Application.DTOs;
using BookCatalog.Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with ID {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with username {Username}", username);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string role)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users by role {Role}", role);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("with-reviews")]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUsersWithReviews()
        {
            try
            {
                // Этот метод можно добавить в сервис, если нужен
                var users = await _userService.GetAllUsersAsync();
                var usersWithReviews = new List<UserDetailDto>();

                foreach (var user in users)
                {
                    var userDetail = await _userService.GetUserByIdAsync(user.Id);
                    usersWithReviews.Add(userDetail);
                }

                return Ok(usersWithReviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users with reviews");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterUser(CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var isValid = await _userService.ValidateUserAsync(loginDto.Username, loginDto.Password);
                if (!isValid)
                    return Unauthorized("Invalid username or password");

                var user = await _userService.GetUserByUsernameAsync(loginDto.Username);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user {Username}", loginDto.Username);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("validate-credentials")]
        public async Task<ActionResult<bool>> ValidateCredentials(LoginDto loginDto)
        {
            try
            {
                var isValid = await _userService.ValidateUserAsync(loginDto.Username, loginDto.Password);
                return Ok(isValid);
            }
            catch (KeyNotFoundException)
            {
                return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating credentials for user {Username}", loginDto.Username);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("check-username/{username}")]
        public async Task<ActionResult<bool>> CheckUsernameExists(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                return Ok(true); // Если пользователь найден
            }
            catch (KeyNotFoundException)
            {
                return Ok(false); // Если пользователь не найден
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking username {Username}", username);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                if (id != updateUserDto.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userService.UpdateUserAsync(updateUserDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/role/{newRole}")]
        public async Task<IActionResult> UpdateUserRole(int id, string newRole)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                var updateUserDto = new UpdateUserDto
                {
                    Id = id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = newRole
                };

                await _userService.UpdateUserAsync(updateUserDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role for user with ID {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
