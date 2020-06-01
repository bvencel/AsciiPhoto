using AsciiPhoto.Enums;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace AsciiPhoto
{
    /// <summary>
    /// Holds the settings of the application, collected from command line.
    /// </summary>
    public class ConverterSettings
    {
        /// <summary>
        /// When matching on the brightness of pixels, increase or decrease the brightness of the read pixel before processing it.
        /// </summary>
        public float BrightnessOffset { get; set; }

        public float BrightnessThreshold { get; set; }

        public bool ClearScreen { get; set; }

        public int DelayBetweenFramesMs { get; set; }

        public int HorizontalOffset { get; set; }

        public bool InverseBrightness { get; set; }

        /// <summary>
        /// Number of times the input source should be processed.
        /// </summary>
        public int LoopNr { get; set; }

        public bool MatchBrightness { get; set; }

        public int NrCharactersInARow { get; set; }

        public string OutputFile { get; set; }

        public string Path { get; set; }

        public int PixelsBetweenRows { get; set; }

        public bool PrintFontMatrices { get; set; }

        public bool PrintResultsAsap { get; set; }

        public decimal ProcessedImageSizeRatio { get; set; }

        public bool ReturnToStart { get; set; }

        /// <summary>
        /// Which screen should be used as screen input.
        /// </summary>
        public int ScreenNr { get; set; }

        public InputSources Source { get; set; }

        public bool Verbose { get; set; }

        public int VerticalOffset { get; set; }

        public decimal WeightOffset { get; set; }

        public decimal WeightTotalPixelNumber { get; set; }

        public static ConverterSettings DeserializeFrom(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine($"Parameter '{nameof(fileName)}' was not set while loading initial settings from file");
                return null;
            }

            try
            {
                using StreamReader srSettings = new StreamReader(fileName, Encoding.Unicode);
                string jsonText = srSettings.ReadToEnd();

                ConverterSettings settings = JsonSerializer.Deserialize<ConverterSettings>(jsonText);

                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading initial settings from file '{fileName}'", ex);
                return null;
            }
        }

        public void SerializeToFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine($"Parameter '{nameof(fileName)}' was not set while saving initial settings to file");
                return;
            }

            try
            {
                using StreamWriter swSettings = new StreamWriter(fileName, false, Encoding.Unicode);
                swSettings.WriteLine(ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving initial settings to file '{fileName}'", ex);
            }
        }

        public override string ToString()
        {
            string settingsJson;
            settingsJson = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });

            return settingsJson;
        }
    }
}