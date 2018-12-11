using System;
using System.Collections.Generic;
using System.Linq;

namespace day_11
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var (x1, y1) = Part1(out var cells);
            System.Diagnostics.Debug.Assert(x1 == 20 && y1 == 37);
            Console.WriteLine($"Part1 - The X,Y coordinates of the top-left cell: {x1},{y1}");

            var (x2, y2, size) = Part2(cells);
            //System.Diagnostics.Debug.Assert(x2 == 0 && y2 == 0 && size == 0);
            Console.WriteLine($"Part2 - The X,Y,Size coordinates of the top-left cell: {x2},{y2},{size}");
        }

        public static (int, int) Part1(out int[,] cells)
        {
            var serial = 7689;
            cells = new int[300, 300];
            var sumRelations = new Dictionary<(int, int), int>();

            for (int y = 0; y < cells.GetUpperBound(0); y++)
            {
                for (int x = 0; x < cells.GetUpperBound(1); x++)
                {
                    var rackId = x + 1 + 10;
                    var powerLevel = rackId * (y + 1) + serial;
                    powerLevel *= rackId;
                    powerLevel = Math.Abs(powerLevel / 100 % 10) - 5;
                    cells[y, x] = powerLevel;
                }
            }

            for (int y = 0; y < cells.GetUpperBound(0); y++)
            {
                for (int x = 0; x < cells.GetUpperBound(1); x++)
                {
                    if (x + 2 > 299 || y + 2 > 299)
                        continue;

                    sumRelations.Add((x, y),
                        cells[y, x] + cells[y + 1, x + 1] + cells[y + 2, x + 2]
                        + cells[y + 1, x] + cells[y + 2, x] + cells[y + 2, x + 1]
                        + cells[y, x + 1] + cells[y, x + 2] + cells[y + 1, x + 2]);
                }
            }

            var (x1, y1) = sumRelations.OrderBy(s => s.Value).Last().Key;
            return (x1 + 1, y1 + 1);
        }

        public static (int, int, int) Part2(int[,] cells)
        {
            var sumRelations = new Dictionary<(int, int, int), int>();
            Console.WriteLine(cells[0, 289]);
            for (var y = 0; y < cells.GetUpperBound(0); y++)
            {
                for (var x = 0; x < cells.GetUpperBound(1); x++)
                {
                    var size = 0;
                    var value = 0;

                    for (int xSlice = x, ySlice = y; xSlice < 299 && ySlice < 299; ySlice = xSlice += 1)
                    {
                        value += cells[ySlice, xSlice];

                        for (int x1 = x; x1 < ySlice; x1++)
                            value += cells[ySlice, x1];

                        for (int y1 = y; y1 < xSlice; y1++)
                            value += cells[y1, xSlice];

                        size++;
                        sumRelations.Add((x, y, size  * size), value);
                    }

                }
            }

            var (a,b,c) = sumRelations.OrderBy(d => d.Value).Last().Key;
            Console.WriteLine($"{a+1},{b+1},{c}");

            var (x2, y2, size2) = sumRelations.OrderBy(s => s.Value).Last().Key;
            return (x2 + 1, y2 + 1, size2);
        }
    }
}
