using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace day_8
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1();
            System.Diagnostics.Debug.Assert(44893 == part1);
            Console.WriteLine($"Part1 - Sum of all metadata: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert(27433 == part2);
            Console.WriteLine($"Part1 - Node value of Root: {part2}");
        }

        public static int Part1()
        {
            Span<int> input = File
                .ReadAllText("input.txt")
                .Split(" ")
                .Select(int.Parse)
                .ToArray();

            var root = GetTree(input, out _);

            return root.GetMetadaSumOfChildren() + root.Metadata.Sum();
        }

        public static int Part2()
        {
            Span<int> input = File
                .ReadAllText("input.txt")
                .Split(" ")
                .Select(int.Parse)
                .ToArray();

            var root = GetTree(input, out _);

            return root.GetNodeValue();
        }

        public static Node GetTree(Span<int> input, out int length)
        {
            var node = new Node()
            {
                NumberOfChildren = input[0],
                NumberOfMetadata = input[1],
            };

            length = 2;

            for (var i = 0; i < node.NumberOfChildren; i++)
            {
                node.Children.Add(GetTree(input.Slice(length), out var childLength));
                length += childLength;
            }

            node.Metadata = input
                .Slice(length, node.NumberOfMetadata)
                .ToArray();

            length += node.NumberOfMetadata;
            return node;
        }
    }

    public class Node
    {
        public int NumberOfMetadata { get; set; }
        public int NumberOfChildren { get; set; }
        public IList<Node> Children { get; set; }
        public IList<int> Metadata { get; set; }

        public Node()
        {
            Children = new List<Node>();
            Metadata = new List<int>();
        }

        public int GetMetadaSumOfChildren() => Children
            .Select(c => c.Metadata.Sum() + c.GetMetadaSumOfChildren())
            .Sum();

        public int GetNodeValue() 
        {
            if (NumberOfChildren == 0)
                return Metadata.Sum();

//tentativa 54267293 (too high)
            return Metadata.Select(m => 
            {
                if (m == 0)
                    return 0;

                var child = Children.ElementAtOrDefault(m - 1);

                if (child != null)
                    return child.GetNodeValue();
                else 
                    return 0;
            })
            .Sum();
        }
    }
}
