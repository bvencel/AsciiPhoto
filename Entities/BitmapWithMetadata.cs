using System.Drawing;

namespace AsciiPhoto.Entities
{
    /// <summary>
    /// Contains the data about how much a letter covers a piece of the processed image.
    /// </summary>
    public class BitmapWithMetadata
    {
        public Bitmap LoadedBitmap { get; set; }

        public int LoadedHeight { get; set; }

        public int LoadedWidth { get; set; }

        public string Path { get; set; }
    }
}