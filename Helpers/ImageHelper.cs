using AsciiPhoto.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AsciiPhoto.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Decides if a pixel should be treated as black, while processing an image.
        /// The images are treated as 1 bit, because everything is either black or white.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="color"></param>
        /// <returns>True, if the color is dark enough to be treated as black, otherwise false (white).</returns>
        public static bool ColorIsDarkEnough(ConverterSettings settings, Color color)
        {
            if (settings is null)
            {
                return false;
            }

            return color.Name != "0" && color.GetBrightness() < settings.BrightnessThreshold;
        }

        public static float GetBrightness(Color color)
        {
            if (color.Name == "0")
            {
                return 1;
            }

            return color.GetBrightness();
        }

        public static List<BitmapWithMetadata> LoadScreenshot(ConverterSettings settings)
        {
            using Bitmap screenshot = ImageHelper.TakeScreenshot();
            Bitmap resizedBitmap = ImageHelper.ResizeLoadedBitmap(settings, out _, screenshot);

            BitmapWithMetadata b = new BitmapWithMetadata()
            {
                LoadedBitmap = resizedBitmap,
                LoadedHeight = screenshot.Height,
                LoadedWidth = screenshot.Width,
                Path = "Screenshot",
            };

            List<BitmapWithMetadata> result = new List<BitmapWithMetadata>();
            result.Add(b);

            return result;
        }

        public static Bitmap ResizeLoadedBitmap(ConverterSettings settings, out string adjustmentApproach, Bitmap bitmapToResize)
        {
            adjustmentApproach = string.Empty;

            if (settings is null)
            {
                return null;
            }

            if (bitmapToResize is null)
            {
                return null;
            }

            int newWidth;
            int newHeight;

            // Resize the loaded image to compensate for the fact that text characters have an extra pixel between the rows on the console, more pixels in editors.
            if (settings.NrCharactersInARow > 0)
            {
                if (settings.MatchBrightness)
                {
                    newWidth = settings.NrCharactersInARow;

                    double ratio = (double)bitmapToResize.Width / newWidth;

                    // One character covers 2 vertical pixels
                    newHeight = (int)(bitmapToResize.Height / ratio / 2);

                    adjustmentApproach = $"targeting a row width of {settings.NrCharactersInARow} characters, matching brightness";
                }
                else
                {
                    newWidth = Math.Min(bitmapToResize.Width, settings.NrCharactersInARow * LucidaConsole.CharacterSize.Width);

                    double ratio = (double)bitmapToResize.Width / newWidth;
                    double unadjustedNewHeight = bitmapToResize.Height / ratio;
                    newHeight = (int)(unadjustedNewHeight - (unadjustedNewHeight / LucidaConsole.CharacterSize.Height * settings.PixelsBetweenRows));

                    adjustmentApproach = $"targeting a row width of {settings.NrCharactersInARow} characters";
                }
            }
            else
            {
                newWidth = (int)(bitmapToResize.Width * settings.ProcessedImageSizeRatio);
                newHeight = (int)((bitmapToResize.Height * settings.ProcessedImageSizeRatio) - (bitmapToResize.Height * settings.ProcessedImageSizeRatio / LucidaConsole.CharacterSize.Height * settings.PixelsBetweenRows));

                adjustmentApproach = $"applying a ratio of {settings.ProcessedImageSizeRatio}";
            }

            Bitmap result = new Bitmap(bitmapToResize, newWidth, newHeight);

            return result;
        }

        public static Bitmap TakeScreenshot()
        {
            int width = 1900;
            int height = 1000;

            // Create a new bitmap.
            var bmpScreenshot = new Bitmap(
                width,
                height);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(
                0,
                0,
                0,
                0,
                new Size(width, height),
                CopyPixelOperation.SourceCopy);

            return bmpScreenshot;
        }
    }
}