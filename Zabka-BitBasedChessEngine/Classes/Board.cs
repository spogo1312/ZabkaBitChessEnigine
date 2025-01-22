// File: Board.cs
using System;
using System.Collections.Generic;

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

    // Additional game state parameters
    public int HalfmoveClock { get; set; } = 0;
    public int FullmoveNumber { get; set; } = 1;

    // Stack to keep track of move history for unmaking moves
    private Stack<MoveHistory> moveHistory = new Stack<MoveHistory>();

    // Initialize the board with the standard starting position
    public void Initialize()
    {
        // Clear all pieces first
        ClearAllPieces();

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

        // Reset game state
        SideToMove = Color.White;
        WhiteCanCastleKingSide = true;
        WhiteCanCastleQueenSide = true;
        BlackCanCastleKingSide = true;
        BlackCanCastleQueenSide = true;
        EnPassantTarget = null;
        HalfmoveClock = 0;
        FullmoveNumber = 1;
    }

    // Clear all piece bitboards
    public void ClearAllPieces()
    {
        WhitePawns = new Bitboard();
        WhiteKnights = new Bitboard();
        WhiteBishops = new Bitboard();
        WhiteRooks = new Bitboard();
        WhiteQueens = new Bitboard();
        WhiteKing = new Bitboard();

        BlackPawns = new Bitboard();
        BlackKnights = new Bitboard();
        BlackBishops = new Bitboard();
        BlackRooks = new Bitboard();
        BlackQueens = new Bitboard();
        BlackKing = new Bitboard();
    }

    // Set a piece on the board based on FEN notation
    public void SetPiece(char pieceChar, Square square)
    {
        switch (pieceChar)
        {
            case 'P':
                WhitePawns.Set(square);
                break;
            case 'N':
                WhiteKnights.Set(square);
                break;
            case 'B':
                WhiteBishops.Set(square);
                break;
            case 'R':
                WhiteRooks.Set(square);
                break;
            case 'Q':
                WhiteQueens.Set(square);
                break;
            case 'K':
                WhiteKing.Set(square);
                break;
            case 'p':
                BlackPawns.Set(square);
                break;
            case 'n':
                BlackKnights.Set(square);
                break;
            case 'b':
                BlackBishops.Set(square);
                break;
            case 'r':
                BlackRooks.Set(square);
                break;
            case 'q':
                BlackQueens.Set(square);
                break;
            case 'k':
                BlackKing.Set(square);
                break;
            default:
                throw new ArgumentException($"Invalid piece character: {pieceChar}");
        }
    }

    // Display the board along with game state information
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

        // Display additional game state information
        Console.WriteLine($"\nSide to move: {(SideToMove == Color.White ? "White" : "Black")}");
        string castlingRights = "";
        if (WhiteCanCastleKingSide) castlingRights += "K";
        if (WhiteCanCastleQueenSide) castlingRights += "Q";
        if (BlackCanCastleKingSide) castlingRights += "k";
        if (BlackCanCastleQueenSide) castlingRights += "q";
        castlingRights = string.IsNullOrEmpty(castlingRights) ? "-" : castlingRights;
        Console.WriteLine($"Castling Rights: {castlingRights}");
        Console.WriteLine($"En Passant Target: {(EnPassantTarget.HasValue ? EnPassantTarget.Value.ToChessNotation() : "-")}");
        Console.WriteLine($"Halfmove Clock: {HalfmoveClock}");
        Console.WriteLine($"Fullmove Number: {FullmoveNumber}");
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

    // Make a move on the board
    // Ensure that after MakeMove, the piece is correctly moved
    public void MakeMove(Move move)
    {
        Color movingColor = SideToMove;

        // Capture the piece if any
        PieceType capturedPiece = GetPiece(move.To);
        if (move.IsEnPassant && movingColor == Color.White)
        {
            // White en passant capture
            Square capturedSquare = (Square)((int)move.To - 8);
            capturedPiece = GetPiece(capturedSquare);
            RemovePiece(capturedSquare);
        }
        else if (move.IsEnPassant && movingColor == Color.Black)
        {
            // Black en passant capture
            Square capturedSquare = (Square)((int)move.To + 8);
            capturedPiece = GetPiece(capturedSquare);
            RemovePiece(capturedSquare);
        }
        else if (move.IsCapture)
        {
            RemovePiece(move.To);
        }

        // Save current state to history
        MoveHistory history = new MoveHistory
        {
            Move = move,
            WhiteCanCastleKingSide = WhiteCanCastleKingSide,
            WhiteCanCastleQueenSide = WhiteCanCastleQueenSide,
            BlackCanCastleKingSide = BlackCanCastleKingSide,
            BlackCanCastleQueenSide = BlackCanCastleQueenSide,
            EnPassantTarget = EnPassantTarget,
            HalfmoveClock = HalfmoveClock,
            FullmoveNumber = FullmoveNumber,
            CapturedPiece = capturedPiece,
            SideToMove = movingColor
        };

        moveHistory.Push(history);

        // Update halfmove clock
        if (IsPawnMove(move) || move.IsCapture)
            HalfmoveClock = 0;
        else
            HalfmoveClock++;

        // Update fullmove number
        if (movingColor == Color.Black)
            FullmoveNumber++;

        // Execute the move
        ExecuteMove(move, movingColor);

        // Switch side to move
        SideToMove = SideToMove == Color.White ? Color.Black : Color.White;
    }



    // Unmake the last move
    public void UnmakeMove()
    {

        if (moveHistory.Count == 0)
            throw new InvalidOperationException("No moves to unmake.");

        MoveHistory history = moveHistory.Pop();

        // Switch side to move back
        SideToMove = history.SideToMove;

        // Revert fullmove number
        if (history.SideToMove == Color.Black)
            FullmoveNumber--;

        // Revert halfmove clock
        HalfmoveClock = history.HalfmoveClock;

        // Revert castling rights
        WhiteCanCastleKingSide = history.WhiteCanCastleKingSide;
        WhiteCanCastleQueenSide = history.WhiteCanCastleQueenSide;
        BlackCanCastleKingSide = history.BlackCanCastleKingSide;
        BlackCanCastleQueenSide = history.BlackCanCastleQueenSide;

        // Revert en passant target
        EnPassantTarget = history.EnPassantTarget;

        // Revert the move
        RevertMove(history.Move, history.CapturedPiece, history.SideToMove);
    }

    // Execute the move on the board
    private void ExecuteMove(Move move, Color movingColor)
    {
        // Retrieve the piece type from the 'from' square BEFORE removing it
        PieceType piece = GetPiece(move.From);

        // Remove piece from 'from' square
        RemovePiece(move.From);

        // Handle promotions
        if (move.Promotion != PieceType.None)
        {
            // Promote the pawn to the specified piece
            SetPiece(move.To, move.Promotion, movingColor);
        }
        else
        {
            // Move the piece to 'to' square
            SetPiece(move.To, piece, movingColor);
        }

        // Handle castling
        if (move.IsCastling)
        {
            // Move the rook accordingly
            if (move.To == Square.G1) // White kingside
            {
                MoveRook(Square.H1, Square.F1, Color.White);
            }
            else if (move.To == Square.C1) // White queenside
            {
                MoveRook(Square.A1, Square.D1, Color.White);
            }
            else if (move.To == Square.G8) // Black kingside
            {
                MoveRook(Square.H8, Square.F8, Color.Black);
            }
            else if (move.To == Square.C8) // Black queenside
            {
                MoveRook(Square.A8, Square.D8, Color.Black);
            }
        }

        // Update castling rights based on the move
        UpdateCastlingRights(move, movingColor);

        // Update en passant target square
        UpdateEnPassantTarget(move, movingColor);
    }



    // Revert the move on the board
    private void RevertMove(Move move, PieceType? capturedPiece, Color movingColor)
    {
        // Move the piece back to 'from' square
        PieceType piece = GetPiece(move.To);
        RemovePiece(move.To);
        SetPiece(move.From, piece, movingColor);

        // Restore captured piece
        if (capturedPiece.HasValue)
        {
            if (move.IsEnPassant && movingColor == Color.White)
            {
                // Restore the captured black pawn
                Square capturedSquare = (Square)((int)move.To - 8);
                SetPiece(capturedSquare, capturedPiece.Value, Color.Black);
            }
            else if (move.IsEnPassant && movingColor == Color.Black)
            {
                // Restore the captured white pawn
                Square capturedSquare = (Square)((int)move.To + 8);
                SetPiece(capturedSquare, capturedPiece.Value, Color.White);
            }
            else if (move.IsCapture)
            {
                // Restore the captured piece on 'to' square
                SetPiece(move.To, capturedPiece.Value, movingColor == Color.White ? Color.Black : Color.White);
            }
        }

        // Handle promotions
        if (move.Promotion != PieceType.None)
        {
            // Revert the promotion by setting back a pawn
            RemovePiece(move.To);
            SetPiece(move.From, PieceType.Pawn, movingColor);
        }

        // Handle castling
        if (move.IsCastling)
        {
            // Move the rook back accordingly
            if (move.To == Square.G1) // White kingside
            {
                MoveRook(Square.F1, Square.H1, Color.White);
            }
            else if (move.To == Square.C1) // White queenside
            {
                MoveRook(Square.D1, Square.A1, Color.White);
            }
            else if (move.To == Square.G8) // Black kingside
            {
                MoveRook(Square.F8, Square.H8, Color.Black);
            }
            else if (move.To == Square.C8) // Black queenside
            {
                MoveRook(Square.D8, Square.A8, Color.Black);
            }
        }
    }

    // Helper method to move the rook during castling
    private void MoveRook(Square from, Square to, Color color)
    {
        PieceType rook = GetPiece(from);
        RemovePiece(from);
        SetPiece(to, rook, color);
    }

    // Update castling rights based on the move
    private void UpdateCastlingRights(Move move, Color movingColor)
    {
        // If king moves, lose both castling rights
        if (movingColor == Color.White)
        {
            if (move.From == Square.E1)
            {
                WhiteCanCastleKingSide = false;
                WhiteCanCastleQueenSide = false;
            }
            if (move.From == Square.H1)
            {
                WhiteCanCastleKingSide = false;
            }
            if (move.From == Square.A1)
            {
                WhiteCanCastleQueenSide = false;
            }
        }
        else
        {
            if (move.From == Square.E8)
            {
                BlackCanCastleKingSide = false;
                BlackCanCastleQueenSide = false;
            }
            if (move.From == Square.H8)
            {
                BlackCanCastleKingSide = false;
            }
            if (move.From == Square.A8)
            {
                BlackCanCastleQueenSide = false;
            }
        }

        // If rook is captured, lose castling rights accordingly
        if (move.IsCapture)
        {
            if (movingColor == Color.White)
            {
                if (move.To == Square.H8)
                {
                    BlackCanCastleKingSide = false;
                }
                if (move.To == Square.A8)
                {
                    BlackCanCastleQueenSide = false;
                }
            }
            else
            {
                if (move.To == Square.H1)
                {
                    WhiteCanCastleKingSide = false;
                }
                if (move.To == Square.A1)
                {
                    WhiteCanCastleQueenSide = false;
                }
            }
        }
    }

    // Update en passant target square based on the move
    private void UpdateEnPassantTarget(Move move, Color movingColor)
    {
        EnPassantTarget = null;

        if (IsPawnDoublePush(move, movingColor))
        {
            int epSquare = (int)move.From + (movingColor == Color.White ? 8 : -8);
            EnPassantTarget = (Square)epSquare;
        }
    }

    // Determine if the move is a pawn double push
    private bool IsPawnDoublePush(Move move, Color movingColor)
    {
        PieceType piece = GetPiece(move.From);
        if (piece != PieceType.Pawn)
            return false;

        if (movingColor == Color.White && move.From >= Square.A2 && move.From <= Square.H2 && move.To >= Square.A4 && move.To <= Square.H4)
            return true;
        if (movingColor == Color.Black && move.From >= Square.A7 && move.From <= Square.H7 && move.To >= Square.A5 && move.To <= Square.H5)
            return true;

        return false;
    }

    // Determine if a move is a pawn move based on the piece type
    private bool IsPawnMove(Move move)
    {
        PieceType piece = GetPiece(move.From);
        return piece == PieceType.Pawn;
    }

    // Get the captured piece type (if any)
    private PieceType? GetCapturedPiece(Move move)
    {
        // If it's en passant, the captured piece is not on the 'to' square
        if (move.IsEnPassant)
        {
            return GetPiece((Square)((int)move.To + (SideToMove == Color.White ? -8 : 8)));
        }

        return GetPiece(move.To);
    }

    // Get the piece type on a given square
    public PieceType GetPiece(Square square)
    {
        if (WhitePawns.IsSet(square)) return PieceType.Pawn;
        if (WhiteKnights.IsSet(square)) return PieceType.Knight;
        if (WhiteBishops.IsSet(square)) return PieceType.Bishop;
        if (WhiteRooks.IsSet(square)) return PieceType.Rook;
        if (WhiteQueens.IsSet(square)) return PieceType.Queen;
        if (WhiteKing.IsSet(square)) return PieceType.King;

        if (BlackPawns.IsSet(square)) return PieceType.Pawn;
        if (BlackKnights.IsSet(square)) return PieceType.Knight;
        if (BlackBishops.IsSet(square)) return PieceType.Bishop;
        if (BlackRooks.IsSet(square)) return PieceType.Rook;
        if (BlackQueens.IsSet(square)) return PieceType.Queen;
        if (BlackKing.IsSet(square)) return PieceType.King;

        return PieceType.None;
    }

    // Set a piece on the board
    public void SetPiece(Square square, PieceType piece, Color color)
    {
        RemovePiece(square);

        switch (piece)
        {
            case PieceType.Pawn:
                if (color == Color.White)
                    WhitePawns.Set(square);
                else
                    BlackPawns.Set(square);
                break;
            case PieceType.Knight:
                if (color == Color.White)
                    WhiteKnights.Set(square);
                else
                    BlackKnights.Set(square);
                break;
            case PieceType.Bishop:
                if (color == Color.White)
                    WhiteBishops.Set(square);
                else
                    BlackBishops.Set(square);
                break;
            case PieceType.Rook:
                if (color == Color.White)
                    WhiteRooks.Set(square);
                else
                    BlackRooks.Set(square);
                break;
            case PieceType.Queen:
                if (color == Color.White)
                    WhiteQueens.Set(square);
                else
                    BlackQueens.Set(square);
                break;
            case PieceType.King:
                if (color == Color.White)
                    WhiteKing.Set(square);
                else
                    BlackKing.Set(square);
                break;
            case PieceType.None:
                // Do nothing
                break;
        }
    }

    // Remove any piece from a square
    public void RemovePiece(Square square)
    {
        WhitePawns.Clear(square);
        WhiteKnights.Clear(square);
        WhiteBishops.Clear(square);
        WhiteRooks.Clear(square);
        WhiteQueens.Clear(square);
        WhiteKing.Clear(square);

        BlackPawns.Clear(square);
        BlackKnights.Clear(square);
        BlackBishops.Clear(square);
        BlackRooks.Clear(square);
        BlackQueens.Clear(square);
        BlackKing.Clear(square);
    }

    // Clear a square (remove any piece)
    private void ClearSquare(Square square)
    {
        RemovePiece(square);
    }
}
