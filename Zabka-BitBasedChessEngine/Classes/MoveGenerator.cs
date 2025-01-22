// File: MoveGenerator.cs
using System;
using System.Collections.Generic;

public class MoveGenerator
{
    // Directions for sliding pieces
    private static readonly int[] KnightMoves = { -17, -15, -10, -6, 6, 10, 15, 17 };
    private static readonly int[] KingMoves = { -9, -8, -7, -1, 1, 7, 8, 9 };
    private static readonly int[] BishopDirections = { -9, -7, 7, 9 };
    private static readonly int[] RookDirections = { -8, -1, 1, 8 };
    private static readonly int[] QueenDirections = { -9, -8, -7, -1, 1, 7, 8, 9 };

    // Generate all legal moves for a given color
    public List<Move> GenerateAllMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();

        // Generate pawn moves
        moves.AddRange(GeneratePawnMoves(board, color));

        // Generate knight moves
        moves.AddRange(GenerateKnightMoves(board, color));

        // Generate bishop moves
        moves.AddRange(GenerateSlidingMoves(board, color, BishopDirections, PieceType.Bishop));

        // Generate rook moves
        moves.AddRange(GenerateSlidingMoves(board, color, RookDirections, PieceType.Rook));

        // Generate queen moves
        moves.AddRange(GenerateSlidingMoves(board, color, QueenDirections, PieceType.Queen));

        // Generate king moves
        moves.AddRange(GenerateKingMoves(board, color));
        //Console.WriteLine($"Generating moves for {color}:");
        //foreach (var move in moves)
        //{
        //    Console.WriteLine($"Move: {move.From} to {move.To}, Capture: {move.IsCapture}, Promotion: {move.Promotion}, Castling: {move.IsCastling}, EnPassant: {move.IsEnPassant}");
        //}

