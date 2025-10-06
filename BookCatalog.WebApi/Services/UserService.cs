using AutoMapper;
using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Interfaces;
using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.ServiceInterfaces;
using BookCatalog.Domain.Entities;
using FluentValidation;

namespace BookCatalog.WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserDto> _createUserValidator;
        private readonly IValidator<UpdateUserDto> _updateUserValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        public UserService(
            IUserRepository userRepository,
            IReviewRepository reviewRepository,
            IMapper mapper,
            IValidator<CreateUserDto> createUserValidator,
            IValidator<UpdateUserDto> updateUserValidator,
            IValidator<LoginDto> loginValidator)
        {
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
            _loginValidator = loginValidator;
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
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "User ID must be greater than 0" } }
                });

            var user = await _userRepository.GetUserWithReviewsAsync(id);
            if (user == null)
                throw new NotFoundException(nameof(User), id);

            return _mapper.Map<UserDetailDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            // Проверка уникальности username
            var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
            if (existingUser != null)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Username", new[] { "Username already exists" } }
                });

            // Проверка уникальности email
            existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingUser != null)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Email already exists" } }
                });

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = createUserDto.Password; // TODO: Hash password

            var createdUser = await _userRepository.AddAsync(user);
            var userDto = _mapper.Map<UserDto>(createdUser);
            userDto.ReviewsCount = 0;

            return userDto;
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            var existingUser = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (existingUser == null)
                throw new NotFoundException(nameof(User), updateUserDto.Id);

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser);

            var userDto = _mapper.Map<UserDto>(existingUser);
            userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(existingUser.Id);

            return userDto;
        }

        public async Task DeleteUserAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "User ID must be greater than 0" } }
                });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException(nameof(User), id);

            // Бизнес-правило: нельзя удалить пользователя с отзывами
            var reviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(id);
            if (reviewsCount > 0)
                throw new BusinessRuleException("Cannot delete user that has reviews");

            await _userRepository.DeleteAsync(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "username", new[] { "Username cannot be empty" } }
                });

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new NotFoundException(nameof(User), username);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(user.Id);

            return userDto;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return false;

            return user.PasswordHash == password; // TODO: Compare hashed passwords
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "role", new[] { "Role cannot be empty" } }
                });

            if (role != "User" && role != "Admin")
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "role", new[] { "Role must be 'User' or 'Admin'" } }
                });

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

    //public class UserService : IUserService
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly IReviewRepository _reviewRepository;
    //    private readonly IMapper _mapper;

    //    public UserService(IUserRepository userRepository, IReviewRepository reviewRepository, IMapper mapper)
    //    {
    //        _userRepository = userRepository;
    //        _reviewRepository = reviewRepository;
    //        _mapper = mapper;
    //    }

    //    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    //    {
    //        var users = await _userRepository.GetAllAsync();
    //        var userDtos = new List<UserDto>();

    //        foreach (var user in users)
    //        {
    //            var userDto = _mapper.Map<UserDto>(user);
    //            userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(user.Id);
    //            userDtos.Add(userDto);
    //        }

    //        return userDtos;
    //    }

    //    public async Task<UserDetailDto> GetUserByIdAsync(int id)
    //    {
    //        var user = await _userRepository.GetUserWithReviewsAsync(id);
    //        if (user == null)
    //            throw new KeyNotFoundException($"User with ID {id} not found");

    //        return _mapper.Map<UserDetailDto>(user);
    //    }

    //    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    //    {
    //        var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
    //        if (existingUser != null)
    //            throw new ArgumentException("Username already exists");

    //        existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
    //        if (existingUser != null)
    //            throw new ArgumentException("Email already exists");

    //        var user = _mapper.Map<User>(createUserDto);
    //        user.PasswordHash = createUserDto.Password; // TODO: Hash password

    //        var createdUser = await _userRepository.AddAsync(user);
    //        var userDto = _mapper.Map<UserDto>(createdUser);
    //        userDto.ReviewsCount = 0;

    //        return userDto;
    //    }

    //    public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
    //    {
    //        var existingUser = await _userRepository.GetByIdAsync(updateUserDto.Id);
    //        if (existingUser == null)
    //            throw new KeyNotFoundException($"User with ID {updateUserDto.Id} not found");

    //        _mapper.Map(updateUserDto, existingUser);
    //        await _userRepository.UpdateAsync(existingUser);

    //        var userDto = _mapper.Map<UserDto>(existingUser);
    //        userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(existingUser.Id);

    //        return userDto;
    //    }

    //    public async Task DeleteUserAsync(int id)
    //    {
    //        var user = await _userRepository.GetByIdAsync(id);
    //        if (user == null)
    //            throw new KeyNotFoundException($"User with ID {id} not found");

    //        await _userRepository.DeleteAsync(user);
    //    }

    //    public async Task<UserDto> GetUserByUsernameAsync(string username)
    //    {
    //        var user = await _userRepository.GetByUsernameAsync(username);
    //        if (user == null)
    //            throw new KeyNotFoundException($"User with username {username} not found");

    //        var userDto = _mapper.Map<UserDto>(user);
    //        userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(user.Id);

    //        return userDto;
    //    }

    //    public async Task<bool> ValidateUserAsync(string username, string password)
    //    {
    //        var user = await _userRepository.GetByUsernameAsync(username);
    //        if (user == null)
    //            return false;

    //        return user.PasswordHash == password; // TODO: Compare hashed passwords
    //    }

    //    public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
    //    {
    //        var users = await _userRepository.GetUsersByRoleAsync(role);
    //        var userDtos = new List<UserDto>();

    //        foreach (var user in users)
    //        {
    //            var userDto = _mapper.Map<UserDto>(user);
    //            userDto.ReviewsCount = await _reviewRepository.GetReviewsCountByUserAsync(user.Id);
    //            userDtos.Add(userDto);
    //        }

    //        return userDtos;
    //    }
    //}
}
