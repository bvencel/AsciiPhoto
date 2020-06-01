using System.Text;

namespace AsciiPhoto.Helpers
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Adds <paramref name="indentSize"/> number of characters before the string.
        /// </summary>
        /// <param name="textToBeIndented"></param>
        /// <param name="indentSize">Number of instances of <paramref name="indentChar"/> to add.</param>
        /// <param name="indentChar">The caracters added <paramref name="indentSize"/> times.</param>
        /// <returns>[indentation][originalString]</returns>
        public static string Indent(this string textToBeIndented, int indentSize, char indentChar = ' ')
        {
            StringBuilder indentString = new StringBuilder();

            for (int i = 0; i < indentSize; i++)
            {
                indentString.Append(indentChar);
            }

            return $"{indentString}{textToBeIndented}";
        }
    }
}