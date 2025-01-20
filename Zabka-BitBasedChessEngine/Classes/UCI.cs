using System;
using System.Collections.Generic;
using System.IO;

public class UCIHandler
{
    private Board board;
    private MoveGenerator moveGenerator;
    private Engine engine;

    public UCIHandler(Board board, MoveGenerator moveGenerator, Engine engine)
    {
        this.board = board;
        this.moveGenerator = moveGenerator;
        this.engine = engine;
    }

    public void HandleInput()
    {
        string input;
        while ((input = Console.ReadLine()) != null)
        {
            if (input == "uci")
            {
                Console.WriteLine("id name BitboardChessEngine");
                Console.WriteLine("id author YourName");
                Console.WriteLine("uciok");
            }
            else if (input.StartsWith("isready"))
            {
                Console.WriteLine("readyok");
            }
            else if (input.StartsWith("ucinewgame"))
            {
                board.Initialize();
            }
            else if (input.StartsWith("position"))
            {
                ParsePosition(input);
            }
            else if (input.StartsWith("go"))
            {
                ParseGo(input);
            }
            else if (input.StartsWith("quit"))
            {
                break;
            }
            // Handle other UCI commands as needed
        }
    }

    private void ParsePosition(string input)
    {
        // Example: position startpos moves e2e4 e7e5
        if (input.Contains("startpos"))
        {
            board.Initialize();
            int movesStart = input.IndexOf("moves");
            if (movesStart != -1)
            {
                string movesStr = input.Substring(movesStart + 6);
                string[] moves = movesStr.Split(' ');
                foreach (var moveStr in moves)
                {
                    Move move = ParseMove(moveStr);
                    engine.MakeMove(board, move);
                }
            }
        }
        else if (input.Contains("fen"))
        {
            // Implement FEN parsing if needed
            // TODO: Add FEN parsing to set up arbitrary positions
        }
    }

    private void ParseGo(string input)
    {
        // Implement search parameters (depth, time, etc.)
        // For simplicity, we'll start a basic search
        Move bestMove = engine.FindBestMove(board, board.SideToMove);
        Console.WriteLine($"bestmove {bestMove}");
    }

    private Move ParseMove(string moveStr)
    {
        // Parse a move in UCI format (e.g., e2e4, e7e8q)
        if (moveStr.Length < 4)
            throw new ArgumentException("Invalid move string");

        Square from = ParseSquare(moveStr.Substring(0, 2));
        Square to = ParseSquare(moveStr.Substring(2, 2));
        PieceType promotion = PieceType.None;

        if (moveStr.Length > 4)
        {
            char promoChar = moveStr[4];
            promotion = promoChar switch
            {
                'q' => PieceType.Queen,
                'r' => PieceType.Rook,
                'b' => PieceType.Bishop,
                'n' => PieceType.Knight,
                _ => PieceType.None
            };
        }

        // Determine if it's a capture (optional, for more accurate move representation)
        bool isCapture = false;
        // You can enhance this by checking if the 'to' square is occupied by an enemy piece

        return new Move
        {
            From = from,
            To = to,
            Promotion = promotion,
            IsCapture = isCapture
        };
    }

    private Square ParseSquare(string squareStr)
    {
        if (squareStr.Length != 2)
            throw new ArgumentException("Invalid square string");

        char file = squareStr[0];
        char rank = squareStr[1];

        int fileNum = file - 'a';
        int rankNum = rank - '1';

        if (fileNum < 0 || fileNum > 7 || rankNum < 0 || rankNum > 7)
            throw new ArgumentException("Invalid square string");

        return (Square)(rankNum * 8 + fileNum);
    }
}
