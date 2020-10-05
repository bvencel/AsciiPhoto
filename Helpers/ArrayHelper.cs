using System;

namespace AsciiPhoto.Helpers
{
    public static class ArrayHelper
    {
        /// <summary>
        /// Returns false if at least one item is true in the matrix.
        /// Used to skip processing empty rectangles.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool AllIsFalse(bool[,] matrix)
        {
            if (matrix is null)
            {
                return true;
            }

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool AllIsZero(decimal[,] matrix)
        {
            if (matrix is null)
            {
                return true;
            }

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[x, y] > 0m)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static T[,] ExtractSubMatrix<T>(T[,] matrix, int colStart, int colEnd, int rowStart, int rowEnd)
        {
            if (matrix is null)
            {
                return null;
            }

            int sizeCol = colEnd - colStart;
            int sizeRow = rowEnd - rowStart;

            T[,] result = new T[sizeCol, sizeRow];

            for (int j = rowStart; j < Math.Min(rowEnd, matrix.GetLength(1)); j++)
            {
                for (int i = colStart; i < Math.Min(colEnd, matrix.GetLength(0)); i++)
                {
                    int x = i - colStart;
                    int y = j - rowStart;

                    result[x, y] = matrix[i, j];
                }
            }

            return result;
        }

        public static T[,] ShiftMatrix<T>(T[,] originalMatrix, int colOffset, int rowOffset)
        {
            if (originalMatrix is null)
            {
                return null;
            }

            T[,] result = new T[originalMatrix.GetLength(0), originalMatrix.GetLength(1)];

            for (int y = 0; y < originalMatrix.GetLength(1); y++)
            {
                for (int x = 0; x < originalMatrix.GetLength(0); x++)
                {
                    bool needsFillWithDefault =
                        (x - colOffset < 0) ||
                        (x - colOffset >= originalMatrix.GetLength(0)) ||
                        (y - rowOffset < 0) ||
                        (y - rowOffset >= originalMatrix.GetLength(1));

                    if (!needsFillWithDefault)
                    {
                        result[x, y] = originalMatrix[x - colOffset, y - rowOffset];
                    }
                }
            }

            return result;
        }
    }
}