using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Items;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.World.EntitiesCreator;

namespace We_have_doom_at_home.World.Builder;

public class MazeBuilder : IBuilder
{
    public IBuilder emptyDungeon()
    {
        if (currentMap == null)
        {
            currentMap = new Map();
        }
        for (int y = 0; y < currentMap.mapHeight; y++)
        {
            for (int x = 0; x < currentMap.mapWidth; x++)
            {
                currentMap.tileMap[x, y] = Tile.Floor;
            }
        }
        return this;
    }

    public IBuilder filledDungeon()
    {
        if (currentMap == null)
        {
            currentMap = new Map();
        }
        for (int y = 0; y < currentMap.mapHeight; y++)
        {
            for (int x = 0; x < currentMap.mapWidth; x++)
            {
                currentMap.tileMap[x, y] = Tile.Wall;
            }
        }
        return this;
    }

    public IBuilder addPaths()
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        bool[,] visited = new bool[currentMap.mapWidth, currentMap.mapHeight];

        for (int y = 0; y < currentMap.mapHeight; y++)
        {
            for (int x = 0; x < currentMap.mapWidth; x++)
            {
                visited[x, y] = false;
            }
        }

        // Start at (0,0)
        currentMap.tileMap[0, 0] = Tile.Floor;
        visited[0, 0] = true;

