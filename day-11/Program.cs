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
            System.Diagnostics.Debug.Assert(x2 == 90 && y2 == 169 && size == 15);
            Console.WriteLine($"Part2 - The X,Y,Size coordinates of the top-left cell: {x2},{y2},{size}");
        }

        public static (int, int) Part1(out int[,] cells)
        {
            var serial = 7689;
            (int X, int Y, int Value) bestArea = default;
            cells = new int[300, 300];

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

                    var value = cells[y, x] + cells[y + 1, x + 1] + cells[y + 2, x + 2]
                        + cells[y + 1, x] + cells[y + 2, x] + cells[y + 2, x + 1]
                        + cells[y, x + 1] + cells[y, x + 2] + cells[y + 1, x + 2];

                    if (value > bestArea.Value)
                        bestArea = (x, y, value);
                }
            }

            return (bestArea.X + 1, bestArea.Y + 1);
        }

        public static (int, int, int) Part2(int[,] cells)
        {
            (int X, int Y , int Size, int Value) bestArea = default;

            for (var y = 0; y < cells.GetUpperBound(0); y++)
            {
                for (var x = 0; x < cells.GetUpperBound(1); x++)
                {
                    var size = 0;
                    var value = 0;

                    for (int xSlice = x, ySlice = y; xSlice < 299 && ySlice < 299;)
                    {
                        value += cells[ySlice, xSlice];

                        for (int x1 = x; x1 < xSlice; x1++)
                            value += cells[ySlice, x1];

                        for (int y1 = y; y1 < ySlice; y1++)
                            value += cells[y1, xSlice];

                        size++;

                        if (value > bestArea.Value)
                            bestArea = (x, y, size, value);

                        ySlice++;
                        xSlice++;
                    }
                }
            }

            return (bestArea.X + 1, bestArea.Y + 1, bestArea.Size);
        }
    }
}
