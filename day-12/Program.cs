using System;
using System.IO;
using System.Linq;

namespace day_12
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1();
            System.Diagnostics.Debug.Assert(2542 == part1);
            Console.WriteLine($"Part1 - Sum of all pots in gen 20: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert(2550000000883 == part2);
            Console.WriteLine($"Part2 - Sum of all pots in gen 50000000000: {part2}");
        }

        public static long Part1() => ProcessGenerations(20);

        public static long Part2()
        {
            var gen100Value = ProcessGenerations(100);

            return gen100Value + (51 * (50_000_000_000 - 100));
        }

        public static long ProcessGenerations(int numOfGererations)
        {
            var padding = numOfGererations * 3;
            var input = File.ReadAllLines("input.txt");
            var initialState = input[0].Substring(15);
            var state = CreateArray(initialState.Length + padding, '.');
            var newState = CreateArray(initialState.Length + padding, '.');

            var rules = input.Skip(2).ToDictionary(
                i => new ValueTuple<char, char, char, char, char>(i[0], i[1], i[2], i[3], i[4]),
                v => v[9]);

            for (int i = 0; i < initialState.Length; i++)
                state[i + (padding/2)] = initialState[i];

            for (var g = 0; g < numOfGererations; g++)
            {
                for (int x = 2; x < state.Length - 2; x++)
                {
                    if (rules.TryGetValue((
                            state[x - 2],
                            state[x - 1],
                            state[x],
                            state[x + 1],
                            state[x + 2]),
                        out var s))
                        newState[x] = s;
                    else
                        newState[x] = '.';
                }

                newState.CopyTo(state, 0);
            }

            return state.Select((s, i) => s == '#' ? i - (padding/2): 0).Sum();

            char[] CreateArray(int length, char def)
            {
                var arr = new char[length];

                for (int i = 0; i < length; i++)
                    arr[i] = def;

                return arr;
            }
        }
    }
}