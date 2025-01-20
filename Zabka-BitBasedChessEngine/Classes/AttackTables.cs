public static class AttackTables
{
    public static readonly ulong[] KnightAttacks = new ulong[64];
    public static readonly ulong[] KingAttacks = new ulong[64];

    static AttackTables()
    {
        for (int sq = 0; sq < 64; sq++)
        {
            KnightAttacks[sq] = GenerateKnightAttacks(sq);
            KingAttacks[sq] = GenerateKingAttacks(sq);
        }
    }

    private static ulong GenerateKnightAttacks(int square)
    {
        int rank = square / 8;
        int file = square % 8;
        ulong attacks = 0;

        // All possible knight moves
        int[] dr = { 2, 1, -1, -2, -2, -1, 1, 2 };
        int[] df = { 1, 2, 2, 1, -1, -2, -2, -1 };

        for (int i = 0; i < 8; i++)
        {
            int newRank = rank + dr[i];
            int newFile = file + df[i];

            if (newRank >= 0 && newRank < 8 && newFile >= 0 && newFile < 8)
            {
                int targetSq = newRank * 8 + newFile;
                attacks |= 1UL << targetSq;
            }
        }

        return attacks;
    }

    private static ulong GenerateKingAttacks(int square)
    {
        int rank = square / 8;
        int file = square % 8;
        ulong attacks = 0;

        // All possible king moves
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int df = -1; df <= 1; df++)
            {
                if (dr == 0 && df == 0)
                    continue;

                int newRank = rank + dr;
                int newFile = file + df;

                if (newRank >= 0 && newRank < 8 && newFile >= 0 && newFile < 8)
                {
                    int targetSq = newRank * 8 + newFile;
                    attacks |= 1UL << targetSq;
                }
            }
        }

        return attacks;
    }

}