        // Call the recursive maze generation function
        RecursiveDFS(0, 0, visited);
        return this;
    }
    private void RecursiveDFS(int x, int y, bool[,] visited)
    {
        Random rand = new Random();
        // Directions: Up, Down, Left, Right
        var directions = new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
        // Shuffle directions to randomize the maze generation
        directions = directions.OrderBy(a => rand.Next()).ToArray();

        foreach (var direction in directions)
        {
            int nx = x + direction.Item1 * 2;
            int ny = y + direction.Item2 * 2;

            if (nx >= 0 && ny >= 0 && nx < currentMap.mapWidth && ny < currentMap.mapHeight && !visited[nx, ny])
            {
                // Carve the wall between current and new position
                currentMap.tileMap[nx - direction.Item1, ny - direction.Item2] = Tile.Floor;
                currentMap.tileMap[nx, ny] = Tile.Floor;
                visited[nx, ny] = true;

                RecursiveDFS(nx, ny, visited);
            }
        }
    }
    public IBuilder addChambers()
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        Random random = new Random();
        int randomSize;
        int roomsNumber = random.Next(BuilderMinChambersNumber,
            BuilderMaxChambersNumber + 1);

        int startingX;
        int startingY;

        for (int i = 0; i < roomsNumber; i++)
        {
            randomSize = random.Next(BuilderMinChamberSize,
            BuilderMaxChamberSize + 1);

            startingX = random.Next(0, currentMap.mapWidth - randomSize + 1);
            startingY = random.Next(0, currentMap.mapHeight - randomSize + 1);

            for (int y = 0; y < randomSize; y++)
            {
                for (int x = 0; x < randomSize; x++)
                {
                    currentMap.tileMap[startingX + x, startingY + y] = Tile.Floor;
                }
            }
        }
        return this;
    }

    public IBuilder addCentralChamber()
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        Random random = new Random();
        int randomSize = random.Next(BuilderMinCentralRoomSize,
            BuilderMaxCentralRoomSize + 1);

        int startingX;
        int startingY;

        startingX = currentMap.mapWidth / 2 - randomSize / 2;
        startingY = currentMap.mapHeight / 2 - randomSize / 3;

        for (int y = 0; y < randomSize * 2 / 3; y++)
        {
            for (int x = 0; x < randomSize; x++)
            {
                currentMap.tileMap[startingX + x, startingY + y] = Tile.Floor;
            }
        }
        return this;
    }

    public IBuilder addWeapons()
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        Random random = new Random();
        WeaponCreator creator = new WeaponCreator();
        int itemNum = random.Next(MinItemNumber,
           MaxItemNumber + 1);
        int X, Y;

        for (int i = 0; i < itemNum; i++)
        {
            for (int j = 0; j < terminationCounterForItemInicialization; j++)
            {
                X = random.Next(0, currentMap.mapWidth);
                Y = random.Next(0, currentMap.mapHeight);
                if (currentMap.tileMap[X, Y] == Tile.Wall)
                    continue;

                var weapon = creator.GetWeapon(X, Y);
                currentMap.items.Add(weapon);
                break;
            }
        }
        return this;
    }

    public IBuilder addModifiedWeapons()
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        Random random = new Random();
        ModifiedWeaponCreator creator = new ModifiedWeaponCreator(); // Create the ModifiedWeaponCreator instance
        int itemNum = random.Next(MinItemNumber, MaxItemNumber + 1); // Determine the number of modified weapons to add
        int X, Y;

        for (int i = 0; i < itemNum; i++)
        {
            for (int j = 0; j < terminationCounterForItemInicialization; j++)
            {
                X = random.Next(0, currentMap.mapWidth); // Random X position on the map
                Y = random.Next(0, currentMap.mapHeight); // Random Y position on the map

                if (currentMap.tileMap[X, Y] == Tile.Wall)  // Ensure it's not placed on a wall
                    continue;

                var weapon = creator.GetModifiedWeapon(X, Y); // Create a modified weapon with the random position
                currentMap.items.Add(weapon); // Add the weapon to the map
                break; // Exit the loop once the weapon has been placed
            }
        }
        return this; // Return the builder to allow method chaining
    }


    public IBuilder addItems() // generates non-usable or a coin 
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        Random random = new Random();
        ItemCreator creator = new ItemCreator();
        int itemNum = random.Next(MinItemNumber,
           MaxItemNumber + 1);
        int X, Y;

        for (int i = 0; i < itemNum; i++)
        {
            for (int j = 0; j < terminationCounterForItemInicialization; j++)
            {
                X = random.Next(0, currentMap.mapWidth);
                Y = random.Next(0, currentMap.mapHeight);
                if (currentMap.tileMap[X, Y] == Tile.Wall)
                    continue;

                var item = creator.GetItem(X, Y);
                currentMap.items.Add(item);
                break;
            }
        }
        return this;
    }

    public IBuilder addPotions()
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        Random random = new Random();
        PotionsCreator creator = new PotionsCreator();

        int potionNum = random.Next(MinItemNumber,
           MaxItemNumber + 1);
        int X, Y;

        for (int i = 0; i < potionNum; i++)
        {
            for (int j = 0; j < terminationCounterForItemInicialization; j++)
            {
                X = random.Next(0, currentMap.mapWidth);
                Y = random.Next(0, currentMap.mapHeight);
                if (currentMap.tileMap[X, Y] == Tile.Wall)
                    continue;

                var potion = creator.GetPotion(X, Y);
                currentMap.items.Add(potion);
                break;

            }

        }
        return this;
    }

    public IBuilder addEnemies()
    {
        if (currentMap == null)
            throw new Exception("No Starting strategy was applied");

        Random random = new Random();
        EnemyCreator creator = new EnemyCreator();

        int enemyNum = random.Next(MinEnemyNumber,
           MaxEnemyNumber + 1);
        int X, Y;

        for (int i = 0; i < enemyNum; i++)
        {
            for (int j = 0; j < terminationCounterForItemInicialization; j++)
            {
                X = random.Next(0, currentMap.mapWidth);
                Y = random.Next(0, currentMap.mapHeight);
                if (currentMap.tileMap[X, Y] == Tile.Wall)
                { 
                    continue;
                }

                var enemy = creator.GetEnemy(X, Y);
                currentMap.enemies.Add(enemy);
                break;

            }

        }
        return this;
    }

    private Map? currentMap = null;

    public Map? GetMap()
    {
        return currentMap;
    }
}

