using AsciiPhoto.Entities;
using AsciiPhoto.Enums;
using AsciiPhoto.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsciiPhoto
{
    /// <summary>
    /// Generates ASCII art from image files found in a folder or from screenshot.
    /// This is the main entry point of the application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Generates ASCII art from image files found in the <paramref name="path" /> directory or screen.
        /// </summary>
        /// <param name="brightnessOffset">Increase or decrease the brightness of the read pixel before processing it.</param>
        /// <param name="brightnessThreshold">The brightness, expressed in percent, above which pixels are considered empty/white.</param>
        /// <param name="charsInRow">Sets the width of the result to this many characters.</param>
        /// <param name="clearScreen">If true, the console will be cleared before every file.</param>
        /// <param name="delay">The wait time after displaying each frame in milliseconds.</param>
        /// <param name="horizontalOffset"></param>
        /// <param name="inverse">If true, the colors will be displayed in the negative.</param>
        /// <param name="loopNr">How many times should the input be processed.</param>
        /// <param name="matchBrightness">If true, the brightness of the image will be processed instead of the oiutlines.</param>
        /// <param name="outputFile">The full path to the file where the results will be written. If empty, no output file will be written. If the file exists, it will be overwritten.</param>
        /// <param name="path">The folder that is searched in all depth for picture files to process (jpg, png, gif).</param>
        /// <param name="pixelsBetweenRows">Number of pixels assumed between console rows. Text editors usually leave 2 px distance, console leaves 1.</param>
        /// <param name="printFontMatrices">If true, bitmatrices of the fonts are printed before starting the processing of the images.</param>
        /// <param name="printResultsAsap">If true, characters are printed as soon as they are generated.</param>
        /// <param name="ratio">Resizes a picture before creating ASCII art, by multiplying width and height with this percent.</param>
        /// <param name="returnToStart">If true, the new result will start on position 0,0 of the console.</param>
        /// <param name="screenNr">Not used at the moment.</param>
        /// <param name="source">The input method: Folder = 1, File = 2, Screen = 4</param>
        /// <param name="verbose">If true, all information will be written in the console, otherwise only the results will be written. File content is not affected, that is always full.</param>
        /// <param name="verticalOffset"></param>
        /// <param name="weightOffsetPercent"></param>
        /// <param name="weightTotalPixelNumberPercent"></param>
        public static void Main(
            decimal brightnessOffset = 0.05m,
            int brightnessThreshold = 85,
            int charsInRow = 0,
            bool clearScreen = false,
            int delay = 0,
            int horizontalOffset = 1,
            bool inverse = true,
            int loopNr = 1,
            bool matchBrightness = false,
            string outputFile = "",
            string path = "",
            int pixelsBetweenRows = 1,
            bool printFontMatrices = false,
            bool printResultsAsap = true,
            int ratio = 50,
            bool returnToStart = false,
            int screenNr = 0,
            InputSources source = InputSources.NotSet,
            bool verbose = true,
            int verticalOffset = 2,
            int weightOffsetPercent = 0,
            int weightTotalPixelNumberPercent = 100)
        {
            ConverterSettings settings = new ConverterSettings()
            {
                BrightnessOffset = (float)brightnessOffset,
                BrightnessThreshold = (float)MathHelper.PercentToDecimal(brightnessThreshold),
                ClearScreen = clearScreen,
                DelayBetweenFramesMs = delay,
                HorizontalOffset = horizontalOffset,
                InverseBrightness = inverse,
                LoopNr = loopNr,
                MatchBrightness = matchBrightness,
                NrCharactersInARow = charsInRow,
                OutputFile = outputFile,
                Path = path,
                PixelsBetweenRows = pixelsBetweenRows,
                PrintFontMatrices = printFontMatrices,
                PrintResultsAsap = printResultsAsap,
                ProcessedImageSizeRatio = MathHelper.PercentToDecimal(ratio),
                ReturnToStart = returnToStart,
                ScreenNr = screenNr,
                Source = source,
                Verbose = verbose,
                VerticalOffset = verticalOffset,
                WeightOffset = MathHelper.PercentToDecimal(weightOffsetPercent),
                WeightTotalPixelNumber = MathHelper.PercentToDecimal(weightTotalPixelNumberPercent),
            };

            AdjustSettings(settings);

            CreateArt(settings);
        }

        private static void AdjustSettings(ConverterSettings settings)
        {
            if (settings.Source == InputSources.NotSet)
            {
                if (FileHelper.IsFile(settings.Path))
                {
                    settings.Source = InputSources.File;
                }
                else
                {
                    settings.Source = InputSources.Folder;
                }
            }
        }

        private static void CreateArt(ConverterSettings settings)
        {
            if (settings is null)
            {
                Console.WriteLine("Settings were null");
                return;
            }

            if (settings.Source == InputSources.NotSet)
            {
                Console.WriteLine("No input source specified");
                return;
            }

            StringBuilder consoleContent = new StringBuilder();

            ////consoleContent.AppendLine($"Using settings {settings}");

            // Generate letter collection
            Stopwatch stopWatch = Stopwatch.StartNew();
            string alphabetString = string.Join(string.Empty, LucidaConsole.Map.Keys);

            // Letters with bitmaps
            List<Letter> alphabet = AsciiHelper.GenerateAlphabetWithMap(settings);

            // Letters for brightness
            Dictionary<string, float> alphabetForBrightness = GetAlphabetForBrightness(settings);

            if (settings.PrintFontMatrices)
            {
                PrintAlphabetWithProperties(alphabet);
            }

            stopWatch.Stop();

            consoleContent.AppendLine($"Using characters: '{alphabetString}'");
            consoleContent.AppendLine($"Loaded {alphabet.Count} fonts ({stopWatch.Elapsed.TotalMilliseconds:N0}ms)");

            stopWatch.Restart();
            List<BitmapWithMetadata> loadedBitmaps = new List<BitmapWithMetadata>();

            // Load bitmaps
            if (settings.Source != InputSources.Screen)
            {
                loadedBitmaps = FileHelper.LoadBitmapsOriginalSize(settings, consoleContent);

                if (loadedBitmaps == null || loadedBitmaps.Count == 0)
                {
                    Console.WriteLine("Could not load any bitmaps");
                    return;
                }
            }

            stopWatch.Stop();

            consoleContent.AppendLine($"Loaded {loadedBitmaps.Count} bitmaps ({stopWatch.Elapsed.TotalMilliseconds:N0}ms)");

            if (settings.Verbose)
            {
                Console.WriteLine(consoleContent);
            }

            int cursorTopPosition = Console.CursorTop;

            for (int i = 0; i < settings.LoopNr; i++)
            {
                // Start processing files
                int counter = 0;
                bool firstRun = true;

                if (settings.Source == InputSources.Screen)
                {
                    loadedBitmaps = ImageHelper.LoadScreenshot(settings);

                    if (loadedBitmaps == null || loadedBitmaps.Count == 0)
                    {
                        Console.WriteLine("Could not load screenshot");
                    }
                }

                foreach (BitmapWithMetadata loadedBitmap in loadedBitmaps)
                {
                    counter++;

                    if (settings.ReturnToStart)
                    {
                        Console.CursorLeft = 0;
                        Console.CursorTop = cursorTopPosition;
                    }

                    if (settings.Verbose)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{counter}/{loadedBitmaps.Count}] Processing '{loadedBitmap.Path}' ...");
                    }

                    stopWatch.Restart();

                    string[,] finalCharacterMap;
                    string fileProcessedConsoleMessage;

                    if (settings.PrintResultsAsap && settings.ClearScreen)
                    {
                        Console.Clear();
                    }

                    if (settings.NrCharactersInARow > 0 && settings.MatchBrightness)
                    {
                        fileProcessedConsoleMessage = $" ■ Image reduced in size (original size: {loadedBitmap.LoadedWidth}×{loadedBitmap.LoadedHeight}, adjusted size: {loadedBitmap.LoadedBitmap.Width}×{loadedBitmap.LoadedBitmap.Height})";

                        if (settings.Verbose)
                        {
                            Console.WriteLine(fileProcessedConsoleMessage);
                        }

                        // ************************************
                        // Generate the art based on brightness
                        // ************************************
                        finalCharacterMap = AsciiHelper.GenerateAsciiFromBitmapByBrightness(settings, loadedBitmap.LoadedBitmap, alphabetForBrightness);
                    }
                    else
                    {
                        bool[,] imageMatrix = AsciiHelper.GetPixelMapFromBitmap(settings, loadedBitmap.LoadedBitmap, LucidaConsole.CharacterSize.Width, LucidaConsole.CharacterSize.Height);

                        fileProcessedConsoleMessage = $" ■ Pixel map generated from image (original size: {loadedBitmap.LoadedWidth}×{loadedBitmap.LoadedHeight}, adjusted size: {loadedBitmap.LoadedBitmap.Width}×{loadedBitmap.LoadedBitmap.Height})";

                        if (settings.Verbose)
                        {
                            Console.WriteLine(fileProcessedConsoleMessage);
                        }

                        // *************************
                        // Generate the art
                        // *************************
                        finalCharacterMap = AsciiHelper.MapAlphabetOntoBitmap(settings, imageMatrix, alphabet, LucidaConsole.CharacterSize);
                    }

                    stopWatch.Stop();
                    double resultsObtainedMs = stopWatch.Elapsed.TotalMilliseconds;

                    if (settings.Verbose)
                    {
                        Console.WriteLine($" ■ Processed image ({resultsObtainedMs:N0}ms)");
                    }

                    stopWatch.Start();
                    string result =
                        (!string.IsNullOrWhiteSpace(settings.OutputFile) || !settings.PrintResultsAsap) ?
                            AsciiHelper.GenerateAsciiArtString(settings, finalCharacterMap) :
                            string.Empty;

                    if (!string.IsNullOrWhiteSpace(settings.OutputFile))
                    {
                        // Write to file
                        using (StreamWriter sw = new StreamWriter(settings.OutputFile, !firstRun))
                        {
                            sw.WriteLine(consoleContent);
                            sw.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{counter}/{loadedBitmaps.Count}] Processed '{loadedBitmap.Path}' ({resultsObtainedMs:N0}ms)");
                            sw.WriteLine(fileProcessedConsoleMessage);
                            sw.WriteLine(result);
                            sw.WriteLine();
                        }

                        if (settings.Verbose)
                        {
                            Console.WriteLine($" ■ Written to '{settings.OutputFile}' ({stopWatch.Elapsed.TotalMilliseconds:N0}ms)");
                        }
                    }

                    if (!settings.PrintResultsAsap)
                    {
                        if (settings.ClearScreen)
                        {
                            Console.Clear();
                        }

                        Console.WriteLine(result);
                    }

                    stopWatch.Stop();
                    Console.WriteLine($"{1000 / (stopWatch.Elapsed.TotalMilliseconds + settings.DelayBetweenFramesMs):N1} FPS");

                    firstRun = false;
                    Thread.Sleep(settings.DelayBetweenFramesMs);
                }
            }
        }

        private static Dictionary<string, float> GetAlphabetForBrightness(ConverterSettings settings)
        {
            Dictionary<string, float> result = new Dictionary<string, float>();

            foreach (KeyValuePair<string, string[]> simpleLetter in LucidaConsole.Map)
            {
                Letter createdLetter = AsciiHelper.CreateLetterFromFontData(simpleLetter.Key);
                float brightnessRounded = (float)Math.Round((decimal)createdLetter.Brightness, 2);

                if (!result.ContainsValue(brightnessRounded))
                {
                    result.Add(createdLetter.Character, brightnessRounded);
                }
            }

            if (settings.InverseBrightness)
            {
                // Sort values from bright → dark
                result = result.OrderBy(d => d.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            else
            {
                // Sort values from bright → dark
                result = result.OrderByDescending(d => d.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return result;
        }

        private static void PrintAlphabetWithProperties(List<Letter> alphabet)
        {
            foreach (Letter letter in alphabet)
            {
                Console.WriteLine($"{letter.Character}: {letter.Brightness}");
            }
        }
    }
}