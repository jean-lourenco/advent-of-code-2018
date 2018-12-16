using System;
using System.Collections.Generic;
using System.Linq;

namespace day_14
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1();
            System.Diagnostics.Debug.Assert("3841138812" == part1);
            Console.WriteLine($"Part1 - 10 scores after the puzzle input: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert(20200561 == part2);
            Console.WriteLine($"Part2 - How many recipes to the left: {part2}");
        }

        public static string Part1()
        {
            var input = 990941;
            var recipes = new LinkedList<byte>();
            var elf1 = recipes.AddFirst(3);
            var elf2 = recipes.AddLast(7);

            while (recipes.Count < input + 10)
            {
                var (recipeA, recipeB) = GetDigits(elf1.Value + elf2.Value);
                recipes.AddLast(recipeA);

                if (recipeB != null)
                    recipes.AddLast(recipeB.Value);

                elf1 = GetNode(elf1, recipes.First);
                elf2 = GetNode(elf2, recipes.First);
            }

            var score = "";
            var current = recipes.Count == input + 10 ? recipes.Last : recipes.Last.Previous;
            for (var i = 0; i < 10; i ++)
            {
                score += current.Value.ToString();
                current = current.Previous;
            }

            return string.Join("", score.Reverse());
        }

        public static int Part2()
        {
            var input = new byte[] {9,9,0,9,4,1};
            var recipes = new LinkedList<byte>();
            var elf1 = recipes.AddFirst(3);
            var elf2 = recipes.AddLast(7);

            while (true)
            {
                var (recipeA, recipeB) = GetDigits(elf1.Value + elf2.Value);
                recipes.AddLast(recipeA);

                if (recipes.Count > 6 && IsTheSequence(recipes.Last))
                    return recipes.Count - input.Length;

                if (recipeB != null)
                {
                    recipes.AddLast(recipeB.Value);

                    if (recipes.Count > 6 && IsTheSequence(recipes.Last))
                        return recipes.Count - input.Length;
                }

                elf1 = GetNode(elf1, recipes.First);
                elf2 = GetNode(elf2, recipes.First);
            }

            bool IsTheSequence(LinkedListNode<byte> current) =>
                current.Value == input[5]
                && current.Previous.Value == input[4]
                && current.Previous.Previous.Value == input[3]
                && current.Previous.Previous.Previous.Value == input[2]
                && current.Previous.Previous.Previous.Previous.Value == input[1]
                && current.Previous.Previous.Previous.Previous.Previous.Value == input[0];
        }

        public static (byte, byte?) GetDigits(int number)
        {
            if (number < 10)
                return ((byte)number, null);

            var b = (byte)(number % 10);
            var a = (byte)((number / 10) % 10);
            return (a, b);
        }

        public static LinkedListNode<byte> GetNode(
            LinkedListNode<byte> curr,
            LinkedListNode<byte> first)
        {
            var iterations = curr.Value + 1;
            for (byte i = 0; i < iterations; i++)
                curr = curr.Next ?? first;

            return curr;
        }
    }
}