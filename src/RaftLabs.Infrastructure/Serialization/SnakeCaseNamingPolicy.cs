using System.Text;
using System.Text.Json;

namespace RaftLabs.Infrastructure.Serialization
{
    /// <summary>
    /// Policy for converting JSON properties from PascalCase to snake_case
    /// </summary>
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        /// <summary>
        /// Overrides the default behavior to consider the snake_case instead of PascalCase
        /// </summary>
        /// <param name="jsonPropertyNameInPascalCase">JSON property name in PascalCase</param>
        /// <returns>Field name converted into snake_case version for the specified name</returns>
        public override string ConvertName(string jsonPropertyNameInPascalCase)
        {
            // Returns name as it is if it's empty, null or whitespace
            if (string.IsNullOrWhiteSpace(jsonPropertyNameInPascalCase))
                return jsonPropertyNameInPascalCase;

            // Instantiate a new string builder to store the name into snake_case conversion
            StringBuilder snake = new ();

            // Iterate over each characters to convert into the appropriate name
            for (int i = 0; i < jsonPropertyNameInPascalCase.Length; i++)
            {
                char c = jsonPropertyNameInPascalCase[i];

                // Replaces each upper char (other than first) by '_' and it's lower invariant
                if (char.IsUpper(c))
                {
                    // Appends '_' before each upper char other than the first one
                    if (i > 0)
                        snake.Append('_');

                    // Appends the lower invariant version of the upper char
                    snake.Append(char.ToLowerInvariant(c));
                }
                else
                    snake.Append(c);
            }

            // Returns the `snake_case` version of the `PascalCase`
            return snake.ToString();
        }
    }
}
