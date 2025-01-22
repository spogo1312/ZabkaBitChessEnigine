// File: Classes/MoveGenerator.cs
using System;
using System.Collections.Generic;

public class MoveGenerator
{
    // Generate all legal knight moves for a given color
    public List<Move> GenerateKnightMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();

        Bitboard knights = color == Color.White ? board.WhiteKnights : board.BlackKnights;
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;

        foreach (Square from in knights.GetSquares())
        {
            ulong attacks = AttackTables.KnightAttacks[(int)from];
            ulong targets = attacks & ~ownPieces.Bits; // Exclude squares occupied by own pieces

            foreach (Square to in SquareFromBits(targets))
            {
                bool isCapture = enemyPieces.IsSet(to);
                moves.Add(new Move
                {
                    From = from,
                    To = to,
                    IsCapture = isCapture
                });
            }
        }

        return moves;
    }

    // Generate all legal king moves for a given color
    public List<Move> GenerateKingMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();

        Bitboard king = color == Color.White ? board.WhiteKing : board.BlackKing;
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;

        // There should only be one king
        Square from = Square.A1; // Default initialization
        foreach (Square sq in king.GetSquares())
        {
            from = sq;
            break;
        }

        ulong attacks = AttackTables.KingAttacks[(int)from];
        ulong targets = attacks & ~ownPieces.Bits; // Exclude squares occupied by own pieces

        foreach (Square to in SquareFromBits(targets))
        {
            bool isCapture = enemyPieces.IsSet(to);
            moves.Add(new Move
            {
                From = from,
                To = to,
                IsCapture = isCapture
            });
        }

        // TODO: Implement Castling

        return moves;
    }

    // Generate all legal pawn moves
    public List<Move> GeneratePawnMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();

        Bitboard pawns = color == Color.White ? board.WhitePawns : board.BlackPawns;
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;
        Bitboard emptySquares = new Bitboard(~board.AllPieces.Bits);

        int direction = color == Color.White ? 8 : -8;
        int startRank = color == Color.White ? 1 : 6;
        int promotionRank = color == Color.White ? 6 : 1;

        foreach (Square from in pawns.GetSquares())
        {
            int fromRank = (int)from / 8;
            int fromFile = (int)from % 8;

            // Single move forward
            int toSq = (int)from + direction;
            if (toSq >= 0 && toSq < 64 && !emptySquares.IsSet((Square)toSq))
            {
                // Square is not empty
            }
            else if (toSq >= 0 && toSq < 64 && emptySquares.IsSet((Square)toSq))
            {
                // Promotion
                int toRank = toSq / 8;
                if (toRank == promotionRank)
                {
                    moves.AddRange(new List<Move>
                    {
                        new Move { From = from, To = (Square)toSq, Promotion = PieceType.Queen, IsCapture = false },
                        new Move { From = from, To = (Square)toSq, Promotion = PieceType.Rook, IsCapture = false },
                        new Move { From = from, To = (Square)toSq, Promotion = PieceType.Bishop, IsCapture = false },
                        new Move { From = from, To = (Square)toSq, Promotion = PieceType.Knight, IsCapture = false }
                    });
                }
                else
                {
                    moves.Add(new Move { From = from, To = (Square)toSq, IsCapture = false });
                }

                // Double move forward from starting rank
                if (fromRank == startRank)
                {
                    int doubleToSq = toSq + direction;
                    if (doubleToSq >= 0 && doubleToSq < 64 && emptySquares.IsSet((Square)doubleToSq))
                    {
                        moves.Add(new Move { From = from, To = (Square)doubleToSq, IsCapture = false });
                    }
                }
            }

            // Captures
            // Capture to the left
            if (fromFile > 0)
            {
                int captureLeftSq = (int)from + direction - 1;
                if (captureLeftSq >= 0 && captureLeftSq < 64 && enemyPieces.IsSet((Square)captureLeftSq))
                {
                    int captureRank = captureLeftSq / 8;
                    if (captureRank == promotionRank)
                    {
                        moves.AddRange(new List<Move>
                        {
                            new Move { From = from, To = (Square)captureLeftSq, Promotion = PieceType.Queen, IsCapture = true },
                            new Move { From = from, To = (Square)captureLeftSq, Promotion = PieceType.Rook, IsCapture = true },
                            new Move { From = from, To = (Square)captureLeftSq, Promotion = PieceType.Bishop, IsCapture = true },
                            new Move { From = from, To = (Square)captureLeftSq, Promotion = PieceType.Knight, IsCapture = true }
                        });
                    }
                    else
                    {
                        moves.Add(new Move { From = from, To = (Square)captureLeftSq, IsCapture = true });
                    }
                }
            }

            // Capture to the right
            if (fromFile < 7)
            {
                int captureRightSq = (int)from + direction + 1;
                if (captureRightSq >= 0 && captureRightSq < 64 && enemyPieces.IsSet((Square)captureRightSq))
                {
                    int captureRank = captureRightSq / 8;
                    if (captureRank == promotionRank)
                    {
                        moves.AddRange(new List<Move>
                        {
                            new Move { From = from, To = (Square)captureRightSq, Promotion = PieceType.Queen, IsCapture = true },
                            new Move { From = from, To = (Square)captureRightSq, Promotion = PieceType.Rook, IsCapture = true },
                            new Move { From = from, To = (Square)captureRightSq, Promotion = PieceType.Bishop, IsCapture = true },
                            new Move { From = from, To = (Square)captureRightSq, Promotion = PieceType.Knight, IsCapture = true }
                        });
                    }
                    else
                    {
                        moves.Add(new Move { From = from, To = (Square)captureRightSq, IsCapture = true });
                    }
                }
            }

            // TODO: En Passant
        }

        return moves;
    }

    public List<Move> GenerateBishopMoves(Board board, Color color)
    {
        // Placeholder
        return new List<Move>();
    }

    // Generate all legal rook moves for a given color
    public List<Move> GenerateRookMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();

        Bitboard rooks = color == Color.White ? board.WhiteRooks : board.BlackRooks;
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;
        Bitboard allPieces = board.AllPieces;

        foreach (Square from in rooks.GetSquares())
        {
            // Iterate through each of the four directions: North, South, East, West
            foreach (int direction in new int[] { 8, -8, 1, -1 })
            {
                int currentSq = (int)from;

                while (true)
                {
                    currentSq += direction;

                    // Check if the new square is within the board boundaries
                    if (currentSq < 0 || currentSq >= 64)
                        break;

                    if (direction == 1 && currentSq % 8 == 0)
                        break; // Moving East from H-file wraps to A-file
                    if (direction == -1 && (currentSq + 1) % 8 == 0)
                        break; // Moving West from A-file wraps to H-file

                    Square to = (Square)currentSq;

                    if (ownPieces.IsSet(to))
                    {
                        // Blocked by own piece; cannot move further in this direction
                        break;
                    }

                    bool isCapture = enemyPieces.IsSet(to);
                    moves.Add(new Move
                    {
                        From = from,
                        To = to,
                        IsCapture = isCapture
                    });

                    if (isCapture)
                    {
                        // Cannot move beyond a capture
                        break;
                    }
                }
            }
        }

        return moves;
    }

    public List<Move> GenerateQueenMoves(Board board, Color color)
    {
        // Placeholder
        return new List<Move>();
    }

    // Generate all legal moves for a given color
    public List<Move> GenerateAllMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();
        moves.AddRange(GenerateKnightMoves(board, color));
        moves.AddRange(GenerateKingMoves(board, color));
        moves.AddRange(GeneratePawnMoves(board, color));
        moves.AddRange(GenerateBishopMoves(board, color));
        moves.AddRange(GenerateRookMoves(board, color));
        moves.AddRange(GenerateQueenMoves(board, color));
        // TODO: Add castling and en passant moves
        return moves;
    }

    // Helper method to convert bitboard to squares
    private IEnumerable<Square> SquareFromBits(ulong bits)
    {
        Bitboard bb = new Bitboard(bits);
        foreach (Square sq in bb.GetSquares())
        {
            yield return sq;
        }
    }
}
