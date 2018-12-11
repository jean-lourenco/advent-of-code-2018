using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace day_1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1();
            System.Diagnostics.Debug.Assert(416 == part1);
            Console.WriteLine($"Part 1: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert(56752 == part2);
            Console.WriteLine($"Part 2: {part2}");
        }

        public static int Part1() => File
            .ReadAllText("input.txt")
            .Split(Environment.NewLine)
            .Select(int.Parse)
            .Sum();

        public static int Part2()
        {
            var list = File
                .ReadAllText("input.txt")
                .Split(Environment.NewLine)
                .Select(int.Parse);
            
            var seed = 0;
            var set = new HashSet<int>();

            while (true) 
            {
                foreach (var item in list)
                {
                    seed += item;

                    if (set.Contains(seed))
                        return seed;
                    else 
                        set.Add(seed);
                }
            }
        } 
    }
}