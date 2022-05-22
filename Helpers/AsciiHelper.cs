using AsciiPhoto.Entities;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AsciiPhoto.Helpers
{
    /// <summary>
    /// Helper class responsible for ASCII operations.
    /// </summary>
    internal static class AsciiHelper
    {
        /// <summary>
        /// The closing part of the decor around the generated text result.
        /// </summary>
        public const string DecorEnd = "∙────────────┘";

        /// <summary>
        /// The opening part of the decor around the generated text result.
        /// </summary>
        public const string DecorStart = "┌────────────∙";

        /// <summary>
        /// This alphabet is fixed width, even when varibale-width font is used.
        /// This makes the result usable in environments, like instant messaging.
        /// </summary>
        public const string FixedWidthAlphabet = "OUVXY░▒│╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌▌▐▀▄█";

        /// <summary>
        /// A table with extended ASCII items and the corresponding Unicode codes.
        /// </summary>
        public static readonly List<CharacterConversionItem> ConversionInfo = new()
        {
            new CharacterConversionItem() { AsciiCode = 128, UnicodeCode = 0x00C7, Character = 'Ç', Description = "latin capital letter c with cedilla" },
            new CharacterConversionItem() { AsciiCode = 129, UnicodeCode = 0x00FC, Character = 'ü', Description = "latin small letter u with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 130, UnicodeCode = 0x00E9, Character = 'é', Description = "latin small letter e with acute" },
            new CharacterConversionItem() { AsciiCode = 131, UnicodeCode = 0x00E2, Character = 'â', Description = "latin small letter a with circumflex" },
            new CharacterConversionItem() { AsciiCode = 132, UnicodeCode = 0x00E4, Character = 'ä', Description = "latin small letter a with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 133, UnicodeCode = 0x00E0, Character = 'à', Description = "latin small letter a with grave" },
            new CharacterConversionItem() { AsciiCode = 134, UnicodeCode = 0x00E5, Character = 'å', Description = "latin small letter a with ring above" },
            new CharacterConversionItem() { AsciiCode = 135, UnicodeCode = 0x00E7, Character = 'ç', Description = "latin small letter c with cedilla" },
            new CharacterConversionItem() { AsciiCode = 136, UnicodeCode = 0x00EA, Character = 'ê', Description = "latin small letter e with circumflex" },
            new CharacterConversionItem() { AsciiCode = 137, UnicodeCode = 0x00EB, Character = 'ë', Description = "latin small letter e with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 138, UnicodeCode = 0x00E8, Character = 'è', Description = "latin small letter e with grave" },
            new CharacterConversionItem() { AsciiCode = 139, UnicodeCode = 0x00EF, Character = 'ï', Description = "latin small letter i with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 140, UnicodeCode = 0x00EE, Character = 'î', Description = "latin small letter i with circumflex" },
            new CharacterConversionItem() { AsciiCode = 141, UnicodeCode = 0x00EC, Character = 'ì', Description = "latin small letter i with grave" },
            new CharacterConversionItem() { AsciiCode = 142, UnicodeCode = 0x00C4, Character = 'Ä', Description = "latin capital letter a with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 143, UnicodeCode = 0x00C5, Character = 'Å', Description = "latin capital letter a with ring above" },
            new CharacterConversionItem() { AsciiCode = 144, UnicodeCode = 0x00C9, Character = 'É', Description = "latin capital letter e with acute" },
            new CharacterConversionItem() { AsciiCode = 145, UnicodeCode = 0x00E6, Character = 'æ', Description = "latin small ligature ae" },
            new CharacterConversionItem() { AsciiCode = 146, UnicodeCode = 0x00C6, Character = 'Æ', Description = "latin capital ligature ae" },
            new CharacterConversionItem() { AsciiCode = 147, UnicodeCode = 0x00F4, Character = 'ô', Description = "latin small letter o with circumflex" },
            new CharacterConversionItem() { AsciiCode = 148, UnicodeCode = 0x00F6, Character = 'ö', Description = "latin small letter o with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 149, UnicodeCode = 0x00F2, Character = 'ò', Description = "latin small letter o with grave" },
            new CharacterConversionItem() { AsciiCode = 150, UnicodeCode = 0x00FB, Character = 'û', Description = "latin small letter u with circumflex" },
            new CharacterConversionItem() { AsciiCode = 151, UnicodeCode = 0x00F9, Character = 'ù', Description = "latin small letter u with grave" },
            new CharacterConversionItem() { AsciiCode = 152, UnicodeCode = 0x00FF, Character = 'ÿ', Description = "latin small letter y with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 153, UnicodeCode = 0x00D6, Character = 'Ö', Description = "latin capital letter o with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 154, UnicodeCode = 0x00DC, Character = 'Ü', Description = "latin capital letter u with diaeresis" },
            new CharacterConversionItem() { AsciiCode = 155, UnicodeCode = 0x00A2, Character = '¢', Description = "cent sign" },
            new CharacterConversionItem() { AsciiCode = 156, UnicodeCode = 0x00A3, Character = '£', Description = "pound sign" },
            new CharacterConversionItem() { AsciiCode = 157, UnicodeCode = 0x00A5, Character = '¥', Description = "yen sign" },
            new CharacterConversionItem() { AsciiCode = 158, UnicodeCode = 0x20A7, Character = '₧', Description = "peseta sign" },
            new CharacterConversionItem() { AsciiCode = 159, UnicodeCode = 0x0192, Character = 'ƒ', Description = "latin small letter f with hook" },
            new CharacterConversionItem() { AsciiCode = 160, UnicodeCode = 0x00E1, Character = 'á', Description = "latin small letter a with acute" },
            new CharacterConversionItem() { AsciiCode = 161, UnicodeCode = 0x00ED, Character = 'í', Description = "latin small letter i with acute" },
            new CharacterConversionItem() { AsciiCode = 162, UnicodeCode = 0x00F3, Character = 'ó', Description = "latin small letter o with acute" },
            new CharacterConversionItem() { AsciiCode = 163, UnicodeCode = 0x00FA, Character = 'ú', Description = "latin small letter u with acute" },
            new CharacterConversionItem() { AsciiCode = 164, UnicodeCode = 0x00F1, Character = 'ñ', Description = "latin small letter n with tilde" },
            new CharacterConversionItem() { AsciiCode = 165, UnicodeCode = 0x00D1, Character = 'Ñ', Description = "latin capital letter n with tilde" },
            new CharacterConversionItem() { AsciiCode = 166, UnicodeCode = 0x00AA, Character = 'ª', Description = "feminine ordinal indicator" },
            new CharacterConversionItem() { AsciiCode = 167, UnicodeCode = 0x00BA, Character = 'º', Description = "masculine ordinal indicator" },
            new CharacterConversionItem() { AsciiCode = 168, UnicodeCode = 0x00BF, Character = '¿', Description = "inverted question mark" },
            new CharacterConversionItem() { AsciiCode = 169, UnicodeCode = 0x2310, Character = '⌐', Description = "reversed not sign" },
            new CharacterConversionItem() { AsciiCode = 170, UnicodeCode = 0x00AC, Character = '¬', Description = "not sign" },
            new CharacterConversionItem() { AsciiCode = 171, UnicodeCode = 0x00BD, Character = '½', Description = "vulgar fraction one half" },
            new CharacterConversionItem() { AsciiCode = 172, UnicodeCode = 0x00BC, Character = '¼', Description = "vulgar fraction one quarter" },
            new CharacterConversionItem() { AsciiCode = 173, UnicodeCode = 0x00A1, Character = '¡', Description = "inverted exclamation mark" },
            new CharacterConversionItem() { AsciiCode = 174, UnicodeCode = 0x00AB, Character = '«', Description = "left-pointing double angle quotation mark" },
            new CharacterConversionItem() { AsciiCode = 175, UnicodeCode = 0x00BB, Character = '»', Description = "right-pointing double angle quotation mark" },
            new CharacterConversionItem() { AsciiCode = 176, UnicodeCode = 0x2591, Character = '░', Description = "light shade" },
            new CharacterConversionItem() { AsciiCode = 177, UnicodeCode = 0x2592, Character = '▒', Description = "medium shade" },
            new CharacterConversionItem() { AsciiCode = 178, UnicodeCode = 0x2593, Character = '▓', Description = "dark shade" },
            new CharacterConversionItem() { AsciiCode = 179, UnicodeCode = 0x2502, Character = '│', Description = "box drawings light vertical" },
            new CharacterConversionItem() { AsciiCode = 180, UnicodeCode = 0x2524, Character = '┤', Description = "box drawings light vertical and left" },
            new CharacterConversionItem() { AsciiCode = 181, UnicodeCode = 0x2561, Character = '╡', Description = "box drawings vertical single and left double" },
            new CharacterConversionItem() { AsciiCode = 182, UnicodeCode = 0x2562, Character = '╢', Description = "box drawings vertical double and left single" },
            new CharacterConversionItem() { AsciiCode = 183, UnicodeCode = 0x2556, Character = '╖', Description = "box drawings down double and left single" },
            new CharacterConversionItem() { AsciiCode = 184, UnicodeCode = 0x2555, Character = '╕', Description = "box drawings down single and left double" },
            new CharacterConversionItem() { AsciiCode = 185, UnicodeCode = 0x2563, Character = '╣', Description = "box drawings double vertical and left" },
            new CharacterConversionItem() { AsciiCode = 186, UnicodeCode = 0x2551, Character = '║', Description = "box drawings double vertical" },
            new CharacterConversionItem() { AsciiCode = 187, UnicodeCode = 0x2557, Character = '╗', Description = "box drawings double down and left" },
            new CharacterConversionItem() { AsciiCode = 188, UnicodeCode = 0x255D, Character = '╝', Description = "box drawings double up and left" },
            new CharacterConversionItem() { AsciiCode = 189, UnicodeCode = 0x255C, Character = '╜', Description = "box drawings up double and left single" },
            new CharacterConversionItem() { AsciiCode = 190, UnicodeCode = 0x255B, Character = '╛', Description = "box drawings up single and left double" },
            new CharacterConversionItem() { AsciiCode = 191, UnicodeCode = 0x2510, Character = '┐', Description = "box drawings light down and left" },
            new CharacterConversionItem() { AsciiCode = 192, UnicodeCode = 0x2514, Character = '└', Description = "box drawings light up and right" },
            new CharacterConversionItem() { AsciiCode = 193, UnicodeCode = 0x2534, Character = '┴', Description = "box drawings light up and horizontal" },
            new CharacterConversionItem() { AsciiCode = 194, UnicodeCode = 0x252C, Character = '┬', Description = "box drawings light down and horizontal" },
            new CharacterConversionItem() { AsciiCode = 195, UnicodeCode = 0x251C, Character = '├', Description = "box drawings light vertical and right" },
            new CharacterConversionItem() { AsciiCode = 196, UnicodeCode = 0x2500, Character = '─', Description = "box drawings light horizontal" },
            new CharacterConversionItem() { AsciiCode = 197, UnicodeCode = 0x253C, Character = '┼', Description = "box drawings light vertical and horizontal" },
            new CharacterConversionItem() { AsciiCode = 198, UnicodeCode = 0x255E, Character = '╞', Description = "box drawings vertical single and right double" },
            new CharacterConversionItem() { AsciiCode = 199, UnicodeCode = 0x255F, Character = '╟', Description = "box drawings vertical double and right single" },
            new CharacterConversionItem() { AsciiCode = 200, UnicodeCode = 0x255A, Character = '╚', Description = "box drawings double up and right" },
            new CharacterConversionItem() { AsciiCode = 201, UnicodeCode = 0x2554, Character = '╔', Description = "box drawings double down and right" },
            new CharacterConversionItem() { AsciiCode = 202, UnicodeCode = 0x2569, Character = '╩', Description = "box drawings double up and horizontal" },
            new CharacterConversionItem() { AsciiCode = 203, UnicodeCode = 0x2566, Character = '╦', Description = "box drawings double down and horizontal" },
            new CharacterConversionItem() { AsciiCode = 204, UnicodeCode = 0x2560, Character = '╠', Description = "box drawings double vertical and right" },
            new CharacterConversionItem() { AsciiCode = 205, UnicodeCode = 0x2550, Character = '═', Description = "box drawings double horizontal" },
            new CharacterConversionItem() { AsciiCode = 206, UnicodeCode = 0x256C, Character = '╬', Description = "box drawings double vertical and horizontal" },
            new CharacterConversionItem() { AsciiCode = 207, UnicodeCode = 0x2567, Character = '╧', Description = "box drawings up single and horizontal double" },
            new CharacterConversionItem() { AsciiCode = 208, UnicodeCode = 0x2568, Character = '╨', Description = "box drawings up double and horizontal single" },
            new CharacterConversionItem() { AsciiCode = 209, UnicodeCode = 0x2564, Character = '╤', Description = "box drawings down single and horizontal double" },
            new CharacterConversionItem() { AsciiCode = 210, UnicodeCode = 0x2565, Character = '╥', Description = "box drawings down double and horizontal single" },
            new CharacterConversionItem() { AsciiCode = 211, UnicodeCode = 0x2559, Character = '╙', Description = "box drawings up double and right single" },
            new CharacterConversionItem() { AsciiCode = 212, UnicodeCode = 0x2558, Character = '╘', Description = "box drawings up single and right double" },
            new CharacterConversionItem() { AsciiCode = 213, UnicodeCode = 0x2552, Character = '╒', Description = "box drawings down single and right double" },
            new CharacterConversionItem() { AsciiCode = 214, UnicodeCode = 0x2553, Character = '╓', Description = "box drawings down double and right single" },
            new CharacterConversionItem() { AsciiCode = 215, UnicodeCode = 0x256B, Character = '╫', Description = "box drawings vertical double and horizontal single" },
            new CharacterConversionItem() { AsciiCode = 216, UnicodeCode = 0x256A, Character = '╪', Description = "box drawings vertical single and horizontal double" },
            new CharacterConversionItem() { AsciiCode = 217, UnicodeCode = 0x2518, Character = '┘', Description = "box drawings light up and left" },
            new CharacterConversionItem() { AsciiCode = 218, UnicodeCode = 0x250C, Character = '┌', Description = "box drawings light down and right" },
            new CharacterConversionItem() { AsciiCode = 219, UnicodeCode = 0x2588, Character = '█', Description = "full block" },
            new CharacterConversionItem() { AsciiCode = 220, UnicodeCode = 0x2584, Character = '▄', Description = "lower half block" },
            new CharacterConversionItem() { AsciiCode = 221, UnicodeCode = 0x258C, Character = '▌', Description = "left half block" },
            new CharacterConversionItem() { AsciiCode = 222, UnicodeCode = 0x2590, Character = '▐', Description = "right half block" },
            new CharacterConversionItem() { AsciiCode = 223, UnicodeCode = 0x2580, Character = '▀', Description = "upper half block" },
            new CharacterConversionItem() { AsciiCode = 224, UnicodeCode = 0x03B1, Character = 'α', Description = "greek small letter alpha" },
            new CharacterConversionItem() { AsciiCode = 225, UnicodeCode = 0x00DF, Character = 'ß', Description = "latin small letter sharp s" },
            new CharacterConversionItem() { AsciiCode = 226, UnicodeCode = 0x0393, Character = 'Γ', Description = "greek capital letter gamma" },
            new CharacterConversionItem() { AsciiCode = 227, UnicodeCode = 0x03C0, Character = 'π', Description = "greek small letter pi" },
            new CharacterConversionItem() { AsciiCode = 228, UnicodeCode = 0x03A3, Character = 'Σ', Description = "greek capital letter sigma" },
            new CharacterConversionItem() { AsciiCode = 229, UnicodeCode = 0x03C3, Character = 'σ', Description = "greek small letter sigma" },
            new CharacterConversionItem() { AsciiCode = 230, UnicodeCode = 0x00B5, Character = 'µ', Description = "micro sign" },
            new CharacterConversionItem() { AsciiCode = 231, UnicodeCode = 0x03C4, Character = 'τ', Description = "greek small letter tau" },
            new CharacterConversionItem() { AsciiCode = 232, UnicodeCode = 0x03A6, Character = 'Φ', Description = "greek capital letter phi" },
            new CharacterConversionItem() { AsciiCode = 233, UnicodeCode = 0x0398, Character = 'Θ', Description = "greek capital letter theta" },
            new CharacterConversionItem() { AsciiCode = 234, UnicodeCode = 0x03A9, Character = 'Ω', Description = "greek capital letter omega" },
            new CharacterConversionItem() { AsciiCode = 235, UnicodeCode = 0x03B4, Character = 'δ', Description = "greek small letter delta" },
            new CharacterConversionItem() { AsciiCode = 236, UnicodeCode = 0x221E, Character = '∞', Description = "infinity" },
            new CharacterConversionItem() { AsciiCode = 237, UnicodeCode = 0x03C6, Character = 'φ', Description = "greek small letter phi" },
            new CharacterConversionItem() { AsciiCode = 238, UnicodeCode = 0x03B5, Character = 'ε', Description = "greek small letter epsilon" },
            new CharacterConversionItem() { AsciiCode = 239, UnicodeCode = 0x2229, Character = '∩', Description = "intersection" },
            new CharacterConversionItem() { AsciiCode = 240, UnicodeCode = 0x2261, Character = '≡', Description = "identical to" },
            new CharacterConversionItem() { AsciiCode = 241, UnicodeCode = 0x00B1, Character = '±', Description = "plus-minus sign" },
            new CharacterConversionItem() { AsciiCode = 242, UnicodeCode = 0x2265, Character = '≥', Description = "greater-than or equal to" },
            new CharacterConversionItem() { AsciiCode = 243, UnicodeCode = 0x2264, Character = '≤', Description = "less-than or equal to" },
            new CharacterConversionItem() { AsciiCode = 244, UnicodeCode = 0x2320, Character = '⌠', Description = "top half integral" },
            new CharacterConversionItem() { AsciiCode = 245, UnicodeCode = 0x2321, Character = '⌡', Description = "bottom half integral" },
            new CharacterConversionItem() { AsciiCode = 246, UnicodeCode = 0x00F7, Character = '÷', Description = "division sign" },
            new CharacterConversionItem() { AsciiCode = 247, UnicodeCode = 0x2248, Character = '≈', Description = "almost equal to" },
            new CharacterConversionItem() { AsciiCode = 248, UnicodeCode = 0x00B0, Character = '°', Description = "degree sign" },
            new CharacterConversionItem() { AsciiCode = 249, UnicodeCode = 0x2219, Character = '∙', Description = "bullet operator" },
            new CharacterConversionItem() { AsciiCode = 250, UnicodeCode = 0x00B7, Character = '·', Description = "middle dot" },
            new CharacterConversionItem() { AsciiCode = 251, UnicodeCode = 0x221A, Character = '√', Description = "square root" },
            new CharacterConversionItem() { AsciiCode = 252, UnicodeCode = 0x207F, Character = 'ⁿ', Description = "superscript latin small letter n" },
            new CharacterConversionItem() { AsciiCode = 253, UnicodeCode = 0x00B2, Character = '²', Description = "superscript two" },
            new CharacterConversionItem() { AsciiCode = 254, UnicodeCode = 0x25A0, Character = '■', Description = "black square" },
        };

        public static string ConverterFrom437ToUnicode(string asciiStringToConvert)
        {
            StringBuilder result = new();

            // Iterate through the string char by char
            foreach (char asciiChar in asciiStringToConvert)
            {
                int asciiCharacterCode = asciiChar;

                CharacterConversionItem? conversionInfo = ConversionInfo.Find(c => c.AsciiCode == asciiCharacterCode);

                if (conversionInfo == null)
                {
                    result.Append(asciiChar);
                }
                else
                {
                    result.Append((char)conversionInfo.UnicodeCode);
                }
            }

            return result.ToString();
        }

        public static string ConverterFromUnicodeTo437(string unicodeStringToConvert)
        {
            StringBuilder result = new();

            // Iterate through the string char by char
            foreach (char unicodeChar in unicodeStringToConvert)
            {
                int unicodeCharacterCode = unicodeChar;

                CharacterConversionItem? conversionInfo = ConversionInfo.Find(c => c.UnicodeCode == unicodeCharacterCode);

                if (conversionInfo == null)
                {
                    result.Append(unicodeChar);
                }
                else
                {
                    result.Append((char)conversionInfo.AsciiCode);
                }
            }

            return result.ToString();
        }

        public static Letter CreateLetterFromFontData(ConverterSettings settings, string simpleLetter)
        {
            bool[,] pixelMap = GetPixelMapFromFontData(settings, simpleLetter);
            Letter createdLetter = new(simpleLetter, pixelMap, 0, 0);

            return createdLetter;
        }

        public static List<Letter> GenerateAlphabetWithMap(ConverterSettings settings)
        {
            List<Letter> letters = new();

            int counter = 0;

            foreach (KeyValuePair<string, string[]> simpleLetter in LucidaConsole.GetFilteredMap(settings.Alphabet))
            {
                Letter createdLetter = CreateLetterFromFontData(settings, simpleLetter.Key);

                letters.Add(createdLetter);

                if (settings.PrintFontMatrices)
                {
                    Console.WriteLine($"{createdLetter.Character} (first true: {createdLetter.FirstBlackPixelStartInFlatMap}, last true: {createdLetter.LastBlackPixelStartInFlatMap})");
                    PrintMatrix(createdLetter.PixelMap, 1);
                    Console.WriteLine();
                    Console.WriteLine();
                }

                // If needed, create variants
                // Variants are not needed if brightness is matched
                if (!settings.MatchBrightness && (settings.HorizontalOffset != 0 || settings.VerticalOffset != 0))
                {
                    for (int y = -settings.VerticalOffset; y <= settings.VerticalOffset; y++)
                    {
                        for (int x = -settings.HorizontalOffset; x <= settings.HorizontalOffset; x++)
                        {
                            if (x == 0 && y == 0)
                            {
                                // 0, 0 is already generated
                                continue;
                            }

                            int offsetCol = x;
                            int offsetRow = y;

                            bool[,] shiftedMatrix = ArrayHelper.ShiftMatrix(createdLetter.PixelMap, offsetCol, offsetRow);
                            Letter letterVariant = new(createdLetter.Character, shiftedMatrix, offsetCol, offsetRow, createdLetter.PixelCountInOriginal);

                            letters.Add(letterVariant);
                        }
                    }
                }

                counter++;
            }

            return letters;
        }

        public static string GenerateAsciiArtString(ConverterSettings settings, Letter[,] characterMatrix)
        {
            StringBuilder result = new();

            if (settings.Verbose)
            {
                result.AppendLine($"    {DecorStart}");
            }

            for (int y = 0; y < characterMatrix.GetLength(1); y++)
            {
                if (settings.Verbose)
                {
                    result.Append($"{y + 1,3} │ ");
                }

                for (int x = 0; x < characterMatrix.GetLength(0); x++)
                {
                    result.Append(characterMatrix[x, y].Character);
                }

                if (settings.Verbose)
                {
                    result.Append(" │");
                }

                result.AppendLine();
            }

            if (settings.Verbose)
            {
                result.AppendLine(DecorEnd.Indent(characterMatrix.GetLength(0) - DecorEnd.Length + 8, ' '));
            }

            return result.ToString();
        }

        /// <summary>
        /// Matches the brightness of the image pixels with the brightness of the characters.
        /// Main entry point of the class.
        /// </summary>
        public static Letter[,] GenerateAsciiFromBitmapByBrightness(ConverterSettings settings, Bitmap image, Dictionary<Letter, float> alphabet)
        {
            Letter[,] resultCharacterMap = new Letter[image.Width, image.Height];

            if (settings.PrintResultsAsap && settings.Verbose)
            {
                Console.WriteLine($"    {DecorStart}");
            }

            for (int y = 0; y < image.Height; y++)
            {
                if (settings.PrintResultsAsap && settings.Verbose)
                {
                    Console.Write($"{y + 1,3} │ ");
                }

                for (int x = 0; x < image.Width; x++)
                {
                    Color colorAtTopPixel = image.GetPixel(x, y);
                    float brightness = ImageHelper.GetBrightness(colorAtTopPixel);

                    Letter resultChar = MapBrightnessToAscii(settings, brightness, alphabet);
                    resultCharacterMap[x, y] = resultChar;

                    if (settings.PrintResultsAsap)
                    {
                        Console.Write(resultChar);
                    }
                }

                if (settings.PrintResultsAsap)
                {
                    if (settings.Verbose)
                    {
                        Console.WriteLine(" │");
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }

            if (settings.PrintResultsAsap && settings.Verbose)
            {
                Console.WriteLine(DecorEnd.Indent(resultCharacterMap.GetLength(0) - DecorEnd.Length + 8, ' '));
            }

            return resultCharacterMap;
        }

        /// <summary>
        /// Gets the ASCII table.
        /// https://en.wikipedia.org/wiki/Code_page_437
        /// </summary>
        /// <returns></returns>
        public static string GetAsciiTableAsUnicode()
        {
            StringBuilder extendedAsciiTable = new();

            // Ignoring the first 16 (0 → 15) items, like ☺☻♥♦♣♠ and other control characters
            // 255 is non-breaking space
            for (int i = 16; i <= 254; i++)
            {
                extendedAsciiTable.Append(ConverterFrom437ToUnicode(((char)i).ToString()));
            }

            return extendedAsciiTable.ToString();
        }

        /// <summary>
        /// Gets a bool matrix from the pixels of an image.
        /// If the pixel's perceived brightness is low, then the bool will be se tto true.
        /// </summary>
        /// <param name="nrFilledItems">Number of items in the matrix that were considered black. Used in pattern optimization (less balcks, the better).</param>
        /// <returns></returns>
        public static decimal[,] GetPixelMapFromBitmap(ConverterSettings settings, Bitmap image, int widthMustBeDivisibleWith, int heightMustBeDivisibleWith)
        {
            decimal[,] matrix = new decimal[MathHelper.RoundUpToBeDivisible(image.Width, widthMustBeDivisibleWith), MathHelper.RoundUpToBeDivisible(image.Height, heightMustBeDivisibleWith)];

            ////Console.WriteLine($"Rounding [{image.Width}x{image.Height}] image up to {matrix.GetLength(0)}x{matrix.GetLength(1)} to make width divisible by {widthMustBeDivisibleWith} and height by {heightMustBeDivisibleWith}");

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color colorAtPixel = image.GetPixel(x, y);

                    // Workaround: Transparent color is identified by name "0"
                    matrix[x, y] = ImageHelper.GetColorStrength(settings, colorAtPixel);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Generates the art by trying to match character as best as possible onto character-sized pieces of the original image.
        /// Main entry point of the class.
        /// </summary>
        public static Letter[,] MapAlphabetOntoBitmap(ConverterSettings settings, decimal[,] imageMatrix, List<Letter> alphabet, Size characterSize)
        {
            int nrColumns = (int)Math.Round((decimal)(imageMatrix.GetLength(0) / characterSize.Width), MidpointRounding.ToEven);
            int nrRows = (int)Math.Round((decimal)(imageMatrix.GetLength(1) / characterSize.Height), MidpointRounding.ToEven);

            Letter[,] resultCharacterMap = new Letter[nrColumns, nrRows];

            if (settings.PrintResultsAsap)
            {
                if (settings.Verbose)
                {
                    Console.WriteLine($"    {DecorStart}");
                }
            }

            for (int row = 0; row < nrRows; row++)
            {
                if (settings.PrintResultsAsap)
                {
                    if (settings.Verbose)
                    {
                        Console.Write($"{row + 1,3} │ ");
                    }
                }

                for (int col = 0; col < nrColumns; col++)
                {
                    resultCharacterMap[col, row] = MapLetterOntoPieceOfImage(settings, imageMatrix, alphabet, characterSize, col, row);

                    if (settings.PrintResultsAsap)
                    {
                        Console.Write(resultCharacterMap[col, row]);
                    }
                }

                if (settings.PrintResultsAsap)
                {
                    if (settings.Verbose)
                    {
                        Console.WriteLine(" │");
                    }
                }
            }

            if (settings.PrintResultsAsap)
            {
                if (settings.Verbose)
                {
                    Console.WriteLine(DecorEnd.Indent(resultCharacterMap.GetLength(0) - DecorEnd.Length + 8, ' '));
                }
            }

            return resultCharacterMap;
        }

        /// <summary>
        /// Overlays the letter over the image fragment and counts the matches.
        /// Uses flattened matrix so this method only needs one cycle.
        /// </summary>
        private static LetterMatch CalculateMatchForLetterUsingFlatArray(ConverterSettings settings, decimal[] subMap, Letter letter)
        {
            int nrMatchingBlackPixels = 0;
            int nrMatchingGrayPixels = 0;

            // There is no need to compare before the first "true" value
            int startPos = Math.Min(Letter.GetFirstNonZero(subMap), letter.FirstBlackPixelStartInFlatMap);
            int endPos = Math.Min(Letter.GetLastNonZero(subMap), letter.LastBlackPixelStartInFlatMap);

            for (int i = startPos; i <= endPos; i++)
            {
                // **********************
                // Matching formula
                // **********************

                // If letter has a pixel here
                if (letter.PixelMapFlat[i])
                {
                    if (subMap[i] == ImageHelper.MaxPencilStrength)
                    {
                        // If black pixel matches
                        nrMatchingBlackPixels++;
                    }

                    if (subMap[i] == ImageHelper.HalfPencilStrength)
                    {
                        // If black pixel matches
                        nrMatchingGrayPixels++;
                    }
                }
            }

            // Create a new letter with color info as well
            Letter resultLetter = (Letter)letter.Clone();
            resultLetter.TextColor = nrMatchingGrayPixels > nrMatchingBlackPixels ? (ConsoleColor?)ConsoleColor.DarkGray : null;

            LetterMatch result = new(
                settings.WeightOffset,
                settings.WeightTotalPixelNumber,
                resultLetter,
                nrMatchingBlackPixels + nrMatchingGrayPixels);

            return result;
        }

        private static LetterMatch GetMatchedCharacterCodeFlat(ConverterSettings settings, decimal[,] subBoolMap, List<Letter> alphabet)
        {
#pragma warning disable CS0162 // Unreachable code detected
            // Charcode/Matches/Nr true items (more "true"s, the worse)
            List<LetterMatch> matchingLetters = new();
            decimal[] flattenedSubBoolMap = Letter.FlattenMatrix(subBoolMap);

            foreach (Letter letter in alphabet)
            {
                LetterMatch matchForLetterVariant = CalculateMatchForLetterUsingFlatArray(settings, flattenedSubBoolMap, letter);

                matchingLetters.Add(matchForLetterVariant);
            }

            LetterMatch selectedBestMatch = SelectBestMatch(matchingLetters);

            return selectedBestMatch;
#pragma warning restore CS0162 // Unreachable code detected
        }

        private static bool[,] GetPixelMapFromFontData(ConverterSettings settings, string character)
        {
            bool[,] matrix = new bool[LucidaConsole.CharacterSize.Width, LucidaConsole.CharacterSize.Height];

            for (int y = 0; y < LucidaConsole.CharacterSize.Height; y++)
            {
                for (int x = 0; x < LucidaConsole.CharacterSize.Width; x++)
                {
                    matrix[x, y] = LucidaConsole.GetFilteredMap(settings.Alphabet)[character][y][x].Equals('█');
                }
            }

            return matrix;
        }

        /// <summary>
        /// Assumes the letters are sorted.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="brightness"></param>
        /// <param name="alphabet"></param>
        private static Letter MapBrightnessToAscii(ConverterSettings settings, float brightness, Dictionary<Letter, float> alphabet)
        {
            brightness += settings.BrightnessOffset;

            if (settings.InverseBrightness)
            {
                // Iterate from darkest → brightest
                foreach (var (key, value) in alphabet)
                {
                    if (value >= brightness)
                    {
                        // Immediately return the first good value
                        return key;
                    }
                }
            }
            else
            {
                // Iterate from brigthest → darkest
                foreach (var (key, value) in alphabet)
                {
                    if (value <= brightness)
                    {
                        // Immediately return the first good value
                        return key;
                    }
                }
            }

            return alphabet.First().Key;
        }

        /// <summary>
        /// Looks for match on a partial rectangle and decides the character.
        /// </summary>
        private static Letter MapLetterOntoPieceOfImage(ConverterSettings settings, decimal[,] imageMatrix, List<Letter> alphabet, Size characterSize, int col, int row)
        {
            decimal[,] subMatrix = ArrayHelper.ExtractSubMatrix(
                imageMatrix,
                col * characterSize.Width,
                (col + 1) * characterSize.Width,
                row * characterSize.Height,
                (row + 1) * characterSize.Height);

            if (ArrayHelper.AllIsZero(subMatrix))
            {
                return alphabet[0];
            }
            else
            {
                // Best matched character
                LetterMatch bestMatch = GetMatchedCharacterCodeFlat(settings, subMatrix, alphabet);

                if (bestMatch == null)
                {
                    return alphabet[0];
                }

                return bestMatch.MatchedLetter;
            }
        }

        private static void PrintMatrix<T>(T[,] matrix, int steps)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                if (y % steps == 0)
                {
                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        if (x % Math.Max(steps / 2, 1) == 0)
                        {
                            if (matrix is bool[,])
                            {
                                const string FilledChar = "█";
                                const string EmptyChar = "·";

                                Console.Write(Convert.ToBoolean(matrix[x, y]) ? FilledChar : EmptyChar);
                            }
                            else
                            {
                                Console.Write(matrix[x, y] != null ? matrix[x, y].ToString() : "··");
                                Console.Write("|");
                            }
                        }
                    }

                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Selects best match from a list of matches by calculating a score for each.
        /// </summary>
        /// <param name="allMatches">List of [Letter, matching number]</param>
        /// <returns>Best scoring match, containing the Letter, that will likely represent the bitmap.</returns>
        private static LetterMatch SelectBestMatch(List<LetterMatch> allMatches)
        {
            if (allMatches == null || allMatches.Count == 0)
            {
                return null;
            }

            decimal highestScore = 0;
            LetterMatch highestScoring = null;

            foreach (LetterMatch match in allMatches)
            {
                if (match is null)
                {
                    // Match was null
                    continue;
                }

                decimal score = match.Score;

                if (highestScoring == null || score > highestScore)
                {
                    highestScore = score;
                    highestScoring = match;
                }
            }

            return highestScoring;
        }
    }
}