        return moves;
    }

    // Generate pawn moves
    private List<Move> GeneratePawnMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();
        Bitboard pawns = color == Color.White ? board.WhitePawns : board.BlackPawns;
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;

        foreach (Square from in pawns.GetSquares())
        {
            int fromSq = (int)from;
            int file = fromSq % 8; // 0 = 'a', 7 = 'h'

            // Single push
            int singlePushSq = color == Color.White ? fromSq + 8 : fromSq - 8;
            if (singlePushSq >= 0 && singlePushSq < 64)
            {
                Square to = (Square)singlePushSq;
                if (!board.AllPieces.IsSet(to))
                {
                    // Promotion
                    if ((color == Color.White && to >= Square.A8 && to <= Square.H8) ||
                        (color == Color.Black && to >= Square.A1 && to <= Square.H1))
                    {
                        foreach (PieceType promo in new PieceType[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight })
                        {
                            moves.Add(new Move
                            {
                                From = from,
                                To = to,
                                Promotion = promo,
                                IsCapture = false
                            });
                        }
                    }
                    else
                    {
                        // Regular push
                        moves.Add(new Move
                        {
                            From = from,
                            To = to,
                            IsCapture = false
                        });
                    }

                    // Double push using rank-based check
                    int rank = color == Color.White ? fromSq / 8 : fromSq / 8;
                    bool isInitialRank = color == Color.White ? rank == 1 : rank == 6;

                    if (isInitialRank)
                    {
                        int doublePushSq = color == Color.White ? fromSq + 16 : fromSq - 16;
                        Square doubleTo = (Square)doublePushSq;
                        if (!board.AllPieces.IsSet(doubleTo))
                        {
                            Move doublePush = new Move
                            {
                                From = from,
                                To = doubleTo,
                                IsCapture = false
                            };
                            moves.Add(doublePush);
                        }
                    }
                }
            }

            // Captures
            // Left Capture
            if (color == Color.White)
            {
                if (file > 0) // Not on 'a' file
                {
                    int captureSq = fromSq + 7;
                    if (captureSq >= 0 && captureSq < 64)
                    {
                        Square to = (Square)captureSq;
                        if (enemyPieces.IsSet(to))
                        {
                            // Promotion
                            if (to >= Square.A8 && to <= Square.H8)
                            {
                                foreach (PieceType promo in new PieceType[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight })
                                {
                                    moves.Add(new Move
                                    {
                                        From = from,
                                        To = to,
                                        Promotion = promo,
                                        IsCapture = true
                                    });
                                }
                            }
                            else
                            {
                                // Regular capture
                                moves.Add(new Move
                                {
                                    From = from,
                                    To = to,
                                    IsCapture = true
                                });
                            }
                        }
                    }
                }

                // Right Capture
                if (file < 7) // Not on 'h' file
                {
                    int captureSq = fromSq + 9;
                    if (captureSq >= 0 && captureSq < 64)
                    {
                        Square to = (Square)captureSq;
                        if (enemyPieces.IsSet(to))
                        {
                            // Promotion
                            if (to >= Square.A8 && to <= Square.H8)
                            {
                                foreach (PieceType promo in new PieceType[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight })
                                {
                                    moves.Add(new Move
                                    {
                                        From = from,
                                        To = to,
                                        Promotion = promo,
                                        IsCapture = true
                                    });
                                }
                            }
                            else
                            {
                                // Regular capture
                                moves.Add(new Move
                                {
                                    From = from,
                                    To = to,
                                    IsCapture = true
                                });
                            }
                        }
                    }
                }
            }
            else // Black
            {
                if (file > 0) // Not on 'a' file
                {
                    int captureSq = fromSq - 9;
                    if (captureSq >= 0 && captureSq < 64)
                    {
                        Square to = (Square)captureSq;
                        if (enemyPieces.IsSet(to))
                        {
                            // Promotion
                            if (to >= Square.A1 && to <= Square.H1)
                            {
                                foreach (PieceType promo in new PieceType[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight })
                                {
                                    moves.Add(new Move
                                    {
                                        From = from,
                                        To = to,
                                        Promotion = promo,
                                        IsCapture = true
                                    });
                                }
                            }
                            else
                            {
                                // Regular capture
                                moves.Add(new Move
                                {
                                    From = from,
                                    To = to,
                                    IsCapture = true
                                });
                            }
                        }
                    }
                }

                if (file < 7) // Not on 'h' file
                {
                    int captureSq = fromSq - 7;
                    if (captureSq >= 0 && captureSq < 64)
                    {
                        Square to = (Square)captureSq;
                        if (enemyPieces.IsSet(to))
                        {
                            // Promotion
                            if (to >= Square.A1 && to <= Square.H1)
                            {
                                foreach (PieceType promo in new PieceType[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight })
                                {
                                    moves.Add(new Move
                                    {
                                        From = from,
                                        To = to,
                                        Promotion = promo,
                                        IsCapture = true
                                    });
                                }
                            }
                            else
                            {
                                // Regular capture
                                moves.Add(new Move
                                {
                                    From = from,
                                    To = to,
                                    IsCapture = true
                                });
                            }
                        }
                    }
                }
            }

            // En Passant
            if (board.EnPassantTarget.HasValue)
            {
                Square epSquare = board.EnPassantTarget.Value;
                int epSq = (int)epSquare;
                if (color == Color.White)
                {
                    if (epSq == fromSq + 7 && (file > 0))
                    {
                        moves.Add(new Move
                        {
                            From = from,
                            To = epSquare,
                            IsCapture = true,
                            IsEnPassant = true
                        });
                    }
                    if (epSq == fromSq + 9 && (file < 7))
                    {
                        moves.Add(new Move
                        {
                            From = from,
                            To = epSquare,
                            IsCapture = true,
                            IsEnPassant = true
                        });
                    }
                }
                else
                {
                    if (epSq == fromSq - 7 && (file < 7))
                    {
                        moves.Add(new Move
                        {
                            From = from,
                            To = epSquare,
                            IsCapture = true,
                            IsEnPassant = true
                        });
                    }
                    if (epSq == fromSq - 9 && (file > 0))
                    {
                        moves.Add(new Move
                        {
                            From = from,
                            To = epSquare,
                            IsCapture = true,
                            IsEnPassant = true
                        });
                    }
                }
            }
        }

        return moves;
    }

    // Generate knight moves
    private List<Move> GenerateKnightMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();
        Bitboard knights = color == Color.White ? board.WhiteKnights : board.BlackKnights;
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;

        foreach (Square from in knights.GetSquares())
        {
            int fromSq = (int)from;
            foreach (int offset in KnightMoves)
            {
                int toSq = fromSq + offset;
                if (toSq < 0 || toSq >= 64)
                    continue;

                // Prevent wrap-around
                if (Math.Abs((fromSq % 8) - (toSq % 8)) > 2)
                    continue;

                Square to = (Square)toSq;
                if (!ownPieces.IsSet(to))
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
        }

        return moves;
    }

    // Generate king moves, including castling
    private List<Move> GenerateKingMoves(Board board, Color color)
    {
        List<Move> moves = new List<Move>();
        Bitboard kings = color == Color.White ? board.WhiteKing : board.BlackKing;
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;

        foreach (Square from in kings.GetSquares())
        {
            int fromSq = (int)from;
            foreach (int offset in KingMoves)
            {
                int toSq = fromSq + offset;
                if (toSq < 0 || toSq >= 64)
                    continue;

                // Prevent wrap-around
                if (Math.Abs((fromSq % 8) - (toSq % 8)) > 1)
                    continue;

                Square to = (Square)toSq;
                if (!ownPieces.IsSet(to))
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

            // Castling
            if (color == Color.White && from == Square.E1)
            {
                // Kingside castling
                if (board.WhiteCanCastleKingSide &&
                    !board.AllPieces.IsSet(Square.F1) &&
                    !board.AllPieces.IsSet(Square.G1) &&
                    !IsSquareAttacked(board, Square.E1, Color.Black) &&
                    !IsSquareAttacked(board, Square.F1, Color.Black) &&
                    !IsSquareAttacked(board, Square.G1, Color.Black))
                {
                    moves.Add(new Move
                    {
                        From = Square.E1,
                        To = Square.G1,
                        IsCastling = true
                    });
                }

                // Queenside castling
                if (board.WhiteCanCastleQueenSide &&
                    !board.AllPieces.IsSet(Square.D1) &&
                    !board.AllPieces.IsSet(Square.C1) &&
                    !board.AllPieces.IsSet(Square.B1) &&
                    !IsSquareAttacked(board, Square.E1, Color.Black) &&
                    !IsSquareAttacked(board, Square.D1, Color.Black) &&
                    !IsSquareAttacked(board, Square.C1, Color.Black))
                {
                    moves.Add(new Move
                    {
                        From = Square.E1,
                        To = Square.C1,
                        IsCastling = true
                    });
                }
            }
            else if (color == Color.Black && from == Square.E8)
            {
                // Kingside castling
                if (board.BlackCanCastleKingSide &&
                    !board.AllPieces.IsSet(Square.F8) &&
                    !board.AllPieces.IsSet(Square.G8) &&
                    !IsSquareAttacked(board, Square.E8, Color.White) &&
                    !IsSquareAttacked(board, Square.F8, Color.White) &&
                    !IsSquareAttacked(board, Square.G8, Color.White))
                {
                    moves.Add(new Move
                    {
                        From = Square.E8,
                        To = Square.G8,
                        IsCastling = true
                    });
                }

                // Queenside castling
                if (board.BlackCanCastleQueenSide &&
                    !board.AllPieces.IsSet(Square.D8) &&
                    !board.AllPieces.IsSet(Square.C8) &&
                    !board.AllPieces.IsSet(Square.B8) &&
                    !IsSquareAttacked(board, Square.E8, Color.White) &&
                    !IsSquareAttacked(board, Square.D8, Color.White) &&
                    !IsSquareAttacked(board, Square.C8, Color.White))
                {
                    moves.Add(new Move
                    {
                        From = Square.E8,
                        To = Square.C8,
                        IsCastling = true
                    });
                }
            }
        }

        return moves;
    }

    // Generate sliding piece moves (Bishop, Rook, Queen)
    private List<Move> GenerateSlidingMoves(Board board, Color color, int[] directions, PieceType pieceType)
    {
        List<Move> moves = new List<Move>();
        Bitboard slidingPieces = GetSlidingPieces(board, color, pieceType);
        Bitboard ownPieces = color == Color.White ? board.WhitePieces : board.BlackPieces;
        Bitboard enemyPieces = color == Color.White ? board.BlackPieces : board.WhitePieces;

        foreach (Square from in slidingPieces.GetSquares())
        {
            int fromSq = (int)from;
            foreach (int direction in directions)
            {
                int toSq = fromSq + direction;
                while (toSq >= 0 && toSq < 64)
                {
                    // Prevent wrap-around for horizontal moves using toSq
                    if (IsInvalidSlidingMove(toSq, direction))
                        break;

                    Square to = (Square)toSq;
                    if (ownPieces.IsSet(to))
                        break;
                    bool isCapture = enemyPieces.IsSet(to);
                    moves.Add(new Move
                    {
                        From = from,
                        To = to,
                        IsCapture = isCapture
                    });

                    if (isCapture)
                        break;

                    toSq += direction;
                }
            }
        }

        return moves;
    }

    // Helper to determine if a sliding move wraps around the board horizontally
    private bool IsInvalidSlidingMove(int toSq, int direction)
    {
        // Moving left
        if (direction == -1 || direction == -9 || direction == 7)
        {
            // If toSq is on the H-file after a left move, it's invalid
            if ((toSq % 8) == 7)
                return true;
        }
        // Moving right
        if (direction == 1 || direction == 9 || direction == -7)
        {
            // If toSq is on the A-file after a right move, it's invalid
            if ((toSq % 8) == 0)
                return true;
        }
        return false;
    }

    // Get sliding pieces based on piece type
    private Bitboard GetSlidingPieces(Board board, Color color, PieceType pieceType)
    {
        Bitboard sliding = new Bitboard();
        switch (pieceType)
        {
            case PieceType.Bishop:
                sliding = new Bitboard(color == Color.White ? board.WhiteBishops.Bits : board.BlackBishops.Bits);
                break;
            case PieceType.Rook:
                sliding = new Bitboard(color == Color.White ? board.WhiteRooks.Bits : board.BlackRooks.Bits);
                break;
            case PieceType.Queen:
                sliding = new Bitboard(color == Color.White ? board.WhiteQueens.Bits : board.BlackQueens.Bits);
                break;
            default:
                sliding = new Bitboard();
                break;
        }
        return sliding;
    }

    // Check if a square is attacked by the opponent
    private bool IsSquareAttacked(Board board, Square square, Color attackerColor)
    {
        // Implementing this function is essential for move legality (e.g., castling)
        // For simplicity, here's a basic implementation. For full correctness, it needs to cover all attack types.

        int sq = (int)square;

        // Pawn attacks
        Bitboard enemyPawns = attackerColor == Color.White ? board.WhitePawns : board.BlackPawns;
        int[] pawnAttackOffsets = attackerColor == Color.White ? new int[] { -9, -7 } : new int[] { 7, 9 };
        foreach (int offset in pawnAttackOffsets)
        {
            int fromSq = sq + offset;
            if (fromSq >= 0 && fromSq < 64)
            {
                Square from = (Square)fromSq;
                if (enemyPawns.IsSet(from))
                    return true;
            }
        }

        // Knight attacks
        Bitboard enemyKnights = attackerColor == Color.White ? board.WhiteKnights : board.BlackKnights;
        foreach (Square from in enemyKnights.GetSquares())
        {
            int fromSq = (int)from;
            foreach (int offset in KnightMoves)
            {
                int toSq = fromSq + offset;
                if (toSq == sq)
                    return true;
            }
        }

        // Bishop and Queen (diagonals)
        Bitboard enemyBishops = attackerColor == Color.White ? board.WhiteBishops : board.BlackBishops;
        Bitboard enemyQueens = attackerColor == Color.White ? board.WhiteQueens : board.BlackQueens;
        foreach (Square from in enemyBishops.GetSquares())
        {
            if (IsSlidingAttack(board, from, square, BishopDirections))
                return true;
        }
        foreach (Square from in enemyQueens.GetSquares())
        {
            if (IsSlidingAttack(board, from, square, BishopDirections) ||
                IsSlidingAttack(board, from, square, RookDirections))
                return true;
        }

        // Rook and Queen (straight lines)
        Bitboard enemyRooks = attackerColor == Color.White ? board.WhiteRooks : board.BlackRooks;
        foreach (Square from in enemyRooks.GetSquares())
        {
            if (IsSlidingAttack(board, from, square, RookDirections))
                return true;
        }
        foreach (Square from in enemyQueens.GetSquares())
        {
            if (IsSlidingAttack(board, from, square, RookDirections) ||
                IsSlidingAttack(board, from, square, BishopDirections))
                return true;
        }

        // King attacks
        Bitboard enemyKings = attackerColor == Color.White ? board.WhiteKing : board.BlackKing;
        foreach (Square from in enemyKings.GetSquares())
        {
            int fromSq = (int)from;
            foreach (int offset in KingMoves)
            {
                int toSq = fromSq + offset;
                if (toSq == sq)
                    return true;
            }
        }

        return false;
    }

    // Helper to check sliding attacks
    private bool IsSlidingAttack(Board board, Square from, Square target, int[] directions)
    {
        int fromSq = (int)from;
        int targetSq = (int)target;
        foreach (int direction in directions)
        {
            int toSq = fromSq + direction;
            while (toSq >= 0 && toSq < 64)
            {
                if (toSq == targetSq)
                    return true;
                if (board.AllPieces.IsSet((Square)toSq))
                    break;
                toSq += direction;
            }
        }
        return false;
    }
}
