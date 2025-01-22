// File: Classes/UCIHandler.cs
using System;
using System.Collections.Generic;

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
            // Extract the FEN string
            int fenStart = input.IndexOf("fen") + 4; // Position after 'fen '
            int fenEnd = input.IndexOf(" moves");
            string fen;
            if (fenEnd == -1)
            {
                fen = input.Substring(fenStart).Trim();
            }
            else
            {
                fen = input.Substring(fenStart, fenEnd - fenStart).Trim();
            }

            ParseFEN(fen);

            // Apply moves if any
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
    }

    private void ParseFEN(string fen)
    {
        // Split the FEN string into its components
        string[] parts = fen.Split(' ');
        if (parts.Length != 6)
        {
            throw new ArgumentException("Invalid FEN string: Incorrect number of fields.");
        }

        string piecePlacement = parts[0];
        string activeColor = parts[1];
        string castlingAvailability = parts[2];
        string enPassant = parts[3];
        string halfmoveClock = parts[4];
        string fullmoveNumber = parts[5];

        // Clear the board before setting up the new position
        board.ClearAllPieces();

        // Parse piece placement
        string[] ranks = piecePlacement.Split('/');
        if (ranks.Length != 8)
        {
            throw new ArgumentException("Invalid FEN string: Incorrect number of ranks.");
        }

        for (int rank = 7; rank >= 0; rank--)
        {
            int file = 0;
            foreach (char c in ranks[7 - rank])
            {
                if (char.IsDigit(c))
                {
                    int emptySquares = c - '0';
                    file += emptySquares;
                }
                else
                {
                    if (file >= 8)
                    {
                        throw new ArgumentException($"Invalid FEN string: Too many squares in rank {rank + 1}.");
                    }

                    Square square = (Square)(rank * 8 + file);
                    board.SetPiece(c, square);
                    file++;
                }
            }

            if (file != 8)
            {
                throw new ArgumentException($"Invalid FEN string: Incomplete rank {rank + 1}.");
            }
        }

        // Parse active color
        board.SideToMove = activeColor == "w" ? Color.White : Color.Black;

        // Parse castling availability
        board.WhiteCanCastleKingSide = castlingAvailability.Contains("K");
        board.WhiteCanCastleQueenSide = castlingAvailability.Contains("Q");
        board.BlackCanCastleKingSide = castlingAvailability.Contains("k");
        board.BlackCanCastleQueenSide = castlingAvailability.Contains("q");

        // Parse en passant target square
        if (enPassant != "-")
        {
            board.EnPassantTarget = ParseSquare(enPassant);
        }
        else
        {
            board.EnPassantTarget = null;
        }

        // Optionally, parse halfmove clock and fullmove number
        // These can be stored in the Board class if needed
    }

    private void ParseGo(string input)
    {
        // Implement search parameters (depth, time, etc.)
        // For simplicity, we'll start a basic search
        Move bestMove = engine.FindBestMove(board, board.SideToMove);
        Console.WriteLine($"bestmove {MoveToUCI(bestMove)}");
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

        // Determine if it's a capture by checking if the 'to' square is occupied by an enemy piece
        bool isCapture = false;
        Bitboard enemyPieces = board.SideToMove == Color.White ? board.BlackPieces : board.WhitePieces;
        isCapture = enemyPieces.IsSet(to);

        return new Move
        {
            From = from,
            To = to,
            Promotion = promotion,
            IsCapture = isCapture
        };
    }

    private string MoveToUCI(Move move)
    {
        // Convert a Move object to UCI string format
        string uciMove = $"{SquareToString(move.From)}{SquareToString(move.To)}";
        if (move.Promotion != PieceType.None)
        {
            uciMove += move.Promotion.ToString().ToLower()[0];
        }
        return uciMove;
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

    private string SquareToString(Square square)
    {
        int sq = (int)square;
        char file = (char)('a' + (sq % 8));
        char rank = (char)('1' + (sq / 8));
        return $"{file}{rank}";
    }
}
