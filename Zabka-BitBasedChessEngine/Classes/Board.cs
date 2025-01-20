public class Board
{
    // White pieces
    public Bitboard WhitePawns { get; private set; } = new Bitboard();
    public Bitboard WhiteKnights { get; private set; } = new Bitboard();
    public Bitboard WhiteBishops { get; private set; } = new Bitboard();
    public Bitboard WhiteRooks { get; private set; } = new Bitboard();
    public Bitboard WhiteQueens { get; private set; } = new Bitboard();
    public Bitboard WhiteKing { get; private set; } = new Bitboard();

    // Black pieces
    public Bitboard BlackPawns { get; private set; } = new Bitboard();
    public Bitboard BlackKnights { get; private set; } = new Bitboard();
    public Bitboard BlackBishops { get; private set; } = new Bitboard();
    public Bitboard BlackRooks { get; private set; } = new Bitboard();
    public Bitboard BlackQueens { get; private set; } = new Bitboard();
    public Bitboard BlackKing { get; private set; } = new Bitboard();

    // Combined bitboards
    public Bitboard WhitePieces
    {
        get
        {
            return new Bitboard(
                WhitePawns.Bits |
                WhiteKnights.Bits |
                WhiteBishops.Bits |
                WhiteRooks.Bits |
                WhiteQueens.Bits |
                WhiteKing.Bits
            );
        }
    }

    public Bitboard BlackPieces
    {
        get
        {
            return new Bitboard(
                BlackPawns.Bits |
                BlackKnights.Bits |
                BlackBishops.Bits |
                BlackRooks.Bits |
                BlackQueens.Bits |
                BlackKing.Bits
            );
        }
    }

    public Bitboard AllPieces
    {
        get
        {
            return new Bitboard(WhitePieces.Bits | BlackPieces.Bits);
        }
    }

    // Game state
    public Color SideToMove { get; set; } = Color.White;
    public bool WhiteCanCastleKingSide { get; set; } = true;
    public bool WhiteCanCastleQueenSide { get; set; } = true;
    public bool BlackCanCastleKingSide { get; set; } = true;
    public bool BlackCanCastleQueenSide { get; set; } = true;
    public Square? EnPassantTarget { get; set; } = null;

    // Initialize the board with the standard starting position
    public void Initialize()
    {
        // White pawns on rank 2
        for (Square sq = Square.A2; sq <= Square.H2; sq++)
        {
            WhitePawns.Set(sq);
        }

        // White knights on b1 and g1
        WhiteKnights.Set(Square.B1);
        WhiteKnights.Set(Square.G1);

        // White bishops on c1 and f1
        WhiteBishops.Set(Square.C1);
        WhiteBishops.Set(Square.F1);

        // White rooks on a1 and h1
        WhiteRooks.Set(Square.A1);
        WhiteRooks.Set(Square.H1);

        // White queen on d1 and king on e1
        WhiteQueens.Set(Square.D1);
        WhiteKing.Set(Square.E1);

        // Black pawns on rank 7
        for (Square sq = Square.A7; sq <= Square.H7; sq++)
        {
            BlackPawns.Set(sq);
        }

        // Black knights on b8 and g8
        BlackKnights.Set(Square.B8);
        BlackKnights.Set(Square.G8);

        // Black bishops on c8 and f8
        BlackBishops.Set(Square.C8);
        BlackBishops.Set(Square.F8);

        // Black rooks on a8 and h8
        BlackRooks.Set(Square.A8);
        BlackRooks.Set(Square.H8);

        // Black queen on d8 and king on e8
        BlackQueens.Set(Square.D8);
        BlackKing.Set(Square.E8);
    }

    // Display the board
    public void Display()
    {
        Console.WriteLine("  +------------------------+");
        for (int rank = 7; rank >= 0; rank--)
        {
            Console.Write((rank + 1) + " | ");
            for (int file = 0; file < 8; file++)
            {
                int sq = rank * 8 + file;
                Console.Write(GetPieceChar((Square)sq) + " ");
            }
            Console.WriteLine("|");
        }
        Console.WriteLine("  +------------------------+");
        Console.WriteLine("    a b c d e f g h");
    }

    // Get the character representation of the piece on a square
    private char GetPieceChar(Square square)
    {
        if (WhitePawns.IsSet(square)) return 'P';
        if (WhiteKnights.IsSet(square)) return 'N';
        if (WhiteBishops.IsSet(square)) return 'B';
        if (WhiteRooks.IsSet(square)) return 'R';
        if (WhiteQueens.IsSet(square)) return 'Q';
        if (WhiteKing.IsSet(square)) return 'K';

        if (BlackPawns.IsSet(square)) return 'p';
        if (BlackKnights.IsSet(square)) return 'n';
        if (BlackBishops.IsSet(square)) return 'b';
        if (BlackRooks.IsSet(square)) return 'r';
        if (BlackQueens.IsSet(square)) return 'q';
        if (BlackKing.IsSet(square)) return 'k';

        return '.';
    }

    // Additional methods to make and unmake moves will be added later
}
