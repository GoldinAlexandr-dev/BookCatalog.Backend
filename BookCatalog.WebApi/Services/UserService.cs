using AutoMapper;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.ServiceInterfaces;
using BookCatalog.Domain.Entities;

namespace BookCatalog.WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IReviewRepository reviewRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(user.Id);
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserDetailDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithReviewsAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            return _mapper.Map<UserDetailDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
            if (existingUser != null)
                throw new ArgumentException("Username already exists");

            existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingUser != null)
                throw new ArgumentException("Email already exists");

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = createUserDto.Password; // TODO: Hash password

            var createdUser = await _userRepository.AddAsync(user);
            var userDto = _mapper.Map<UserDto>(createdUser);
            userDto.ReviewsCount = 0;

            return userDto;
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (existingUser == null)
                throw new KeyNotFoundException($"User with ID {updateUserDto.Id} not found");

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser);

            var userDto = _mapper.Map<UserDto>(existingUser);
            userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(existingUser.Id);

            return userDto;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            await _userRepository.DeleteAsync(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new KeyNotFoundException($"User with username {username} not found");

            var userDto = _mapper.Map<UserDto>(user);
            userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(user.Id);

            return userDto;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return false;

            return user.PasswordHash == password; // TODO: Compare hashed passwords
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(user.Id);
                userDtos.Add(userDto);
            }

            return userDtos;
        }
    }
}
