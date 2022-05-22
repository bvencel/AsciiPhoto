using AsciiPhoto.Entities;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AsciiPhoto.Helpers
{
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

        public static Letter CreateLetterFromFontData(ConverterSettings settings, string simpleLetter)
        {
            bool[,] pixelMap = GetPixelMapFromFontData(settings, simpleLetter);
            Letter createdLetter = new(simpleLetter, pixelMap, 0, 0);

            return createdLetter;
        }

        public static List<Letter> GenerateAlphabetWithMap(ConverterSettings settings)
        {
            List<Letter> letters = new ();

            int counter = 0;

            foreach (KeyValuePair<string, string[]> simpleLetter in LucidaConsole.GetFilteredMap(settings.Alphabet))
            {
                Letter createdLetter = CreateLetterFromFontData(settings, simpleLetter.Key);

                letters.Add(createdLetter);

                if (settings.PrintFontMatrices)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    Console.WriteLine($"{createdLetter.Character} (first true: {createdLetter.FirstBlackPixelStartInFlatMap}, last true: {createdLetter.LastBlackPixelStartInFlatMap})");
                    PrintMatrix(createdLetter.PixelMap, 1);
                    Console.WriteLine();
                    Console.WriteLine();
#pragma warning restore CS0162 // Unreachable code detected
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