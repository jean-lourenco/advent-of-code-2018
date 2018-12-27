using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day_20
{
    public class Program
    {
        public static Dictionary<char, (int X, int Y)> Coord = new Dictionary<char, (int X, int Y)>
        {
            { 'W', (-1, 0) }, { 'E', (1, 0) }, { 'N', (0, -1) }, { 'S', (0, 1) }
        };
        public static Dictionary<(int X, int Y), int> Map = new Dictionary<(int X, int Y), int>();

        public static void Main(string[] args)
        {
            var part1 = Part1();
            System.Diagnostics.Debug.Assert(4184 == part1);
            Console.WriteLine($"Part1: {part1}");
           
            var part2 = Map.Where(x => x.Value >= 1000).Count();
            System.Diagnostics.Debug.Assert(8596 == part2);
            Console.WriteLine($"Part2: {part2}");
        }

        public static int Part1()
        {
            var input = File.ReadAllText("input.txt");
            input = input.Substring(1, input.Length - 2);
            
            var map = GetMap(null, input, (0, 0), 0);

            return MostSteps(map, 0);
        }

        public static int MostSteps(Room room, int initial)
        {
            initial++;
            var most = initial;

            foreach (var path in room.Paths)
            {
                var steps = MostSteps(path, initial);

                if (steps > most)
                    most = steps;
            }

            return most;
        }

        public static Room GetMap(Room initialRoom, string input, (int X, int Y) initial, int depth)
        {
            Room first = null;
            var curr = initialRoom;
            var currCood = initial;

            for (var i = 0; i < input.Length; i++)
            {
                var word = input[i];

                if (word == '(')
                {
                    var openParen = 1;
                    var lastPipe = i;
                    var lastIndex = i + 1;

                    for (; lastIndex < input.Length; lastIndex++)
                    {
                        var newWord = input[lastIndex];

                        if (newWord == '(')
                            openParen++;
                        else if (newWord == ')')
                            openParen--;
                        else if (newWord == '|' && openParen == 1)
                        {
                            GetMap(curr, input.Substring(lastPipe + 1, lastIndex - lastPipe - 1), currCood, depth);
                            lastPipe = lastIndex;
                        }

                        if (openParen == 0)
                        {
                            if (input[lastIndex - 1] != '|')
                                GetMap(curr, input.Substring(lastPipe + 1, lastIndex - lastPipe - 1), currCood, depth);
                            else
                                curr.Paths.Remove(curr.Paths.Last());

                            break;
                        }
                    }

                    i = lastIndex;
                    continue;
                }

                depth++;

                var c = Coord[word];
                currCood = (c.X + currCood.X, c.Y + currCood.Y);
                if (!Map.ContainsKey(currCood))
                    Map.Add(currCood, depth);

                var newRoom = new Room();

                if (curr == null)
                {
                    first = newRoom;
                    curr = newRoom;
                    continue;
                }

                curr.Paths.Add(newRoom);
                curr = newRoom;
            }

            return first;
        }

        public class Room
        {
            public IList<Room> Paths { get; }

            public Room()
            {
                Paths = new List<Room>();
            }
        }
    }
}
