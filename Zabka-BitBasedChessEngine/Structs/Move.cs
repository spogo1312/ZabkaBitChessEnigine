public struct Move
{
    public Square From { get; set; }
    public Square To { get; set; }
    public PieceType Promotion { get; set; } = PieceType.None;
    public bool IsCapture { get; set; } = false;
    public bool IsEnPassant { get; set; } = false;
    public bool IsCastling { get; set; } = false;

    // Explicit constructor
    public Move(Square from, Square to, PieceType promotion = PieceType.None, bool isCapture = false, bool isEnPassant = false, bool isCastling = false)
    {
        From = from;
        To = to;
        Promotion = promotion;
        IsCapture = isCapture;
        IsEnPassant = isEnPassant;
        IsCastling = isCastling;
    }
    public override string ToString()
    {
        string moveStr = $"{From.ToChessNotation()}-{To.ToChessNotation()}";
        if (Promotion != PieceType.None)
        {
            moveStr += $"={Promotion.ToString()[0]}";
        }
        return moveStr;
    }
}
