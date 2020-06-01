﻿using AsciiPhoto.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AsciiPhoto.Helpers
{
    internal static class AsciiHelper
    {
        private const string DecorEnd = "∙────────────┘";

        private const string DecorStart = "┌────────────∙";

        public static Letter CreateLetterFromFontData(string simpleLetter)
        {
            bool[,] pixelMap = GetPixelMapFromFontData(simpleLetter);
            Letter createdLetter = new Letter(simpleLetter, pixelMap, 0, 0);

            return createdLetter;
        }

        public static string GenerateAsciiArtString(ConverterSettings settings, string[,] characterMatrix)
        {
            StringBuilder result = new StringBuilder();

            if (settings.Verbose)
            {
                result.AppendLine($" {DecorStart}");
            }

            for (int y = 0; y < characterMatrix.GetLength(1); y++)
            {
                if (settings.Verbose)
                {
                    result.Append(" │ ");
                }

                for (int x = 0; x < characterMatrix.GetLength(0); x++)
                {
                    result.Append(characterMatrix[x, y]);
                }

                if (settings.Verbose)
                {
                    result.Append(" │");
                }

                result.AppendLine();
            }

            if (settings.Verbose)
            {
                result.AppendLine(DecorEnd.Indent(characterMatrix.GetLength(0) - DecorEnd.Length + 5, ' '));
            }

            return result.ToString();
        }

        /// <summary>
        /// Matches the brightness of the image pixels with the brightness of the characters.
        /// Main entry point of the class.
        /// </summary>
        public static string[,] GenerateAsciiFromBitmapByBrightness(ConverterSettings settings, Bitmap image, Dictionary<string, float> alphabet)
        {
            string[,] resultCharacterMap = new string[image.Width, image.Height];

            if (settings.PrintResultsAsap && settings.Verbose)
            {
                Console.WriteLine($" {DecorStart}");
            }

            for (int y = 0; y < image.Height; y++)
            {
                if (settings.PrintResultsAsap && settings.Verbose)
                {
                    Console.Write(" │ ");
                }

                for (int x = 0; x < image.Width; x++)
                {
                    Color colorAtTopPixel = image.GetPixel(x, y);
                    float brightness = ImageHelper.GetBrightness(colorAtTopPixel);

                    string resultChar = MapBrightnessToAscii(settings, brightness, alphabet);
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
                Console.WriteLine(DecorEnd.Indent(resultCharacterMap.GetLength(0) - DecorEnd.Length + 5, ' '));
            }

            return resultCharacterMap;
        }

        public static List<Letter> GenerateAlphabetWithMap(ConverterSettings settings)
        {
            List<Letter> letters = new List<Letter>();

            int counter = 0;

            foreach (KeyValuePair<string, string[]> simpleLetter in LucidaConsole.Map)
            {
                Letter createdLetter = CreateLetterFromFontData(simpleLetter.Key);

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
                            Letter letterVariant = new Letter(createdLetter.Character, shiftedMatrix, offsetCol, offsetRow, createdLetter.PixelCountInOriginal);

                            letters.Add(letterVariant);
                        }
                    }
                }

                counter++;
            }

            return letters;
        }

        /// <summary>
        /// Gets a bool matrix from the pixels of an image.
        /// If the pixel's perceived brightness is low, then the bool will be se tto true.
        /// </summary>
        /// <param name="nrFilledItems">Number of items in the matrix that were considered black. Used in pattern optimization (less balcks, the better).</param>
        /// <returns></returns>
        public static bool[,] GetPixelMapFromBitmap(ConverterSettings settings, Bitmap image, int widthMustBeDivisibleWith, int heightMustBeDivisibleWith)
        {
            bool[,] matrix = new bool[MathHelper.RoundUpToBeDivisible(image.Width, widthMustBeDivisibleWith), MathHelper.RoundUpToBeDivisible(image.Height, heightMustBeDivisibleWith)];

            ////Console.WriteLine($"Rounding [{image.Width}x{image.Height}] image up to {matrix.GetLength(0)}x{matrix.GetLength(1)} to make width divisible by {widthMustBeDivisibleWith} and height by {heightMustBeDivisibleWith}");

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color colorAtPixel = image.GetPixel(x, y);

                    // Workaround: Transparent color is identified by name "0"
                    matrix[x, y] = ImageHelper.ColorIsDarkEnough(settings, colorAtPixel);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Generates the art by trying to match character as best as possible onto character-sized pieces of the original image.
        /// Main entry point of the class.
        /// </summary>
        public static string[,] MapAlphabetOntoBitmap(ConverterSettings settings, bool[,] imageMatrix, List<Letter> alphabet, Size characterSize)
        {
            int nrColumns = (int)Math.Round((decimal)(imageMatrix.GetLength(0) / characterSize.Width), MidpointRounding.ToEven);
            int nrRows = (int)Math.Round((decimal)(imageMatrix.GetLength(1) / characterSize.Height), MidpointRounding.ToEven);

            string[,] resultCharacterMap = new string[nrColumns, nrRows];

            if (settings.PrintResultsAsap)
            {
                if (settings.Verbose)
                {
                    Console.WriteLine($" {DecorStart}");
                }
            }

            for (int row = 0; row < nrRows; row++)
            {
                if (settings.PrintResultsAsap)
                {
                    if (settings.Verbose)
                    {
                        Console.Write(" │ ");
                    }
                }

                for (int col = 0; col < nrColumns; col++)
                {
                    resultCharacterMap[col, row] = MapLetterOntoAPieceOfImage(settings, imageMatrix, alphabet, characterSize, col, row);

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
                    Console.WriteLine(DecorEnd.Indent(resultCharacterMap.GetLength(0) - DecorEnd.Length + 5, ' '));
                }
            }

            return resultCharacterMap;
        }

        /// <summary>
        /// Overlays the letter over the image fragment and counts the matches.
        /// Uses flattened matrix so this method only needs one cycle.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="subBoolMap"></param>
        /// <param name="letter"></param>
        /// <returns></returns>
        private static LetterMatch CalculateMatchForLetterUsingFlatArray(ConverterSettings settings, bool[] subBoolMap, Letter letter)
        {
            int nrMatchingBlackPixels = 0;

            // There is no need to compare before the first "true" value
            int startPos = Math.Min(Letter.GetFirstTrue(subBoolMap), letter.FirstBlackPixelStartInFlatMap);
            int endPos = Math.Min(Letter.GetLastTrue(subBoolMap), letter.LastBlackPixelStartInFlatMap);

            for (int i = startPos; i <= endPos; i++)
            {
                // **********************
                // Matching formula
                // **********************
                if (subBoolMap[i] && letter.PixelMapFlat[i])
                {
                    // If black pixel matches
                    nrMatchingBlackPixels++;
                }
            }

            LetterMatch result = new LetterMatch(
                settings.WeightOffset,
                settings.WeightTotalPixelNumber,
                letter,
                nrMatchingBlackPixels);

            return result;
        }

        private static LetterMatch GetMatchedCharacterCodeFlat(ConverterSettings settings, bool[,] subBoolMap, List<Letter> alphabet)
        {
#pragma warning disable CS0162 // Unreachable code detected
            // Charcode/Matches/Nr true items (more "true"s, the worse)
            List<LetterMatch> matchingLetters = new List<LetterMatch>();
            bool[] flattenedSubBoolMap = Letter.FlattenMatrix(subBoolMap);

            foreach (Letter letter in alphabet)
            {
                LetterMatch matchForLetterVariant = CalculateMatchForLetterUsingFlatArray(settings, flattenedSubBoolMap, letter);

                matchingLetters.Add(matchForLetterVariant);
            }

            LetterMatch selectedBestMatch = SelectBestMatch(matchingLetters);

            return selectedBestMatch;
#pragma warning restore CS0162 // Unreachable code detected
        }

        private static bool[,] GetPixelMapFromFontData(string character)
        {
            bool[,] matrix = new bool[LucidaConsole.CharacterSize.Width, LucidaConsole.CharacterSize.Height];

            for (int y = 0; y < LucidaConsole.CharacterSize.Height; y++)
            {
                for (int x = 0; x < LucidaConsole.CharacterSize.Width; x++)
                {
                    matrix[x, y] = LucidaConsole.Map[character][y][x].Equals('█');
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
        /// <returns></returns>
        private static string MapBrightnessToAscii(ConverterSettings settings, float brightness, Dictionary<string, float> alphabet)
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
        /// <param name="settings"></param>
        /// <param name="imageMatrix"></param>
        /// <param name="alphabet"></param>
        /// <param name="characterSize"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static string MapLetterOntoAPieceOfImage(ConverterSettings settings, bool[,] imageMatrix, List<Letter> alphabet, Size characterSize, int col, int row)
        {
            bool[,] subMatrix = ArrayHelper.ExtractSubMatrix(
                imageMatrix,
                col * characterSize.Width,
                (col + 1) * characterSize.Width,
                row * characterSize.Height,
                (row + 1) * characterSize.Height);

            if (ArrayHelper.AllIsFalse(subMatrix))
            {
                return " ";
            }
            else
            {
                // Best matched character
                LetterMatch bestMatch = GetMatchedCharacterCodeFlat(settings, subMatrix, alphabet);

                return (bestMatch is null) ? "@" : bestMatch.MatchedLetter.Character;
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