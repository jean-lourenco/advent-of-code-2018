using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace day_16
{
    public class Program
    {
        public static int[] R = new int[] 
        {
            0, 0, 0, 0
        };

        public static void Main(string[] args)
        {
            var part1 = Part1(out var opTest);
            System.Diagnostics.Debug.Assert(640 == part1);
            Console.WriteLine($"Part1: OpCodes that behave like 3 or more: {part1}");

            var part2 = Part2(opTest);
            System.Diagnostics.Debug.Assert(472 == part2);
            Console.WriteLine($"Part2: Output of the program: {part2}");
        }

        public static int Part1(out IDictionary<TestInstructions, IList<string>> opTest)
        {
            var input = File.ReadAllLines("input1.txt");
            var inst = new List<TestInstructions>(input.Length / 4);

            for (int i = 0; i < input.Length; i += 4)
                inst.Add(new TestInstructions(input[i], input[i + 1], input[i + 2]));

            opTest = new Dictionary<TestInstructions, IList<string>>();

            foreach (var i in inst)
            {
                opTest.Add(i, new List<string>());
                foreach (var op in OpCodeImpl)
                {
                    R[0] = i.InitialState.A;
                    R[1] = i.InitialState.B;
                    R[2] = i.InitialState.C;
                    R[3] = i.InitialState.D;

                    op.Value.Invoke(
                        i.Instruction.A, 
                        i.Instruction.B, 
                        i.Instruction.C, 
                        i.Instruction.D);

                    if (R[0] == i.PosState.A 
                        && R[1] == i.PosState.B 
                        && R[2] == i.PosState.C 
                        && R[3] == i.PosState.D)
                        opTest[i].Add(op.Key);
                }
            }

            return opTest.Count(x => x.Value.Count() >= 3);
        }

        public static int Part2(IDictionary<TestInstructions, IList<string>> opTest)
        {
            var used = new HashSet<string>();
            var opCodes = new Dictionary<int, Action<int, int, int, int>>();

            var grp = opTest
                .GroupBy(x => x.Key.Instruction.A)
                .Select(x => new { x.Key, Possibilities = x.SelectMany(d => d.Value).Distinct().ToList() })
                .Select(x => new { x.Key, x.Possibilities, Count = x.Possibilities.Count()})
                .OrderBy(x => x.Possibilities.Count())
                .ToList();

            while (opCodes.Count < 16)
            {
                foreach (var op in grp.OrderBy(x => x.Count))
                {
                    var ops = op.Possibilities.Where(x => !used.Contains(x));
                    if (ops.Count() != 1)
                        continue;

                    var opString = ops.Single();

                    used.Add(ops.Single());

                    opCodes.Add(op.Key, OpCodeImpl[opString]);
                }
            }

            var inst = File.ReadAllLines("input2.txt").Select(x =>
            {
                var i = x.Split(" ");
                return (int.Parse(i[0]), int.Parse(i[1]), int.Parse(i[2]), int.Parse(i[3]));
            });

            R[0] = R[1] = R[2] = R[3] = 0;

            foreach (var i in inst)
                opCodes[i.Item1].Invoke(i.Item1, i.Item2, i.Item3, i.Item4);

            return R[0];
        }

        public static IDictionary<string, Action<int, int, int, int>> OpCodeImpl = 
            new Dictionary<string, Action<int, int, int, int>>
        {
            { "addr", (op, a, b, c) => { R[c] = R[a] + R[b]; } },
            { "addi", (op, a, b, c) => { R[c] = R[a] + b ;} },
            { "mulr", (op, a, b, c) => { R[c] = R[a] * R[b];} },
            { "muli", (op, a, b, c) => { R[c] = R[a] * b ;} },
            { "banr", (op, a, b, c) => { R[c] = R[a] & R[b];} },
            { "bani", (op, a, b, c) => { R[c] = R[a] & b;} },
            { "borr", (op, a, b, c) => { R[c] = R[a] | R[b];} },
            { "bori", (op, a, b, c) => { R[c] = R[a] | b;} },
            { "setr", (op, a, b, c) => { R[c] = R[a]; } },
            { "seti", (op, a, b, c) => { R[c] = a; } },
            { "gtir", (op, a, b, c) => { R[c] = a > R[b] ? 1 : 0;} },
            { "gtri", (op, a, b, c) => { R[c] = R[a] > b ? 1 : 0;} },
            { "gtrr", (op, a, b, c) => { R[c] = R[a] > R[b] ? 1 : 0;} },
            { "eqir", (op, a, b, c) => { R[c] = a == R[b] ? 1 : 0;} },
            { "eqri", (op, a, b, c) => { R[c] = R[a] == b ? 1 : 0;} },
            { "eqrr", (op, a, b, c) => { R[c] = R[a] == R[b] ? 1 : 0;} },
        };
    }

    public class TestInstructions
    {
        public (int A, int B, int C, int D) InitialState { get; }
        public (int A, int B, int C, int D) Instruction { get; set; }
        public (int A, int B, int C, int D) PosState { get; }

        public TestInstructions(string line1, string line2, string line3)
        {
            InitialState = (
                int.Parse(line1.ElementAt(9).ToString()), 
                int.Parse(line1.ElementAt(12).ToString()),
                int.Parse(line1.ElementAt(15).ToString()), 
                int.Parse(line1.ElementAt(18).ToString()));

            var codes = line2.Split(" ");
            Instruction = (
                int.Parse(codes[0]),
                int.Parse(codes[1]),
                int.Parse(codes[2]),
                int.Parse(codes[3]));

            PosState = (
                int.Parse(line3.ElementAt(9).ToString()), 
                int.Parse(line3.ElementAt(12).ToString()),
                int.Parse(line3.ElementAt(15).ToString()), 
                int.Parse(line3.ElementAt(18).ToString()));
        }
    }
}
