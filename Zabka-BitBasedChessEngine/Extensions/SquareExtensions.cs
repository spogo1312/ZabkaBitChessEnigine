public static class SquareExtensions
{
    // Converts a square to standard chess notation (e.g., E2)
    public static string ToChessNotation(this Square square)
    {
        int sq = (int)square;
        char file = (char)('a' + (sq % 8));
        int rank = (sq / 8) + 1;
        return $"{file}{rank}";
    }
}
