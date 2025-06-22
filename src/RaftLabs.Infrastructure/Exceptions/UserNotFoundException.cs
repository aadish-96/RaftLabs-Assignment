namespace RaftLabs.Infrastructure.Exceptions
{
    /// <summary>
    /// Used to indicate that a user with the specified ID was not found in the system.
    /// </summary>
    /// <param name="userId">The ID of the user that was not found.</param>
    public class UserNotFoundException(int userId) : Exception($"User with ID {userId} not found.")
    {
    }
}
