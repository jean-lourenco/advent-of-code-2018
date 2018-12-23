using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day_19
{
    public class Program
    {
        public static int[] R = new int[]
        {
            0, 0, 0, 0, 0, 0
        };

        public static void Main(string[] args)
        {
            var part1 = Part1And2(0);
            System.Diagnostics.Debug.Assert(2072 == part1);
            Console.WriteLine($"Part1 - Value on register 0: {part1}");

            var part2 = Part1And2(1);
            System.Diagnostics.Debug.Assert(27578880 == part2);
            Console.WriteLine($"Part2 - Value on register 0 with initial value of 1: {part2}");
        }

        public static int Part1And2(int initialRegister0Value)
        {
            var input = File.ReadAllLines("input.txt");
            var ipRegister = int.Parse(input[0][4].ToString());
            var instructions = input
                .Skip(1)
                .Select(x =>
                {
                    var values = x.Split(" ");
                    return new {
                        Cmd = values[0], 
                        A = int.Parse(values[1]), 
                        B = int.Parse(values[2]), 
                        C = int.Parse(values[3])
                    };
                })
                .ToList();

            ClearRegisters();
            R[0] = initialRegister0Value;
            var ipValue = R[ipRegister];
            var iteration = 0;
            while (ipValue < instructions.Count())
            {
                R[ipRegister] = ipValue;

                var a0 = R[0];
                var a1 = R[1];
                var a2 = R[2];
                var a3 = R[3];
                var a4 = R[4];
                var a5 = R[5];
                var ipBefore = ipValue;

                var inst = instructions[ipValue];
                OpCodeImpl[inst.Cmd].Invoke(inst.A, inst.B, inst.C);
                ipValue = R[ipRegister];
                ipValue += 1;
                iteration++;
                
                // The program sums all factors of the number in register 5
                // The number in register 5 is allocated before iteration 30
                if (initialRegister0Value == 1 && iteration >= 30)
                    return SumOfFactors(R[5]);
            }

            return R[0];
        }

        public static void ClearRegisters()
        {
            for (var i = 0; i < R.Length; i++)
                R[i] = 0;
        }

        public static IDictionary<string, Action<int, int, int>> OpCodeImpl = 
            new Dictionary<string, Action<int, int, int>>
        {
            { "addr", (a, b, c) => { R[c] = R[a] + R[b]; } },
            { "addi", (a, b, c) => { R[c] = R[a] + b ;} },
            { "mulr", (a, b, c) => { R[c] = R[a] * R[b];} },
            { "muli", (a, b, c) => { R[c] = R[a] * b ;} },
            { "banr", (a, b, c) => { R[c] = R[a] & R[b];} },
            { "bani", (a, b, c) => { R[c] = R[a] & b;} },
            { "borr", (a, b, c) => { R[c] = R[a] | R[b];} },
            { "bori", (a, b, c) => { R[c] = R[a] | b;} },
            { "setr", (a, b, c) => { R[c] = R[a]; } },
            { "seti", (a, b, c) => { R[c] = a; } },
            { "gtir", (a, b, c) => { R[c] = a > R[b] ? 1 : 0;} },
            { "gtri", (a, b, c) => { R[c] = R[a] > b ? 1 : 0;} },
            { "gtrr", (a, b, c) => { R[c] = R[a] > R[b] ? 1 : 0;} },
            { "eqir", (a, b, c) => { R[c] = a == R[b] ? 1 : 0;} },
            { "eqri", (a, b, c) => { R[c] = R[a] == b ? 1 : 0;} },
            { "eqrr", (a, b, c) => { R[c] = R[a] == R[b] ? 1 : 0;} },
        };

        public static int SumOfFactors(int number)
        {
            var sum = 0;
            for (var i = 1; i <= number; i++)
                sum += number % i == 0 ? i : 0;
                
            return sum;
        }
    }   
}