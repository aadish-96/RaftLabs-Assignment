namespace RaftLabs.Application.DTOs.Users
{
    /// <summary>
    /// User Data Transfer Object (DTO) for external components.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        /// <param name="id">A unique ID associated to the specified user.</param>
        /// <param name="email">Email Id associated to the specified user.</param>
        /// <param name="firstName">First name associated to the specified user.</param>
        /// <param name="lastName">Last name associated to the specified user.</param>
        /// <param name="avatar">Avatar (image URL) associated to the specified user.</param>
        public UserDto(int id, string email, string firstName, string lastName, string avatar)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Avatar = avatar;
        }

        /// <summary>
        /// Default constructor for Json Serialization and Deserialization.
        /// </summary>
        public UserDto() { }

        public int Id { get; set; } = 0;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
    }
}
