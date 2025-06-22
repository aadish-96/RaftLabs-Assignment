using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using RaftLabs.Application.Abstractions;
using RaftLabs.Application.DTOs.Common;
using RaftLabs.Application.DTOs.Users;
using RaftLabs.Domain.Models;
using RaftLabs.Infrastructure.Constants;
using RaftLabs.Infrastructure.Exceptions;
using RaftLabs.Infrastructure.Mappings;
using RaftLabs.Infrastructure.Services;

namespace RaftLabs.Tests.Users
{
    /// <summary>
    /// Unit tests for the ExternalUserService which validates caching, API interactions and AutoMapper configuration.
    /// </summary>
    public class ExternalUserServiceUnitTests
    {
        private readonly Mock<IExternalApiClient> _externalApiClientMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
        private readonly IMapper _mapper = new MapperConfiguration(_ => _.AddProfile<MappingProfile>()).CreateMapper();
        private readonly UserService _service;
        private readonly User _firstPageUser = new(1, "aadish.shah@raftlabs.com", "Aadish", "Shah", "<avatar_image_path_page_1>");
        private readonly User _lastPageUser = new(2, "shah.aadish@raftlabs.com", "Shah", "Aadish", "<avatar_image_path_page_2>");
        private readonly UserDto _firstPageUserDto = new(1, "aadish.shah@raftlabs.com", "Aadish", "Shah", "<avatar_image_path_page_1>");
        private readonly UserDto _lastPageUserDto = new(2, "shah.aadish@raftlabs.com", "Shah", "Aadish", "<avatar_image_path_page_2>");
        private readonly string _firstUserIdCacheKey;

        /// <summary>
        /// Constructor that initializes mocks, mapper and the UserService under test.
        /// </summary>
        public ExternalUserServiceUnitTests()
        {
            _externalApiClientMock = new Mock<IExternalApiClient>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _service = new(_externalApiClientMock.Object, _loggerMock.Object, _memoryCache, _mapper);
            _firstUserIdCacheKey = string.Format(CacheKeys.UserId, _firstPageUser.Id);
        }

        /// <summary>
        /// Ensures that AutoMapper configuration is valid for all registered mappings.
        /// </summary>
        [Fact]
        public void AutoMapper_Configuration_IsValid() => Assert.Null(Record.Exception(_mapper.ConfigurationProvider.AssertConfigurationIsValid));

        /// <summary>
        /// Validates that AutoMapper can map from UserDto to User correctly.
        /// </summary>
        [Fact]
        public void AutoMapper_Maps_UserDto_To_User()
        {
            User user = _mapper.Map<UserDto, User>(_firstPageUserDto);
            Assert.Equal(_firstPageUserDto.Id, user.Id);
            Assert.Equal(_firstPageUserDto.Email, user.Email);
            Assert.Equal(_firstPageUserDto.FirstName, user.FirstName);
            Assert.Equal(_firstPageUserDto.LastName, user.LastName);
            Assert.Equal(_firstPageUserDto.Avatar, user.Avatar);
        }

        /// <summary>
        /// Validates that AutoMapper can map from User to UserDto correctly.
        /// </summary>
        [Fact]
        public void AutoMapper_Maps_User_To_UserDto()
        {
            UserDto userDto = _mapper.Map<User, UserDto>(_firstPageUser);
            Assert.Equal(_firstPageUser.Id, userDto.Id);
            Assert.Equal(_firstPageUser.Email, userDto.Email);
            Assert.Equal(_firstPageUser.FirstName, userDto.FirstName);
            Assert.Equal(_firstPageUser.LastName, userDto.LastName);
            Assert.Equal(_firstPageUser.Avatar, userDto.Avatar);
        }

