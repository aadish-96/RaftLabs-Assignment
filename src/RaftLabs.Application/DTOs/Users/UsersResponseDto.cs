using RaftLabs.Application.DTOs.Common;

namespace RaftLabs.Application.DTOs.Users
{
    /// <summary>
    /// Data Transfer Object (DTO) for a paginated response of users from an external component.
    /// </summary>
    public class UsersResponseDto : ApiResponseDto<IEnumerable<UserDto>>
    {
        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        /// <param name="userDtos">List of the user records to be populated in the API response.</param>
        /// <param name="page">Page number of the API response.</param>
        /// <param name="totalPages">Total number of pages available in the external component.</param>
        /// <param name="perPage">Total number of user records per page in the response.</param>
        /// <param name="total">Total number of user records available in the external component.</param>
        public UsersResponseDto(IEnumerable<UserDto> userDtos, int page = 0, int totalPages = 0, int perPage = 0, int total = 0) : base(userDtos)
        {
            Page = page;
            TotalPages = totalPages;
            PerPage = perPage;
            Total = total;
        }

        /// <summary>
        /// Default constructor for Json Serialization and Deserialization.
        /// </summary>
        public UsersResponseDto() { }

        /// <summary>
        /// Gets or sets the page number of the response.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available in the external component.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total number of user records per page in the response.
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of user records available in the external component.
        /// </summary>
        public int Total { get; set; }
    }
}
