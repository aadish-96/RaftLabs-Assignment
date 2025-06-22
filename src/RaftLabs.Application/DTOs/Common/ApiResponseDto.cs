namespace RaftLabs.Application.DTOs.Common
{
    /// <summary>
    /// Data Transfer Object (DTO) for API responses.
    /// </summary>
    /// <typeparam name="T">Generic type parameter for the data contained in the response.</typeparam>
    public class ApiResponseDto<T>
    {
        /// <summary>
        /// Default constructor for Json Serialization and Deserialization.
        /// </summary>
        public ApiResponseDto() { }

        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        /// <param name="data">Data to be expected from the API response</param>
        public ApiResponseDto(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Represents the data returned by the API.
        /// </summary>
        public T? Data { get; set; }
    }
}
