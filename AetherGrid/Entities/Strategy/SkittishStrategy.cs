using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
namespace We_have_doom_at_home.Entities.Strategy;

public class SkittishStrategy : IStrategy
{
    private readonly int MaxPlayerConsiderDistance = MaxConsideredSearchDistance;

    public void Behave(Map map, List<Player> players, Enemy enemy)
    {
        var nearbyPlayers = GetNearbyPlayers(map, enemy, players, MaxPlayerConsiderDistance);
        if (nearbyPlayers.Count == 0)
            return;

        var closestPlayer = FindClosestPlayer(map, enemy, nearbyPlayers);
        int currentDistance = map.GetShortestDistance(enemy.PosX, enemy.PosY, closestPlayer.PosX, closestPlayer.PosY, MaxPlayerConsiderDistance);

        var bestMove = FindBestMove(map, enemy, closestPlayer, currentDistance);

        MoveEnemyIfNeeded(enemy, bestMove, map);
    }

    private List<Player> GetNearbyPlayers(Map map, Enemy enemy, List<Player> players, int maxDistance)
    {
        return players
            .Where(p => map.GetShortestDistance(enemy.PosX, enemy.PosY, p.PosX, p.PosY, MaxPlayerConsiderDistance) <= maxDistance)
            .ToList();
    }

    private Player FindClosestPlayer(Map map, Enemy enemy, List<Player> players)
    {
        return players
            .OrderBy(p => map.GetShortestDistance(enemy.PosX, enemy.PosY, p.PosX, p.PosY, MaxPlayerConsiderDistance))
            .First();
    }

    private (int x, int y) FindBestMove(Map map, Enemy enemy, Player closestPlayer, int currentDistance)
    {
        var possibleMoves = new List<(int x, int y)>
        {
            (enemy.PosX, enemy.PosY),         // stay put
            (enemy.PosX, enemy.PosY - 1),     // up
            (enemy.PosX, enemy.PosY + 1),     // down
            (enemy.PosX - 1, enemy.PosY),     // left
            (enemy.PosX + 1, enemy.PosY),     // right
        };

        var walkableMoves = possibleMoves
            .Where(pos => enemy.IsWalkable(pos.x, pos.y, map))
            .ToList();

        (int x, int y) bestMove = (enemy.PosX, enemy.PosY);
        int bestDistance = currentDistance;

        foreach (var move in walkableMoves)
        {
            int dist = map.GetShortestDistance(move.x, move.y, closestPlayer.PosX, closestPlayer.PosY, MaxPlayerConsiderDistance);
            if (dist > bestDistance)
            {
                bestDistance = dist;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private void MoveEnemyIfNeeded(Enemy enemy, (int x, int y) bestMove, Map map)
    {
        if (bestMove != (enemy.PosX, enemy.PosY))
        {
            int deltaX = bestMove.x - enemy.PosX;
            int deltaY = bestMove.y - enemy.PosY;
            enemy.TryWalk(deltaX, deltaY, map); 
        }
    }
    private void AdjustEnemyStrategyAfterFight(Enemy enemy)
    {
        if (enemy.Health < MinHealthForAgressiveStrategy)
        {
            enemy.SetStrategy(new SkittishStrategy());
        }
        else
        {
            enemy.SetStrategy(new AggresiveStrategy());
        }
    }
}
