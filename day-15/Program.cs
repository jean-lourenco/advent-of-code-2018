using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace day_15
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1(3, out _);
            System.Diagnostics.Debug.Assert(198531 == part1);
            Console.WriteLine($"Part1: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert(90420 == part2);
            Console.WriteLine($"Part2: {part2}");
        }

        public static int Part1(int elvenAttackPower, out int elvenLosses)
        {
            var units = new List<Unit>();
            var map = File
                .ReadAllLines("input.txt")
                .Select((line, y) =>
                    line.Select((word, x) =>
                    {
                        if (word == 'G' || word == 'E')
                            units.Add(new Unit((x, y), word == 'G' ? Type.G : Type.E, elvenAttackPower));
                        
                        return word == '#' ? 1 : 0;
                    }).ToArray())
                .ToArray();

            for (var round = 0; ;round++)
            {
                foreach (var unit in units.OrderBy(u => u.Location.Y).ThenBy(u => u.Location.X))
                {
                    if (!unit.IsAlive)
                        continue;

                    // attack (if possible) and end turn
                    if (TryGetEnemyToAttack(unit, out var enemy))
                    {
                        enemy.Health -= unit.Attack;
                        continue;
                    }

                    // search nearest reachable enemy
                    var (found, newLocation) = FindNearestEnemy(unit);

                    // move (if possible)
                    if (found)
                        unit.Location = newLocation;

                    // attack (if possible)
                    if (TryGetEnemyToAttack(unit, out enemy))
                        enemy.Health -= unit.Attack;

                    // check endgame condition
                    if (!IsThereAnyEnemyAlive(unit.Type))
                    {
                        elvenLosses = units.Count(u => !u.IsAlive && u.Type == Type.E);
                        return round * units.Where(u => u.IsAlive).Sum(u => u.Health);
                    }
                }
            }

            bool IsThereAnyEnemyAlive(Type unit) =>
                units.Any(u => GetEnemyType(unit) == u.Type && u.IsAlive);
            
            Type GetEnemyType(Type unit) => unit == Type.G ? Type.E : Type.G;

            bool TryGetEnemyToAttack(Unit unit, out Unit enemy)
            {
                var enemies = units.Where(u => 
                    u.IsAlive
                    && u.Type == GetEnemyType(unit.Type)
                    && ((u.Location.X + 1 == unit.Location.X && u.Location.Y == unit.Location.Y)
                    || (u.Location.X - 1 == unit.Location.X && u.Location.Y == unit.Location.Y)
                    || (u.Location.X == unit.Location.X && u.Location.Y + 1 == unit.Location.Y)
                    || (u.Location.X == unit.Location.X && u.Location.Y - 1 == unit.Location.Y)));

                enemy = enemies
                    .OrderBy(u => u.Health)
                    .ThenBy(u => u.Location.Y)
                    .ThenBy(u => u.Location.X)
                    .FirstOrDefault();

                return enemy != null;
            }

            (bool found, (int x, int y) loc) FindNearestEnemy(Unit unit)
            {
                var queue = new Queue<ImmutableList<(int x, int y)>>();
                var visited = new HashSet<(int x, int y)>();
                var foundList = new List<ImmutableList<(int x, int y)>>();
                queue.Enqueue(ImmutableList.Create(unit.Location));

                while (queue.Count > 0)
                {
                    var path = queue.Dequeue();
                    var curr = path.Last();

                    if (visited.Contains(curr))
                        continue;

                    visited.Add(curr);

                    if (curr != unit.Location 
                        && (map[curr.y][curr.x] == 1 
                        || units.Any(u => u.IsAlive && u.Location == curr)))
                        continue;

                    if (LocationIsAdjacentToEnemy(curr, unit))
                        foundList.Add(path);

                    if (!foundList.Any())
                    {
                        foreach (var adjancent in GetAdjancentPositions(curr).OrderBy(c => c.Y).ThenBy(c => c.X))
                            queue.Enqueue(path.Add(adjancent));
                    }

                }

                if (foundList.Any())
                {
                    return (true, foundList
                        .OrderBy(x => x.Count())
                        .ThenBy(x => x.Last().y)
                        .ThenBy(x => x.Last().x)
                        .First()
                        .Skip(1)
                        .First());
                }

                return (false, default);
            }

            bool LocationIsAdjacentToEnemy((int X, int Y) location, Unit curr) =>
                units.Any(u =>
                    u.IsAlive
                    && u.Type == GetEnemyType(curr.Type)
                    && ((u.Location.X + 1 == location.X && u.Location.Y == location.Y)
                    || (u.Location.X - 1 == location.X && u.Location.Y == location.Y)
                    || (u.Location.X == location.X && u.Location.Y + 1 == location.Y)
                    || (u.Location.X == location.X && u.Location.Y - 1 == location.Y)));

            IEnumerable<(int X, int Y)> GetAdjancentPositions((int X, int Y) curr)
            {
                if (curr.Y > 0)
                    yield return (curr.X, curr.Y - 1);

                if (curr.X < map[0].Length)
                    yield return (curr.X + 1, curr.Y);

                if (curr.X > 0)
                    yield return (curr.X - 1, curr.Y);

                if (curr.Y < map.Length)
                    yield return (curr.X, curr.Y + 1);
            }
        }

        public static int Part2()
        {
            for (var attack = 4; ;attack++)
            {
                var result = Part1(attack, out var elvenLosses);

                if (elvenLosses == 0)
                    return result;
            }
        }
    }

    public class Unit
    {
        public int Attack { get; }
        public int Health { get; set; } = 200;
        public Type Type { get; set; }
        public (int X, int Y) Location { get; set; }
        public bool IsAlive => Health > 0;

        public Unit((int x, int y) currentLocation, Type type, int elvenAttackPower)
        {
            Location = currentLocation;
            Type = type;
            Attack = type == Type.E ? elvenAttackPower : 3;
        }
    }

    public enum Type
    {
        G, E
    }
}