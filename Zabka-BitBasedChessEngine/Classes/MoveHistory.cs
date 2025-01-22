// File: Classes/MoveHistory.cs
public class MoveHistory
{
    public Move Move { get; set; }
    public bool WhiteCanCastleKingSide { get; set; }
    public bool WhiteCanCastleQueenSide { get; set; }
    public bool BlackCanCastleKingSide { get; set; }
    public bool BlackCanCastleQueenSide { get; set; }
    public Square? EnPassantTarget { get; set; }
    public int HalfmoveClock { get; set; }
    public int FullmoveNumber { get; set; }
    public PieceType? CapturedPiece { get; set; }
    public Color SideToMove { get; set; } // Optional: To track the side that made the move
}
