namespace AsciiPhoto.Entities
{
    /// <summary>
    /// Represents a letter that needs to be tested for every part of the processed image.
    /// </summary>
    internal class Letter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Letter"/> class.
        /// </summary>
        public Letter(string character, bool[,] pixelMap, int horizontalOffset, int verticalOffset, int pixelCountInOriginal)
        {
            Character = character;
            PixelMap = pixelMap;
            HorizontalOffset = horizontalOffset;
            VerticalOffset = verticalOffset;
            PixelCountInOriginal = pixelCountInOriginal;

            PixelMapFlat = FlattenMatrix(pixelMap);
            FirstBlackPixelStartInFlatMap = GetFirstTrue(PixelMapFlat);
            LastBlackPixelStartInFlatMap = GetLastTrue(PixelMapFlat);

            Brightness = 1f - ((float)CountTrueValues(PixelMap) / (float)(PixelMap.GetLength(0) * PixelMap.GetLength(1)));
            BrightnessBottom = 1f - (CountTrueValuesBottom(PixelMap) / 56f);
            BrightnessTop = 1f - (CountTrueValuesTop(PixelMap) / 56f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Letter"/> class.
        /// If this is the original letter, it can count its own black pixels.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="pixelMap"></param>
        /// <param name="horizontalOffset"></param>
        /// <param name="verticalOffset"></param>
        public Letter(string character, bool[,] pixelMap, int horizontalOffset, int verticalOffset)
            : this(character, pixelMap, horizontalOffset, verticalOffset, 0)
        {
            PixelCountInOriginal = CountTrueValues(PixelMap);
        }

        public float Brightness { get; }

        public float BrightnessBottom { get; }

        public float BrightnessTop { get; }

        public string Character { get; set; }

        /// <summary>
        /// Used to avoid comparing the values from <see cref="PixelMapFlat"/> before this position.
        /// </summary>
        public int FirstBlackPixelStartInFlatMap { get; set; }

        public int HorizontalOffset { get; set; }

        /// <summary>
        /// Used to avoid comparing the values from <see cref="PixelMapFlat"/> after this position.
        /// </summary>
        public int LastBlackPixelStartInFlatMap { get; set; }

        /// <summary>
        /// When calculating score, using the number of black pixels, the black pixels of the original letter is the one that matters.
        /// </summary>
        public int PixelCountInOriginal { get; set; }

        /// <summary>
        /// True means that the pixel is black.
        /// </summary>
        public bool[,] PixelMap { get; set; }

        /// <summary>
        /// Flattened matrix.
        /// True means that the pixel is black.
        /// </summary>
        public bool[] PixelMapFlat { get; set; }

        public int VerticalOffset { get; set; }

        public static int CountTrueValues(bool[,] matrix)
        {
            int result = 0;

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[x, y])
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public static int CountTrueValuesBottom(bool[,] matrix)
        {
            int result = 0;

            for (int y = matrix.GetLength(1) / 2; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[x, y])
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public static int CountTrueValuesTop(bool[,] matrix)
        {
            int result = 0;

            for (int y = 0; y < matrix.GetLength(1) / 2; y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[x, y])
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public static T[] FlattenMatrix<T>(T[,] matrix)
        {
            T[] result = new T[matrix.GetLength(0) * matrix.GetLength(1)];

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    result[x + (y * matrix.GetLength(0))] = matrix[x, y];
                }
            }

            return result;
        }

        /// <summary>
        /// Finds the first 'true' value's position in a bool array.
        /// </summary>
        /// <param name="pixelMapFlat"></param>
        /// <returns></returns>
        public static int GetFirstTrue(bool[] pixelMapFlat)
        {
            if (pixelMapFlat.Length == 0)
            {
                return 0;
            }

            for (int i = 0; i < pixelMapFlat.Length; i++)
            {
                if (pixelMapFlat[i])
                {
                    return i;
                }
            }

            return pixelMapFlat.Length - 1;
        }

        /// <summary>
        /// Finds the last 'true' value's position in a bool array.
        /// </summary>
        /// <param name="pixelMapFlat"></param>
        /// <returns></returns>
        public static int GetLastTrue(bool[] pixelMapFlat)
        {
            if (pixelMapFlat.Length == 0)
            {
                return 0;
            }

            for (int i = pixelMapFlat.Length - 1; i >= 0; i--)
            {
                if (pixelMapFlat[i])
                {
                    return i;
                }
            }

            return 0;
        }
    }
}