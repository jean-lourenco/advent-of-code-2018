using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace day_7
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Refactor with Topological Sort
            var part1 = Part1();
            System.Diagnostics.Debug.Assert("GJKLDFNPTMQXIYHUVREOZSAWCB" == part1);
            Console.WriteLine($"Part1 - Order of the instructions: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert(967 == part2);
            Console.WriteLine($"Part2 - Number of Seconds: {part2}");
        }

        public static string Part1()
        {
            var instructions = File
                .ReadAllLines("input.txt")
                .Select(Instruction.Parse)
                .OrderBy(i => i.Before);

            var visited = new HashSet<char>();
            var steps = Enumerable.Range('A', 26).Select(c => (char)c);
            var sb = new StringBuilder();

            while (true)
            {
                var next = steps.FirstOrDefault(s => 
                    !visited.Contains(s)
                    && !instructions.Any(i => i.After == s && !visited.Contains(i.Before)));

                if (next == default(char))
                    return sb.ToString();

                sb.Append(next);
                visited.Add(next);
            }
        }

        public static int Part2()
        {
            var instructions = File
                .ReadAllLines("input.txt")
                .Select(Instruction.Parse)
                .OrderBy(i => i.Before);

            var pool = new WorkerPool();
            var visited = new HashSet<char>();
            var steps = Enumerable.Range('A', 26).Select(c => (char)c).ToList();

            while (true)
            {
                var allNext = steps
                    .Where(s => 
                        !instructions.Any(i => i.After == s && !visited.Contains(i.Before)))
                    .ToList();

                var valid = allNext.Any();
                
                if (!valid && !pool.AreWorking)
                    return pool.Timer;

               if (pool.IsWorkerAvailble() && valid) 
               {
                   foreach (var next in allNext)
                   {
                        pool.SetTask(() => 
                            visited.Add(next), 
                            pool.Timer + GetEffort(next));

                        steps.Remove(next);
                   }
               }

                pool.AddTimer();
            }

            int GetEffort(char letter) => letter - 'A' + 61;
        }
    }

    public class WorkerPool
    {
        private IList<TempTask> Tasks;
        public int Timer { get; private set; }
        public bool AreWorking => Tasks.Count() > 0;
        public bool IsWorkerAvailble() => Tasks.Count() < 5;
        public void SetTask(Action cb, int duration) =>
            Tasks.Add(new TempTask(cb, duration));

        public void AddTimer()
        {
            Timer++;

            var toRemove = Tasks
                .Where(t => t.Duration <= Timer)
                .Select(t => 
                {
                    t.Callback();
                    return t;
                });

            foreach (var remove in toRemove.ToList())
                Tasks.Remove(remove);
        }

        public WorkerPool()
        {
            Tasks = new List<TempTask>();
        }

        public class TempTask
        {
            public Action Callback { get; set; }
            public int Duration { get; set; }

            public TempTask(Action callback, int duration)
            {
                Callback = callback;
                Duration = duration;
            }
        }
    }

    public struct Instruction
    {
        public char Before { get; }
        public char After { get; }

        public Instruction(string input)
        {
            var span = input.AsSpan();
            Before = span.Slice(5, 1).ToString()[0];
            After = span.Slice(36, 1).ToString()[0];
        }

        public static Instruction Parse(string input) =>
            new Instruction(input);
    }
}