using RaftLabs.Domain.Models;

namespace RaftLabs.Domain.Interfaces
{
    /// <summary>
    /// Interface for user-related services.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets a list of all users
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a list of <see cref="User"/> entities.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="User"/> entity.</returns>
        Task<User> GetUserByIdAsync(int userId);
    }
}