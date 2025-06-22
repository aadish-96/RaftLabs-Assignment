namespace RaftLabs.Domain.Models
{
    /// <summary>
    /// User model representing a user in the system.
    /// </summary>
    /// <param name="id">Unique identifier for the user.</param>
    /// <param name="email">Email address of the user.</param>
    /// <param name="firstName">First name of the user.</param>
    /// <param name="lastName">Last name of the user.</param>
    /// <param name="avatar">Avatar URL of the user.</param>
    public class User(int id, string email, string firstName, string lastName, string avatar)
    {
        public int Id { get; set; } = id;
        public string Email { get; set; } = email;
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string Avatar { get; set; } = avatar;
    }
}
