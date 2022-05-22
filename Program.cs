using AsciiPhoto.Entities;
using AsciiPhoto.Enums;
using AsciiPhoto.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
        /// <param name="alphabet">Define the characters that the used alphabet is allowed to contain. If a character is in this string, then if will be used. Empty to use all available characters.</param>
        /// <param name="brightnessOffset">Increase or decrease the brightness of the read pixel before processing it.</param>
        /// <param name="minPixelDarkness">The minimum darkness of a pixel, expressed in percent to make a pixel register. Below this value pixels are considered empty/white.</param>
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
        /// <param name="refreshSources">
        /// If true, each frame will be reread from the source, instead of being used from memory.
        /// This is useful for cases when the source is being modified and needs to be displayed.
        /// Static sources should use false for this setting.
        /// </param>
        /// <param name="returnToStart">If true, the new result will start on position 0,0 of the console.</param>
        /// <param name="screenNr">Not used at the moment, but it was originally used when screen was recorded (source = 4).</param>
        /// <param name="source">The input method: Folder = 1, File = 2, Screen = 4</param>
        /// <param name="verbose">
        /// If true, all information will be written in the console, otherwise only the results will be written.
        /// File content is not affected, that is always fully verbose.
        /// </param>
        /// <param name="verticalOffset"></param>
        /// <param name="weightOffsetPercent"></param>
        /// <param name="weightTotalPixelNumberPercent"></param>
        public static void Main(
            string alphabet = "",
            decimal brightnessOffset = 0.05m,
            decimal minPixelDarkness = 0.85m,
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
            bool refreshSources = false,
            bool returnToStart = false,
            int screenNr = 0,
            InputSources source = InputSources.NotSet,
            bool verbose = true,
            int verticalOffset = 2,
            int weightOffsetPercent = 0,
            int weightTotalPixelNumberPercent = 100)
        {
            ConverterSettings settings = new()
            {
                Alphabet = alphabet,
                BrightnessOffset = (float)brightnessOffset,
                MinPixelDarkness = (float)minPixelDarkness,
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
                RefreshSources = refreshSources,
                ReturnToStart = returnToStart,
                ScreenNr = screenNr,
                Source = source,
                Verbose = verbose,
                VerticalOffset = verticalOffset,
                WeightOffset = MathHelper.PercentToDecimal(weightOffsetPercent),
                WeightTotalPixelNumber = MathHelper.PercentToDecimal(weightTotalPixelNumberPercent),
            };

            AdjustSettings(settings);

            Console.OutputEncoding = Encoding.Unicode;
            string asciiTable = AsciiHelper.GetAsciiTableAsUnicode();
            Console.WriteLine(asciiTable);

            ////CreateArt(settings);
        }

        /// <summary>
        /// Tweaks the setting values to make sure they are valid, after the ConverterSettings were constructed.
        /// </summary>
        /// <param name="settings">The originally constructed settings.</param>
        [Pure]
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

        /// <summary>
        /// Creates the art, the result of the app.
        /// </summary>
        /// <param name="settings">The settings.</param>
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

            StringBuilder consoleContent = new();

            ////consoleContent.AppendLine($"Using settings {settings}");

            // Generate letter collection
            Stopwatch stopWatch = Stopwatch.StartNew();
            string alphabetString = string.Join(string.Empty, LucidaConsole.GetFilteredMap(settings.Alphabet).Keys);

            // Letters with bitmaps
            List<Letter> alphabet = AsciiHelper.GenerateAlphabetWithMap(settings);

            if (alphabet.Count == 0)
            {
                consoleContent.AppendLine($"Could not find any font data for the defined alphaber '{settings.Alphabet}'");
                return;
            }

            // Letters for brightness
            Dictionary<Letter, float> alphabetForBrightness = GetAlphabetForBrightness2(settings);

            if (settings.PrintFontMatrices)
            {
                PrintAlphabetWithProperties(alphabet);
            }

            stopWatch.Stop();

            consoleContent.AppendLine($"Using characters: '{alphabetString}'");
            consoleContent.AppendLine($"Loaded {alphabet.Count} fonts ({stopWatch.Elapsed.TotalMilliseconds:N0}ms)");

            stopWatch.Restart();
            List<BitmapWithMetadata> loadedBitmaps = new();

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
                else
                {
                    if (settings.RefreshSources)
                    {
                        loadedBitmaps = FileHelper.LoadBitmapsOriginalSize(settings, consoleContent);

                        if (loadedBitmaps == null || loadedBitmaps.Count == 0)
                        {
                            Console.WriteLine("Could not load any bitmaps inside loop");
                            return;
                        }
                    }
                }

                if (loadedBitmaps == null || loadedBitmaps.Count == 0)
                {
                    Console.WriteLine("Could not load any bitmaps");
                    return;
                }

                foreach (BitmapWithMetadata loadedBitmap in loadedBitmaps)
                {
                    if (loadedBitmap is null)
                    {
                        Console.WriteLine("Skipped null bitmap");
                        continue;
                    }

                    if (loadedBitmap.LoadedBitmap is null)
                    {
                        Console.WriteLine("Skipped bitmap with null LoadedBitmap");
                        continue;
                    }

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

                    Letter[,] finalCharacterMap = new Letter[0, 0];
                    string fileProcessedConsoleMessage = string.Empty;

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
                        decimal[,] imageMatrix = AsciiHelper.GetPixelMapFromBitmap(settings, loadedBitmap.LoadedBitmap, LucidaConsole.CharacterSize.Width, LucidaConsole.CharacterSize.Height);

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

                    // Final art as text
                    string resultForFileRaw =
                        (!string.IsNullOrWhiteSpace(settings.OutputFile) || !settings.PrintResultsAsap) ?
                            AsciiHelper.GenerateAsciiArtString(settings, finalCharacterMap) :
                            string.Empty;

                    string resultForFile = resultForFileRaw;

                    if (!string.IsNullOrWhiteSpace(settings.OutputFile))
                    {
                        // Write to file
                        using (StreamWriter sw = new(settings.OutputFile, !firstRun, Encoding.ASCII))
                        {
                            string hidratedText = HidrateWithHtml(resultForFile);
                            sw.WriteLine(hidratedText);
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

                        WriteToConsole(settings, finalCharacterMap);
                    }

                    stopWatch.Stop();
                    Console.WriteLine($"{1000 / (stopWatch.Elapsed.TotalMilliseconds + settings.DelayBetweenFramesMs):N1} FPS");

                    firstRun = false;
                    Thread.Sleep(settings.DelayBetweenFramesMs);
                }
            }
        }

        private static Dictionary<Letter, float> GetAlphabetForBrightness2(ConverterSettings settings)
        {
            Dictionary<Letter, float> result = new();

            foreach (KeyValuePair<string, string[]> simpleLetter in LucidaConsole.GetFilteredMap(settings.Alphabet))
            {
                Letter createdLetter = AsciiHelper.CreateLetterFromFontData(settings, simpleLetter.Key);
                float brightnessRounded = (float)Math.Round((decimal)createdLetter.CharacterBrightnessBasedOnPixels, 2);

                bool brightnessExists = result.Where(r => r.Value == brightnessRounded).Any();

                if (!brightnessExists)
                {
                    result.Add(createdLetter, brightnessRounded);
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

        private static string HidrateWithHtml(string text)
        {
            StringBuilder result = new();
            result.AppendLine("<html><head><style>body{font-size: 10px; font-family: Lucida Console} div{width:12px;display: inline-block}</style></head><body>");

            result.AppendLine($"<pre>{text}</pre>");

            foreach (string line in new LineReader(() => new StringReader(text)))
            {
                foreach (char c in line)
                {
                    result.Append($"<div>{EmojiOverlay.ApplyOverlay(c.ToString().Replace(" ", "&nbsp;", StringComparison.OrdinalIgnoreCase))}</div>");
                }

                result.AppendLine("<br />");
            }

            result.AppendLine("</body></html>");
            return result.ToString();
        }

        private static void PrintAlphabetWithProperties(List<Letter> alphabet)
        {
            foreach (Letter letter in alphabet)
            {
                Console.WriteLine($"{letter.Character}: {letter.CharacterBrightnessBasedOnPixels}");
            }
        }

        private static void WriteToConsole(ConverterSettings settings, Letter[,] letterMatrixToWriteToConsole)
        {
            ConsoleColor origTextColor = Console.ForegroundColor;

            if (settings.Verbose)
            {
                Console.ForegroundColor = origTextColor;
                Console.WriteLine($"    {AsciiHelper.DecorStart}");
            }

            for (int y = 0; y < letterMatrixToWriteToConsole.GetLength(1); y++)
            {
                if (settings.Verbose)
                {
                    Console.ForegroundColor = origTextColor;

                    // Indent, so there are 3 character wide numbers
                    Console.Write($"{y + 1,3} │ ");
                }

                for (int x = 0; x < letterMatrixToWriteToConsole.GetLength(0); x++)
                {
                    Console.ForegroundColor = letterMatrixToWriteToConsole[x, y].TextColor ?? origTextColor;
                    Console.Write(letterMatrixToWriteToConsole[x, y].Character);
                }

                if (settings.Verbose)
                {
                    Console.ForegroundColor = origTextColor;
                    Console.Write(" │");
                }

                Console.WriteLine();
            }

            if (settings.Verbose)
            {
                Console.WriteLine(AsciiHelper.DecorEnd.Indent(letterMatrixToWriteToConsole.GetLength(0) - AsciiHelper.DecorEnd.Length + 8, ' '));
            }
        }
    }
}

///// Console.WriteLine(@"        ,                              ");
///// Console.WriteLine(@"       ▐Γ ∙                            ");
///// Console.WriteLine(@"       `█ ▐▌                           ");
///// Console.WriteLine(@"       ■▀ ▐'                           ");
///// Console.WriteLine(@"      `▐ ▌                             ");
///// Console.WriteLine(@"     ▐█▀▀▀▀▀█   ,▄█▄,                  ");
///// Console.WriteLine(@"      ▀▐█ █▌▀  ▄█▀ '█                  ");
///// Console.WriteLine(@"       ▐█ █▌ ■█▀     ▀▄                ");
///// Console.WriteLine(@"       ▐█ █▄█▀'       ▀█               ");
///// Console.WriteLine(@"       ▐█_█▀'  ▄▀█▀▄   `█,             ");
///// Console.WriteLine(@"       ▐█▀'   ▐█▄█▄▐█   `█             ");
///// Console.WriteLine(@"     ,▄▀      ▐█ ▌ ▐Γ    `█,           ");
///// Console.WriteLine(@"    ▄▀'       `▀▀▀▀°      `█,          ");
///// Console.WriteLine(@"  ■█▄▄▄▄▄▄▄▄▄▄▄___________ `█■         ");
///// Console.WriteLine(@"  `°T°T█`  ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█▀▀▀         ");
///// Console.WriteLine(@"       █▌       ▐█▀▀█▀▀█▌▐█            ");
///// Console.WriteLine(@"       █▌       ▐▌  █Γ █▌▐█            ");
///// Console.WriteLine(@"       █▌ ▄▄▄_¬ ▐▌▀▀█▀▀█Γ▐█  _  ▄▄  ■▄ ");
///// Console.WriteLine(@"       █▌ █▌  █ █▌ ▐█  █ ▐█_▄█▌▐▀█Γ▐██ ");
///// Console.WriteLine(@"     ▄▄█▌ █▌ _█ ▀▀▀▀▀▀▀▀▀▐███▐██▌▐██▌█ ");
///// Console.WriteLine(@"   ▐█▀▐█▌ █Γ°▐█          ▐███▐█▐▌▐_█∩█ ");
///// Console.WriteLine(@"▄▄■█_███▌_█__▐█__________██▄█▐██▌▐▄█▐▌ ");
///// Console.WriteLine(@"``````▀▀▀█▀▀▀▌▀▀▀▀▀▀▀▀▀▀▀▀''''''''''   ");
///// Console.WriteLine(@"          ▌  ▐▌                        ");
///// Console.WriteLine(@"          ▌   █,                       ");
///// Console.WriteLine(@"          █    ▀▄▄_                    ");
///// Console.WriteLine(@"          `▀■∙+⌐+_▀▀▀▄__               ");
///// Console.WriteLine(@"            `▀▄▄_∙∙╜∙-═▀▀▄_            ");
///// Console.WriteLine(@"               `*▀▀▄_`°___█▄,          ");
///// Console.WriteLine(@"                    ▀▄*═⌐╕⌐═█,         ");
///// Console.WriteLine(@"                     `▌,__═⌐∙█         ");
///// Console.WriteLine(@"                      █└──∙  ▐         ");
///// Console.WriteLine(@"                     √▌ ___  ▐▌        ");
///// Console.WriteLine(@"■∙══∙══∙══∙══∙══∙══∙══∙══∙══∙══∙══∙══∙■");
///// Console.WriteLine(@"∙                                     ║");
///// Console.WriteLine(@"║  CONGRATULATIONS ON SELLING         ∙");
///// Console.WriteLine(@"∙  the▐█                              ║");
///// Console.WriteLine(@"║     ▐█▄▄▄▄  ▄▄▄▄  ▄▄▄▄▄▄▄▄■  ▄▀▀▄   ∙");
///// Console.WriteLine(@"∙     ▐█  ▐█ █▌  ██ █▌ ▐█  ██ ██▄▄█▌  ║");
///// Console.WriteLine(@"║     ▐█  ▐█ ██__██ █▌ ▐█  ▐█ ██▄__   ∙");
///// Console.WriteLine(@"∙      ▀   ▀   ▀▀▀  ▀  └▀   ▀   ▀▀▀   ║");
///// Console.WriteLine(@"║                                     ∙");
///// Console.WriteLine(@"■∙══∙══∙══∙══∙══∙══∙══∙══∙══∙══∙══∙══∙■");
///// Console.WriteLine(@"               ▐▌,=══╖ ═¬_U▌           ");
///// Console.WriteLine(@"              √▌ /   |/  /▐▌           ");
///// Console.WriteLine(@"              █  ∙═══ ──═ ▐            ");
///// Console.WriteLine(@"             ▐▌   -~─══Γ  █            ");
///// Console.WriteLine(@"             ▐   |____/   ▌            ");
///// Console.WriteLine(@"             █ =╛⌐═∙ ,═══ █            ");
///// Console.WriteLine(@"             █ `∙¬__-∙__/_▐▌           ");
///// Console.WriteLine(@"             ▐ (``* ∙ ╒∙* \▀▄          ");
///// Console.WriteLine(@"             `█ `¬══___'∙,∙ '▀▄_       ");
///// Console.WriteLine(@"              `▀, (   `)  _-⌐*'|▀▄     ");
///// Console.WriteLine(@"                ▀▄,∙═_∙,∙╕|____╛ `▀■   ");
///// Console.WriteLine(@"                 `▀▄, (   \`° ╥──¬ ▐▌  ");
///// Console.WriteLine(@"                   `▀▄ \ ,∙∙  └═══┘ █  ");
///// Console.WriteLine(@"                     ╙▌ '           ▐▌ ");
///// Console.WriteLine(@"                      █              █ ");
///// Console.WriteLine(@"                      █              █ ");
///// Console.WriteLine(@"                      ▀              ▀ ");