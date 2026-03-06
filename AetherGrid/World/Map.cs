using We_have_doom_at_home.Entities.Decorators;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Items;
using We_have_doom_at_home.Technical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using System.ComponentModel.DataAnnotations;
using We_have_doom_at_home.Entities;


namespace We_have_doom_at_home.World;

public class Map
{
    public int mapHeight;
    public int mapWidth;
    public Tile[,] tileMap;
    public List<IItem> items;
    public List<Enemy> enemies;
    public Map()
    {
        mapHeight = MapHeight;
        mapWidth = MapWidth;
        tileMap = new Tile[mapWidth, mapHeight];
        items = new List<IItem>();
        enemies = new List<Enemy>();

    }

    // BFS to find shortest path distance on grid
    public int GetShortestDistance(int startX, int startY, int targetX, int targetY, int maxMoves)
    {
        if (startX == targetX && startY == targetY)
            return 0;

        int manhattanDistance = Math.Abs(startX - targetX) + Math.Abs(startY - targetY);
        if (manhattanDistance > maxMoves)
            return int.MaxValue;

        int width = mapWidth;
        int height = mapHeight;

        bool[,] visited = new bool[width, height];
        Queue<(int x, int y, int dist)> queue = new Queue<(int x, int y, int dist)>();

        queue.Enqueue((startX, startY, 0));
        visited[startX, startY] = true;

        int[] dx = new int[] { 0, 0, -1, 1 };
        int[] dy = new int[] { -1, 1, 0, 0 };

        while (queue.Count > 0)
        {
            var (x, y, dist) = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx == targetX && ny == targetY)
                    return dist + 1;

                if (nx >= 0 && ny >= 0 && nx < width && ny < height
                    && !visited[nx, ny]
                    && tileMap[nx, ny] != Tile.Wall)
                {
                    visited[nx, ny] = true;
                    queue.Enqueue((nx, ny, dist + 1));
                }
            }
        }

        // No path found
        return int.MaxValue;
    }
}


