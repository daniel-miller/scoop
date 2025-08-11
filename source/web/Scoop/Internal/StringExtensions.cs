using System.Text.RegularExpressions;

namespace Scoop
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to kebab-case (lowercase with hyphens)
        /// </summary>
        /// <param name="input">The input string to convert</param>
        /// <returns>A kebab-case formatted string</returns>
        public static string ToKebabCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Handle multiple consecutive uppercase letters (e.g., "XMLHttpRequest" -> "xml-http-request")
            var result = Regex.Replace(input, "([A-Z]+)([A-Z][a-z])", "$1-$2");

            // Handle transition from lowercase to uppercase (e.g., "camelCase" -> "camel-Case")
            result = Regex.Replace(result, "([a-z])([A-Z])", "$1-$2");

            // Replace spaces, underscores, and other non-alphanumeric characters with hyphens
            result = Regex.Replace(result, "[^a-zA-Z0-9]+", "-");

            // Remove leading/trailing hyphens and convert to lowercase
            return result.Trim('-').ToLower();
        }
    }
}