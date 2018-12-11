using System;
using System.Collections.Generic;
using System.Linq;

namespace day_9
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //459 players; last marble is worth 71320 points
            var part1 = Part1And2(459, 71320);
            System.Diagnostics.Debug.Assert(375414 == part1);
            Console.WriteLine($"Part1 - Highest score: {part1}");

            var part2 = Part1And2(459, 71320 * 100);
            System.Diagnostics.Debug.Assert(3168033673 == part2);
            Console.WriteLine($"Part2 - Highest score with x100: {part2}");   
        }
        
        public static long Part1And2(int numOfPlayers, long lastMarble)
        {
            var player = 0;
            var currentMarble = 0;
            var scores = new Dictionary<int, long>();
            var board = new LinkedList<int>();
            board.AddFirst(0);
            var curr = board.First;

            while (true)
            {
                if (++currentMarble % 23 == 0)
                {
                    if (!scores.ContainsKey(player))
                        scores.Add(player, 0);

                    var (nowCurr, score) = Remove7MarblesBehind();
                    curr = nowCurr;
                    scores[player] += currentMarble + score;
                }
                else
                    curr = InsertInNextPosition();

                if (++player >= numOfPlayers)
                    player = 0;

                if (currentMarble >= lastMarble)
                    break;
            }

            return scores.Max(kv => kv.Value);

            LinkedListNode<int> InsertInNextPosition() =>
                board.AddAfter(curr.Next ?? board.First, currentMarble);

            (LinkedListNode<int>, int) Remove7MarblesBehind()
            {
                var toRemove = curr;

                for (int i = 0; i < 7; i++)
                    toRemove = toRemove.Previous ?? board.Last;

                var nowCurr = toRemove.Next ?? board.First;
                var score = toRemove.Value;
                board.Remove(toRemove);

                return (nowCurr, score);
            }
        }
    }
}
