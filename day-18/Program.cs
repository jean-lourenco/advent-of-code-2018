using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace day_18
{
    public class Program
    {
        public static Dictionary<char, int> Neighbours = new Dictionary<char, int>()
        {
            {'|', 0}, {'#', 0}, {'.', 0}
        };

        public static void Main(string[] args)
        {
            var part1 = Part1And2(10);
            System.Diagnostics.Debug.Assert(360720 == part1);
            Console.WriteLine($"Part1 - Product of woods and lumberyards after 10 min: {part1}");

            var part2 = Part1And2(524);
            System.Diagnostics.Debug.Assert(197276 == part2);
            Console.WriteLine($"Part2 - Product of woods and lumberyards after 1000000000 min: {part2}");
        }

        public static int Part1And2(int minutes)
        {
            var map = File
                .ReadAllLines("input.txt")
                .Select(line =>
                    line.Select(letter => letter).ToArray())
                .ToArray();

            var snapshot = new char[map.Length][];
            for (var y = 0; y < map.Length; y++)
                snapshot[y] = new char[map[0].Length];

            for (var i = 0; i < minutes; i ++)
            {
                CopyArray(map, snapshot);

                for (var y = 0; y < map.Length; y++)
                {
                    for (var x = 0; x < map[0].Length; x++)
                    {
                        var tile = snapshot[y][x];
                        var neig = GetNeighbours(snapshot, x, y);

                        if (tile == '.')
                            map[y][x] = neig['|'] >= 3 ? '|' : '.';
                        else if (tile == '|')
                            map[y][x] = neig['#'] >= 3 ? '#' : '|';
                        else
                            map[y][x] = neig['|'] >= 1 && neig['#'] >= 1 ? '#' : '.';
                    }
                }
            }

            var woods = 0;
            var lumberyards = 0;
            for (var y = 0; y < map.Length; y++)
            {
                for (var x = 0; x < map[0].Length; x++)
                {
                    if (map[y][x] == '#')
                        lumberyards++;
                    else if (map[y][x] == '|')
                        woods++;
                }
            }

            return woods * lumberyards;
        }

        public static IDictionary<char, int> GetNeighbours(
            char[][] map, 
            int x, 
            int y)
        {
            Neighbours['#'] = 0;
            Neighbours['|'] = 0;
            Neighbours['.'] = 0;

            // down
            if (y + 1 < map.Length)
                Neighbours[map[y+1][x]]++;

            //up
            if (y - 1 >= 0)
                Neighbours[map[y - 1][x]]++;

            //left
            if (x - 1 >= 0)
                Neighbours[map[y][x - 1]]++;

            //up
            if (x + 1 < map[0].Length)
                Neighbours[map[y][x + 1]]++;

            //up-left
            if (y - 1 >= 0 && x - 1 >= 0)
                Neighbours[map[y - 1][x - 1]]++;

            //up-right
            if (y - 1 >= 0 && x + 1 < map[0].Length)
                Neighbours[map[y - 1][x + 1]]++;

            //down-left
            if (y + 1 < map.Length && x - 1 >= 0)
                Neighbours[map[y + 1][x - 1]]++;

            //down-right
            if (y + 1 < map.Length && x + 1 < map[0].Length)
                Neighbours[map[y + 1][x + 1]]++;

            return Neighbours;
        }

        public static void CopyArray(char[][] source, char[][] snapshot)
        {
            for (var y = 0; y < source.Length; y++)
            {
                for (var x = 0; x < source[0].Length; x++)
                    snapshot[y][x] = source[y][x];
            }
        }
    }
}
