using System;
using System.Collections.Generic;

public class Engine
{
    private MoveGenerator moveGenerator;

    public Engine()
    {
        moveGenerator = new MoveGenerator();
    }

    // Make a move on the board
    public void MakeMove(Board board, Move move)
    {
        // Remove piece from the 'from' square
        RemovePiece(board, move.From);

        // Handle captures
        if (move.IsCapture)
        {
            RemovePiece(board, move.To);
        }

        // Handle promotions
        if (move.Promotion != PieceType.None)
        {
            AddPiece(board, move.To, move.Promotion, board.SideToMove);
        }
        else
        {
            // Move the piece to the 'to' square
            AddPiece(board, move.To, GetPieceType(board, move.From), board.SideToMove);
        }

        // Update pawn bitboards if necessary (e.g., en passant)
        // TODO: Implement en passant and castling

        // Switch side to move
        board.SideToMove = board.SideToMove == Color.White ? Color.Black : Color.White;
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

    private PieceType GetPieceType(Board board, Square square)
    {
        if (board.WhitePawns.IsSet(square)) return PieceType.Pawn;
        if (board.WhiteKnights.IsSet(square)) return PieceType.Knight;
        if (board.WhiteBishops.IsSet(square)) return PieceType.Bishop;
        if (board.WhiteRooks.IsSet(square)) return PieceType.Rook;
        if (board.WhiteQueens.IsSet(square)) return PieceType.Queen;
        if (board.WhiteKing.IsSet(square)) return PieceType.King;

        if (board.BlackPawns.IsSet(square)) return PieceType.Pawn;
        if (board.BlackKnights.IsSet(square)) return PieceType.Knight;
        if (board.BlackBishops.IsSet(square)) return PieceType.Bishop;
        if (board.BlackRooks.IsSet(square)) return PieceType.Rook;
        if (board.BlackQueens.IsSet(square)) return PieceType.Queen;
        if (board.BlackKing.IsSet(square)) return PieceType.King;

        return PieceType.None;
    }

    private void AddPiece(Board board, Square square, PieceType pieceType, Color color)
    {
        switch (pieceType)
        {
            case PieceType.Pawn:
                if (color == Color.White) board.WhitePawns.Set(square);
                else board.BlackPawns.Set(square);
                break;
            case PieceType.Knight:
                if (color == Color.White) board.WhiteKnights.Set(square);
                else board.BlackKnights.Set(square);
                break;
            case PieceType.Bishop:
                if (color == Color.White) board.WhiteBishops.Set(square);
                else board.BlackBishops.Set(square);
                break;
            case PieceType.Rook:
                if (color == Color.White) board.WhiteRooks.Set(square);
                else board.BlackRooks.Set(square);
                break;
            case PieceType.Queen:
                if (color == Color.White) board.WhiteQueens.Set(square);
                else board.BlackQueens.Set(square);
                break;
            case PieceType.King:
                if (color == Color.White) board.WhiteKing.Set(square);
                else board.BlackKing.Set(square);
                break;
        }
    }

    private void RemovePiece(Board board, Square square)
    {
        if (board.WhitePawns.IsSet(square)) board.WhitePawns.Clear(square);
        if (board.WhiteKnights.IsSet(square)) board.WhiteKnights.Clear(square);
        if (board.WhiteBishops.IsSet(square)) board.WhiteBishops.Clear(square);
        if (board.WhiteRooks.IsSet(square)) board.WhiteRooks.Clear(square);
        if (board.WhiteQueens.IsSet(square)) board.WhiteQueens.Clear(square);
        if (board.WhiteKing.IsSet(square)) board.WhiteKing.Clear(square);

        if (board.BlackPawns.IsSet(square)) board.BlackPawns.Clear(square);
        if (board.BlackKnights.IsSet(square)) board.BlackKnights.Clear(square);
        if (board.BlackBishops.IsSet(square)) board.BlackBishops.Clear(square);
        if (board.BlackRooks.IsSet(square)) board.BlackRooks.Clear(square);
        if (board.BlackQueens.IsSet(square)) board.BlackQueens.Clear(square);
        if (board.BlackKing.IsSet(square)) board.BlackKing.Clear(square);
    }
}
