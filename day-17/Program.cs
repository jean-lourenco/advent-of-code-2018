using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace day_17
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1();
            //System.Diagnostics.Debug.Assert(0 == part1);
            Console.WriteLine($"Part1 - Number of water tiles covered: {part1}");
        }

        public static int Part1()
        {
            var regex = new Regex(@"([a-zA-Z])=([0-9]+), ([a-zA-Z])=([0-9]+)..([0-9]+)");
            var clayCoord = File
                .ReadAllLines("input.txt")
                .Select(i => regex.Match(i))
                .Select(r => new {
                    Coord1 = r.Groups[1].Value[0],
                    Coord1Value = int.Parse(r.Groups[2].Value),
                    Coord2 = r.Groups[3].Value[0],
                    Coord2From = int.Parse(r.Groups[4].Value),
                    Coord2To = int.Parse(r.Groups[5].Value),
                });

            var maxX = clayCoord
                .Select(c => c.Coord1 == 'x' ? c.Coord1Value : c.Coord2To)
                .Max() + 2;

            var maxY = clayCoord
                .Select(c => c.Coord1 == 'y' ? c.Coord1Value : c.Coord2To)
                .Max() + 1;

            var underground = new char[maxY, maxX];
            underground[0, 500] = '+';

            foreach (var clay in clayCoord)
            {
                var rangeIsInX = clay.Coord1 == 'y';
                for (var c = clay.Coord2From; c <= clay.Coord2To; c++)
                {
                    if (rangeIsInX)
                        underground[clay.Coord1Value, c] = '#';
                    else
                        underground[c, clay.Coord1Value] = '#';
                }
            }

            while (!RayTraceDown(underground, (500, 1)))
            {
            }

            Write(underground);
            var sum = 0;
            for (int y = 0; y < underground.GetUpperBound(0) + 1; y++)
            {
                for (int x = 0; x < underground.GetUpperBound(1) + 1; x++)
                {
                    var tile = underground[y, x];
                    if (tile == '|' || tile == '~')
                        sum++;
                }
            }

            return sum;
        }

        public static bool RayTraceDown(char[,] matrix, (int x, int y) initial)
        {
            var (x, y) = initial;
            int leftX, rightX = 0;
            var newY = y;

            while (true)
            {
                if (isEnclosed())
                    break;

                matrix[newY++, x] = '|';

                if (newY + 1 > matrix.GetUpperBound(0))
                {
                    matrix[newY, x] = '|';
                    return true;
                }
                //Console.WriteLine($"{x},{newY}");
                var next = matrix[newY + 1, x];

                if (next == '~' || next == '#')
                    break;
            }

            var fillChar = isEnclosed() ? '~' : '|';

            for (var newX = leftX; newX <= rightX; newX++)
            {
                //Console.WriteLine($"{x},{newY}");
                matrix[newY, newX] = fillChar;
            }

            var done = true;
            if (fillChar == '|')
            {
                var leftBeneath = matrix[newY + 1, leftX];
                if (leftBeneath != '#' && leftBeneath != '~')
                    done = done && RayTraceDown(matrix, (leftX, newY + 1));

                var rightBeneath = matrix[newY + 1, rightX];
                if (rightBeneath != '#' && rightBeneath != '~')
                    done = done && RayTraceDown(matrix, (rightX, newY + 1));
            }
            else
                return false;

            return done;

            bool isEnclosed()
            {
                var enclosed = true;

                // Left
                for (leftX = x; leftX >= 0; leftX--)
                {
                    var beneath = matrix[newY + 1, leftX];

                    if (!(beneath == '~') && !(beneath == '#'))
                    {
                        enclosed = false;
                        break;
                    }

                    if (matrix[newY, leftX - 1] == '#')
                        break;
                }

                // Right
                for (rightX = x; rightX < matrix.GetUpperBound(1) + 1; rightX++)
                {
                    var beneath = matrix[newY + 1, rightX];

                    if (!(beneath == '~') && !(beneath == '#'))
                    {
                        enclosed = false;
                        break;
                    }

                    if (matrix[newY, rightX + 1] == '#')
                        break;
                }

                return enclosed;
            }
        }

        public static void Write(char[,] underground)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int y = 0; y < underground.GetUpperBound(0) + 1; y++)
            {
                sb.Append(y.ToString().PadLeft(2, '0'));
                //for (int x = 494; x < underground.GetUpperBound(1) + 1; x++)
                for (int x = 425; x < underground.GetUpperBound(1) + 1; x++)
                {
                    sb.Append(underground[y, x] == default(char) ? '.' : underground[y, x]);
                }
                sb.AppendLine("");
            }

            //System.Console.WriteLine(sb.ToString());
            File.WriteAllText("output.txt", sb.ToString());
        }
    }
}

/***
 * 
 * 
 *   444444444444455555555555555555
  888999999999900000000001111111
  789012345678901234567890123456
00.............+................
01.............|................
02...........||||||.............
03...........|####|.............
04||||||||||||||||||||||||||||..
05|############||############|..
06|............||............|..
07|.#..........||............|..
08|.#..........||.........#..|..
09|.#..........||.........#..|..
10|.#..........||###......#..|..
11|.#..........||.........#..|..
12|.#..........||.........#..|..
13|.#..........||.........#..|..
14|.#..........||.........#..|..
15|.#..........||###......#..|..
16|.#..........||.........#..|..
17|.#..........||.........#..|..
18|.#######################..|..
19|..........................|..
  444444444444455555555555555555
  888999999999900000000001111111
  789012345678901234567890123456
  
  
x=636, y=158..169
y=445, x=630..650


y=3, x=499..502
y=5, x=488..499
y=5, x=502..513
x=489, y=7..18
x=511, y=8..18
y=18, x=489..511
y=10, x=502..504
y=15, x=502..504
*/