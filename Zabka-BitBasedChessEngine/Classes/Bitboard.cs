using System;
using System.Collections.Generic;
using System.Numerics;

public class Bitboard
{
    public ulong Bits { get; private set; }

    public Bitboard(ulong bits = 0)
    {
        Bits = bits;
    }

    // Set a bit at a specific square
    public void Set(Square square)
    {
        Bits |= 1UL << (int)square;
    }

    // Clear a bit at a specific square
    public void Clear(Square square)
    {
        Bits &= ~(1UL << (int)square);
    }

    // Toggle a bit at a specific square
    public void Toggle(Square square)
    {
        Bits ^= 1UL << (int)square;
    }

    // Check if a bit is set at a specific square
    public bool IsSet(Square square)
    {
        return (Bits & (1UL << (int)square)) != 0;
    }

    // Count the number of set bits
    public int Count()
    {
        return BitOperations.PopCount(Bits);
    }

    // Iterate through all set bits
    public IEnumerable<Square> GetSquares()
    {
        ulong bb = Bits;
        while (bb != 0)
        {
            int lsb = BitOperations.TrailingZeroCount(bb);
            yield return (Square)lsb;
            bb &= bb - 1; // Remove the least significant bit set
        }
    }

    // Print the bitboard (for debugging)
    public void Print()
    {
        for (int rank = 7; rank >= 0; rank--)
        {
            for (int file = 0; file < 8; file++)
            {
                int sq = rank * 8 + file;
                Console.Write(((Bits & (1UL << sq)) != 0) ? "1 " : ". ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    // Bitwise AND
    public Bitboard And(Bitboard other)
    {
        return new Bitboard(this.Bits & other.Bits);
    }

    // Bitwise OR
    public Bitboard Or(Bitboard other)
    {
        return new Bitboard(this.Bits | other.Bits);
    }

    // Bitwise XOR
    public Bitboard Xor(Bitboard other)
    {
        return new Bitboard(this.Bits ^ other.Bits);
    }

    // Bitwise NOT
    public Bitboard Not()
    {
        return new Bitboard(~this.Bits);
    }

    // Shift North
    public Bitboard ShiftNorth(int distance = 8)
    {
        return new Bitboard(this.Bits << distance);
    }

    // Shift South
    public Bitboard ShiftSouth(int distance = 8)
    {
        return new Bitboard(this.Bits >> distance);
    }

    // Shift East with file mask
    public Bitboard ShiftEast(int distance = 1)
    {
        return new Bitboard((this.Bits & ~FileH) << distance);
    }

    // Shift West with file mask
    public Bitboard ShiftWest(int distance = 1)
    {
        return new Bitboard((this.Bits & ~FileA) >> distance);
    }

    // Diagonal shifts
    public Bitboard ShiftNorthEast()
    {
        return new Bitboard((this.Bits & ~FileH) << 9);
    }

    public Bitboard ShiftNorthWest()
    {
        return new Bitboard((this.Bits & ~FileA) << 7);
    }

    public Bitboard ShiftSouthEast()
    {
        return new Bitboard((this.Bits & ~FileH) >> 7);
    }

    public Bitboard ShiftSouthWest()
    {
        return new Bitboard((this.Bits & ~FileA) >> 9);
    }

    // File masks
    public static readonly ulong FileA = 0x0101010101010101;
    public static readonly ulong FileH = 0x8080808080808080;

    // Rank masks (optional, useful for certain operations)
    public static readonly ulong Rank1 = 0x00000000000000FF;
    public static readonly ulong Rank8 = 0xFF00000000000000;
}
