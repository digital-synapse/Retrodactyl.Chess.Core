using System;
using System.Collections.Generic;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public static class Tables
    {
        static Tables()
        {
            king_moves = buildKingTables(true);
            king_moves_inv = buildKingTables(false);
        }
        public static ulong[] king_moves;
        public static ulong[] king_moves_inv;

        private static ulong[] buildKingTables(bool bit)
        {
            ulong[] data = new ulong[64];
            int i = 0;
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var m = get2d(!bit);
                    if (x - 1 > 0) m[y, x - 1] = bit;
                    if (x - 1 > 0 && y - 1 > 0) m[y - 1, x - 1] = bit;
                    if (y - 1 > 0) m[y - 1, x] = bit;
                    if (y - 1 > 0 && x + 1 < 8) m[y - 1, x + 1] = bit;
                    if (x + 1 < 8) m[y, x + 1] = bit;
                    if (y + 1 < 8 && x + 1 < 8) m[y + 1, x + 1] = bit;
                    if (y + 1 < 8) m[y + 1, x] = bit;
                    if (y + 1 < 8 && x - 1 > 0) m[y + 1, x - 1] = bit;
                    data[i++] = getBits(m);
                }
            }
            return data;
        }

        private static bool[,] get2d(bool init)
        {
            var m = new bool[8, 8];
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    m[y, x] = init;
                }
            }
            return m;
        }

        private static ulong getBits(bool[,] matrix)
        {
            ulong result = 0;
            for (var y=0; y<8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    ulong bit = matrix[y, x] ? 1UL : 0UL;
                    result = (result << 1) | bit;
                }
            }
            return result;
        }
    }
}
