using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace day_17
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var (part1, part2) = Part1();
            System.Diagnostics.Debug.Assert(39649 == part1);
            System.Diagnostics.Debug.Assert(28872 == part2);
            
            Console.WriteLine($"Part1 - Number of water tiles covered: {part1}");
            Console.WriteLine($"Part2 - Number of water tiles retained: {part2}");
        }

        public static int Iterations = 0;

        public static (int part1, int part2) Part1()
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

            var minY = clayCoord
                .Select(c => c.Coord1 == 'y' ? c.Coord1Value : c.Coord2From)
                .Min();

            var matrix = new char[maxY, maxX];
            matrix[0, 500] = '+';

            foreach (var clay in clayCoord)
            {
                var rangeIsInX = clay.Coord1 == 'y';
                for (var c = clay.Coord2From; c <= clay.Coord2To; c++)
                {
                    if (rangeIsInX)
                        matrix[clay.Coord1Value, c] = '#';
                    else
                        matrix[c, clay.Coord1Value] = '#';
                }
            }

            for (var i = 0; i < 3; i++)
                TraceDown(matrix, (500, 1));

            var water = 0;
            var sand = 0;
            for (int y = minY; y < matrix.GetUpperBound(0) + 1; y++)
            {
                for (int x = 0; x < matrix.GetUpperBound(1) + 1; x++)
                {
                    var tile = matrix[y, x];

                    if (tile == '|')
                        sand++;

                    if (tile == '~')
                        water++;
                }
            }

            return (sand + water, water);
        }

        public static bool TraceDown(char[,] matrix, (int x, int y) initial)
        {
            var (x, y) = initial;
            int leftX, rightX = 0;
            var newY = y;

            while (true)
            {
                if (isEnclosed())
                    break;

                var next = matrix[newY + 1, x];

                if (next == '~' || next == '#')
                    break;

                matrix[newY++, x] = '|';

                if (newY + 1 > matrix.GetUpperBound(0))
                {
                    matrix[newY, x] = '|';
                    return true;
                }

                next = matrix[newY + 1, x];

                if (next == '~' || next == '#')
                    break;
            }

            char lastChar = default;
            while (newY >= y || isEnclosed())
            {
                lastChar = isEnclosed() ? '~' : '|';

                for (var newX = leftX; newX <= rightX; newX++)
                    matrix[newY, newX] = lastChar;

                newY--;

                if (lastChar == '|')
                    break;
            }
            newY++;

            var done = true;

            if (lastChar == '|')
            {
                var leftBeneath = matrix[newY + 1, leftX];
                if (leftBeneath != '#' && leftBeneath != '~')
                {
                    var leftDone = TraceDown(matrix, (leftX, newY + 1));
                    done = done && leftDone;
                }

                var rightBeneath = matrix[newY + 1, rightX];
                if (rightBeneath != '#' && rightBeneath != '~')
                {
                    var rightDone = TraceDown(matrix, (rightX, newY + 1));
                    done = done && rightDone;
                }
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
    }
}