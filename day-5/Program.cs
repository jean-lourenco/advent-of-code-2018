using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace day_5
{
    public class Program
    {
        public static IDictionary<char, char> CorrelationTable { get; } = new Dictionary<char, char>();

        public static void Main(string[] args)
        {
            var lower = Enumerable.Range('a', 'z' - 'a' + 1).Select(x => (char)x).ToArray();
            var upper = Enumerable.Range('A', 'Z' - 'A' + 1).Select(x => (char)x).ToArray();

            foreach (var words in lower.Zip(upper, (l, u) => new { Lower = l, Upper = u }))
            {
                CorrelationTable.Add(words.Upper, words.Lower);
                CorrelationTable.Add(words.Lower, words.Upper);
            }

            var part1 = Part1();
            System.Diagnostics.Debug.Assert(10886 == part1);
            Console.WriteLine($"Part1 - Number of Polymers left: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert(4684 == part2);
            Console.WriteLine($"Part2 - Number of Polymers left (improved): {part2}");
        }

        public static int Part1() =>
            ReducePolymer(File.ReadAllText("input.txt"));

        public static int Part2()
        {
            var input = File.ReadAllText("input.txt");
            
            return CorrelationTable
                .Where(x => x.Key <= 90)
                .Select(x => ReducePolymer(input, x.Key, x.Value))
                .Min();
        }

        public static int ReducePolymer(
            string input, 
            char? unitToIgnore1 = null,
            char? unitToIgnore2 = null)
        {
            char prev, curr;
            var sb = new StringBuilder();

            for (var i = 0; i < input.Length; i++)
            {
                curr = input[i];

                if (curr == unitToIgnore1 || curr == unitToIgnore2)
                    continue;

                if (sb.Length == 0)
                {
                    sb.Append(curr);
                    continue;
                }

                prev = sb[sb.Length - 1];

                if (CorrelationTable[prev] == curr)
                    sb.Remove(sb.Length - 1, 1);
                else
                    sb.Append(curr);
            }
            
            return sb.Length;
        }
    }
}