        /// <summary>
        /// Ensures the user is returned from memory cache without hitting the external API.
        /// </summary>
        [Fact]
        public async Task GetUserByIdAsync_Returns_User_From_Cache()
        {
            // Arrange: Set up the memory cache to return the userDetail when queried with the key "user_1".
            _memoryCache.Set(_firstUserIdCacheKey, _firstPageUser);

            // Act: Call GetUserByIdAsync with ID 1.
            User result = await _service.GetUserByIdAsync(_firstPageUser.Id);

            // Assert: Confirm result was returned from cache.
            Assert.Equal(_firstPageUser, result);

            // Assert: Ensure no API call was made.
            _externalApiClientMock.Verify(_ => _.GetUserByIdAsync(It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// Ensures the external API is called and result is cached when user is not found in memory.
        /// </summary>
        [Fact]
        public async Task GetUserByIdAsync_Calls_Api_And_Caches_Result()
        {
            // Arrange: Set up the external API client to return the userDetail when passing the appropriate Id.
            _externalApiClientMock.Setup(_ => _.GetUserByIdAsync(_firstPageUser.Id)).ReturnsAsync(new ApiResponseDto<UserDto>(_firstPageUserDto));

            // Act: Call GetUserByIdAsync with ID 1.
            User result = await _service.GetUserByIdAsync(_firstPageUser.Id);

            // Assert: Result should match the DTO returned.
            Assert.Equal(_firstPageUser.Id, result.Id);

            // Assert: Confirm result is cached.
            Assert.True(_memoryCache.TryGetValue(_firstUserIdCacheKey, out _));
        }

        /// <summary>
        /// Ensures cached user list is returned and API is not called.
        /// </summary>
        [Fact]
        public async Task GetAllUsersAsync_Returns_From_Cache()
        {
            // Arrange: Insert cached users.
            List<User> users = [_firstPageUser, _lastPageUser];
            _memoryCache.Set(CacheKeys.Users, users);

            // Act: Call all users.
            List<User> result = await _service.GetAllUsersAsync();

            // Assert: Validate data source and cache presence.
            Assert.Equal<User>(users, result);

            // Assert: External API should not be called.
            _externalApiClientMock.Verify(_ => _.GetUsersAsync(It.IsAny<int>()), Times.Never);

            // Assert: Cache should still hold the users.
            Assert.True(_memoryCache.TryGetValue(CacheKeys.Users, out _));
        }

        /// <summary>
        /// Ensures service correctly aggregates paginated user data from external API.
        /// </summary>
        [Fact]
        public async Task GetAllUsersAsync_Fetches_Paginated_Data()
        {
            // Arrange: Set up paginated API response.
            UsersResponseDto[] pages = [new UsersResponseDto([_firstPageUserDto], 1, 2, 1, 2), new UsersResponseDto([_lastPageUserDto], 2, 2, 1, 2)];

            // Arrange: Setup the external API to return the first page response when queried with the first user ID
            _externalApiClientMock.Setup(_ => _.GetUsersAsync(_firstPageUserDto.Id)).ReturnsAsync(pages.First());

            // Arrange: Setup the external API to return the last page response when queried with the last user ID
            _externalApiClientMock.Setup(_ => _.GetUsersAsync(_lastPageUserDto.Id)).ReturnsAsync(pages.Last());

            // Act: Fetch all users across pages.
            List<User> result = await _service.GetAllUsersAsync();

            // Assert: Should contain combined users from all pages.
            Assert.Equal(pages.Length, result.Count);
        }

        /// <summary>
        /// Validates that proper exceptions are thrown when external API fails.
        /// </summary>
        [Fact]
        public async Task GetUserByIdAsync_Throws_API_Exception()
        {
            // Arrange: Configure the external API client to throw for both IDs.
            _externalApiClientMock.Setup(_ => _.GetUserByIdAsync(_firstPageUserDto.Id)).ThrowsAsync(new UserNotFoundException(_firstPageUserDto.Id));
            _externalApiClientMock.Setup(_ => _.GetUserByIdAsync(_lastPageUserDto.Id)).ThrowsAsync(new ExternalServiceException($"API error testing for id: {_lastPageUserDto.Id}"));

            // Assert: Verify exceptions are properly thrown.
            await Assert.ThrowsAsync<UserNotFoundException>(() => _service.GetUserByIdAsync(_firstPageUserDto.Id));
            await Assert.ThrowsAsync<ExternalServiceException>(() => _service.GetUserByIdAsync(_lastPageUserDto.Id));
        }
    }
}
