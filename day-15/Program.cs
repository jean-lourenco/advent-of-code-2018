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
            var part1 = Part1();
            //Sytem.Diagnostics.Debug.Assert(0 == part1);
            Console.WriteLine($"Part1: {part1}");
        }

        public static int Part1()
        {
            var round = 0;
            var units = new List<Unit>();
            var map = File
                .ReadAllLines("input.txt")
                .Select((line, y) =>
                    line.Select((word, x) =>
                    {
                        if (word == 'G' || word == 'E')
                            units.Add(new Unit((x, y), word == 'G' ? Type.G : Type.E));
                        
                        return word == '#' ? 1 : 0;
                    }).ToArray())
                .ToArray();

            while (true)
            {
                foreach (var unit in units.OrderBy(u => u.Location.Y).ThenBy(u => u.Location.X))
                {
                    if (!unit.IsAlive)
                        continue;

                    if (TryGetEnemyToAttack(unit, out var enemy))
                    {
                        enemy.Health -= unit.Attack;
                        continue;
                    }

                    // SEARCH NEAREST REACHABLE ENEMY
                    var (found, newLocation) = FindNearestEnemy(unit);

                    // MOVE (IF POSSIBLE)
                    if (found)
                        unit.Location = newLocation;

                    // ATTACK (IF POSSIBLE)
                    if (TryGetEnemyToAttack(unit, out enemy))
                        enemy.Health -= unit.Attack;

                    if (IsThereAnyEnemyAlive(unit.Type))
                        return round * units.Where(u => u.IsAlive).Sum(u => u.Health);
                }

                round++; 
            }

            bool IsThereAnyEnemyAlive(Type unit) =>
                units.All(u => GetEnemyType(unit) == u.Type && !u.IsAlive);
            
            Type GetEnemyType(Type unit) => unit == Type.G ? Type.E : Type.G;

            bool TryGetEnemyToAttack(Unit unit, out Unit enemy)
            {
                var enemies = units.Where(u => 
                    u.IsAlive
                    && ((u.Location.X + 1 == unit.Location.X && u.Location.Y == unit.Location.Y)
                    || (u.Location.X - 1 == unit.Location.X && u.Location.Y == unit.Location.Y)
                    || (u.Location.X == unit.Location.X && u.Location.Y + 1 == unit.Location.Y)
                    || (u.Location.X == unit.Location.X && u.Location.Y - 1 == unit.Location.Y)));

                enemy = enemies
                    .OrderBy(u => u.Health)
                    .ThenBy(u => u.Location.X)
                    .ThenBy(u => u.Location.Y)
                    .FirstOrDefault();

                return enemy != null;
            }

            (bool found, (int x, int y) loc) FindNearestEnemy(Unit unit)
            {
                var queue = new Queue<ImmutableList<(int x, int y)>>();
                var visited = new HashSet<(int x, int y)>();

                queue.Enqueue(ImmutableList.Create(unit.Location));

                while (queue.Count > 0)
                {
                    var path = queue.Dequeue();
                    var curr = path.Last();

                    if (visited.Contains(curr))
                        continue;

                    visited.Add(curr);

                    if (map[curr.y][curr.x] == 1 || units.Any(u => u.IsAlive && u.Location == curr))
                        continue;

                    if (LocationIsAdjacentToEnemy(curr))
                        return (true, path.Skip(1).First());

                    foreach (var adjancent in GetAdjancentPositions(curr).OrderBy(c => c.Y).ThenBy(c => c.X))
                        queue.Enqueue(path.Add(adjancent));
                }

                return (false, default);
            }

            bool LocationIsAdjacentToEnemy((int X, int Y) location) =>
                units.Any(u =>
                    u.IsAlive
                    && ((u.Location.X + 1 == location.X && u.Location.Y == location.Y)
                    || (u.Location.X - 1 == location.X && u.Location.Y == location.Y)
                    || (u.Location.X == location.X && u.Location.Y + 1 == location.Y)
                    || (u.Location.X == location.X && u.Location.Y - 1 == location.Y)));

            IEnumerable<(int X, int Y)> GetAdjancentPositions((int X, int Y) curr)
            {
                if (curr.Y > 0)
                    yield return (curr.X, curr.Y - 1);

                if (curr.X < map[0].Length - 1)
                    yield return (curr.X + 1, curr.Y);

                if (curr.X > 0)
                    yield return (curr.X - 1, curr.Y);

                if (curr.Y < map.Length - 1)
                    yield return (curr.X, curr.Y + 1);
            }
        }
    }

    public class Unit
    {
        public int Attack { get; } = 3;
        public int Health { get; set; } = 300;
        public Type Type { get; set; }
        public (int X, int Y) Location { get; set; }
        public bool IsAlive => Health > 0;

        public Unit((int x, int y) currentLocation, Type type)
        {
            Location = currentLocation;
            Type = type;
        }
    }

    public enum Type
    {
        G, E
    }
}