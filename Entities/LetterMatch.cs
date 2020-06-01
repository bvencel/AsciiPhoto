using System;

namespace AsciiPhoto.Entities
{
    /// <summary>
    /// Contains the data about how much a letter covers a piece of the processed image.
    /// </summary>
    internal class LetterMatch
    {
        public LetterMatch(decimal weightOffset, decimal weightTotalPixelNumber, Letter matchedLetter, int nrMatchingBlackPixels)
        {
            MatchedLetter = matchedLetter;
            int totalOffset = Math.Abs(MatchedLetter.HorizontalOffset) + Math.Abs(MatchedLetter.VerticalOffset);
            Score = CalculateMatchScore(weightOffset, weightTotalPixelNumber, matchedLetter, nrMatchingBlackPixels, totalOffset);
            ////ScoreComposition = $"{nrMatchingBlackPixels} - (({matchedLetter.PixelCountInOriginal} - {nrMatchingBlackPixels}) * {weightTotalPixelNumber}) - ({totalOffset} * {weightOffset}) = {Score}";
        }

        public Letter MatchedLetter { get; set; }

        public decimal Score { get; }

        ////private string ScoreComposition { get; }

        public static decimal CalculateMatchScore(decimal weightOffset, decimal weightTotalPixelNumber, Letter matchedLetter, int nrMatchingBlackPixels, int totalOffset)
        {
            return
                nrMatchingBlackPixels -

                // The larger the character, the smaller the score
                // Do not count the already matches
                ((matchedLetter.PixelCountInOriginal - nrMatchingBlackPixels) * weightTotalPixelNumber) -
                (totalOffset * weightOffset);
        }

        public override string ToString()
        {
            ////return ScoreComposition;
            return $"'{MatchedLetter.Character}' - {Score}";
        }
    }
}