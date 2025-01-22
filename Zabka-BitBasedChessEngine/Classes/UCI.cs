// File: UCIHandler.cs
using System;
using System.Collections.Generic;

public class UCIHandler
{
    private Board board;
    private MoveGenerator moveGenerator;
    private Engine engine;
    private bool isDebugMode = false; // Debug mode flag

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
            try
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
                    if (isDebugMode)
                        Console.WriteLine("New game initialized.");
                }
                else if (input.StartsWith("position"))
                {
                    ParsePosition(input);
                    if (isDebugMode)
                    {
                        Console.WriteLine("Position set:");
                        board.Display();
                    }
                }
                else if (input.StartsWith("go"))
                {
                    ParseGo(input);
                }
                else if (input.StartsWith("quit"))
                {
                    break;
                }
                else if (input.StartsWith("display"))
                {
                    // Handle the display command
                    board.Display();
                }
                else if (input.StartsWith("debug on"))
                {
                    isDebugMode = true;
                    Console.WriteLine("Debug mode enabled.");
                }
                else if (input.StartsWith("debug off"))
                {
                    isDebugMode = false;
                    Console.WriteLine("Debug mode disabled.");
                }
                else if (input.StartsWith("perft"))
                {
                    // Handle the perft command
                    HandlePerftCommand(input);
                }
                else if (input.StartsWith("divide"))
                {
                    // Handle the divide command
                    HandleDivideCommand(input);
                }
                else
                {
                    // Handle unrecognized commands
                    if (isDebugMode)
                    {
                        Console.WriteLine($"Unrecognized command: {input}");
                    }
                    // Alternatively, silently ignore
                }
            }
            catch (Exception ex)
            {
                if (isDebugMode)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                // Optionally, handle or log the error
            }
        }
    }

    private void HandlePerftCommand(string input)
    {
        // Example: perft 3
        string[] parts = input.Split(' ');
        if (parts.Length != 2 || !int.TryParse(parts[1], out int depth))
        {
            Console.WriteLine("Usage: perft <depth>");
            return;
        }

        Console.WriteLine($"Running Perft({depth})...");
        ulong nodes = engine.Perft(board, depth);
        Console.WriteLine($"Perft({depth}) = {nodes}");
    }

    private void HandleDivideCommand(string input)
    {
        // Example: divide 3
        string[] parts = input.Split(' ');
        if (parts.Length != 2 || !int.TryParse(parts[1], out int depth))
        {
            Console.WriteLine("Usage: divide <depth>");
            return;
        }

        Console.WriteLine($"Running Divide({depth})...");
        Divide(board, depth);
    }

    private void Divide(Board board, int depth)
    {
        if (depth < 1)
        {
            Console.WriteLine("Depth must be at least 1.");
            return;
        }

        List<Move> moves = moveGenerator.GenerateAllMoves(board, board.SideToMove);
        ulong totalNodes = 0;

        foreach (var move in moves)
        {
            board.MakeMove(move);
            ulong nodes = engine.Perft(board, depth - 1);
            board.UnmakeMove();

            Console.WriteLine($"{move.ToString()} {nodes}");
            totalNodes += nodes;
        }

        Console.WriteLine($"Total nodes: {totalNodes}");
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

        // Parse halfmove clock and fullmove number
        if (int.TryParse(halfmoveClock, out int halfmoves))
        {
            board.HalfmoveClock = halfmoves;
        }
        else
        {
            throw new ArgumentException("Invalid FEN string: Halfmove clock is not an integer.");
        }

        if (int.TryParse(fullmoveNumber, out int fullmoves))
        {
            board.FullmoveNumber = fullmoves;
        }
        else
        {
            throw new ArgumentException("Invalid FEN string: Fullmove number is not an integer.");
        }
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
        if (board.SideToMove == Color.White)
        {
            isCapture = board.BlackPieces.IsSet(to);
        }
        else
        {
            isCapture = board.WhitePieces.IsSet(to);
        }

        // Handle en passant capture
        bool isEnPassant = false;
        if (board.EnPassantTarget.HasValue && to == board.EnPassantTarget.Value)
        {
            isCapture = true;
            isEnPassant = true;
        }

        // Determine if it's a castling move
        bool isCastling = false;
        if (board.SideToMove == Color.White)
        {
            if (from == Square.E1 && to == Square.G1) isCastling = true; // Kingside
            if (from == Square.E1 && to == Square.C1) isCastling = true; // Queenside
        }
        else
        {
            if (from == Square.E8 && to == Square.G8) isCastling = true; // Kingside
            if (from == Square.E8 && to == Square.C8) isCastling = true; // Queenside
        }

        return new Move
        {
            From = from,
            To = to,
            Promotion = promotion,
            IsCapture = isCapture,
            IsEnPassant = isEnPassant,
            IsCastling = isCastling
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
