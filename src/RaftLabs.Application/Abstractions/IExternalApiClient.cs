using RaftLabs.Application.DTOs.Common;
using RaftLabs.Application.DTOs.Users;

namespace RaftLabs.Application.Abstractions
{
    public interface IExternalApiClient
    {
        /// <summary>
        /// Gets a list of users from the external API with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number of the users to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="UsersResponseDto"/> with an <see cref="IEnumerable{UserDto}"/> of users.</returns>
        Task<UsersResponseDto> GetUsersAsync(int pageNumber);

        /// <summary>
        /// Gets the specific user by their ID from the external API.
        /// </summary>
        /// <param name="userId">The ID of the specific user.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="ApiResponseDto{UserDto}"/> of the specified user.</returns>
        Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId);
    }
}