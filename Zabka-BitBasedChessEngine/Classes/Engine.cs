// File: Engine.cs
using System;
using System.Collections.Generic;

public class Engine
{
    private MoveGenerator moveGenerator;

    public Engine()
    {
        moveGenerator = new MoveGenerator();
    }
    public void MakeMove(Board board, Move move)
    {
        board.MakeMove(move);
    }

    // Add UnmakeMove method
    public void UnmakeMove(Board board)
    {
        board.UnmakeMove();
    }
    // Find the best move using a simple evaluation (e.g., random move)
    public Move FindBestMove(Board board, Color color)
    {
        List<Move> moves = moveGenerator.GenerateAllMoves(board, color);
        if (moves.Count == 0)
            throw new InvalidOperationException("No legal moves available");

        // Simple evaluation: choose a random move for demonstration
        Random rand = new Random();
        return moves[rand.Next(moves.Count)];
    }

    // Evaluate the board position (simple material count)
    public int Evaluate(Board board)
    {
        int score = 0;

        // Material weights
        Dictionary<PieceType, int> pieceValues = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 100 },
            { PieceType.Knight, 320 },
            { PieceType.Bishop, 330 },
            { PieceType.Rook, 500 },
            { PieceType.Queen, 900 },
            { PieceType.King, 20000 }
        };

        // White material
        score += CountPieces(board.WhitePawns, pieceValues[PieceType.Pawn]);
        score += CountPieces(board.WhiteKnights, pieceValues[PieceType.Knight]);
        score += CountPieces(board.WhiteBishops, pieceValues[PieceType.Bishop]);
        score += CountPieces(board.WhiteRooks, pieceValues[PieceType.Rook]);
        score += CountPieces(board.WhiteQueens, pieceValues[PieceType.Queen]);
        score += CountPieces(board.WhiteKing, pieceValues[PieceType.King]);

        // Black material
        score -= CountPieces(board.BlackPawns, pieceValues[PieceType.Pawn]);
        score -= CountPieces(board.BlackKnights, pieceValues[PieceType.Knight]);
        score -= CountPieces(board.BlackBishops, pieceValues[PieceType.Bishop]);
        score -= CountPieces(board.BlackRooks, pieceValues[PieceType.Rook]);
        score -= CountPieces(board.BlackQueens, pieceValues[PieceType.Queen]);
        score -= CountPieces(board.BlackKing, pieceValues[PieceType.King]);

        return score;
    }

    private int CountPieces(Bitboard bitboard, int value)
    {
        return bitboard.Count() * value;
    }

    // Perft function
    public ulong Perft(Board board, int depth)
    {
        if (depth == 0)
            return 1;

        ulong nodes = 0;
        List<Move> moves = moveGenerator.GenerateAllMoves(board, board.SideToMove);

        foreach (var move in moves)
        {
            board.MakeMove(move);
            nodes += Perft(board, depth - 1);
            board.UnmakeMove();
        }

        return nodes;
    }
}
