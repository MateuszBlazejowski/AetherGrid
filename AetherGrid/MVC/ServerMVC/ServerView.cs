using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Rendering;
using We_have_doom_at_home.Technical;
using We_have_doom_at_home.World;

namespace We_have_doom_at_home.MVC.ServerMVC;

public class ServerView
{
    private int consoleMidX = 0;
    private int consoleMidY = 0;
    private int mapDrawingStartingPointX = 2;
    private int mapDrawingStartingPointY = 1;
    private int lastWidth = Console.WindowWidth;
    private int lastHeight = Console.WindowHeight;
    private int lastLeftHandLength = 0;
    private int lastRightHandLength = 0;
    private char[,] mapBuffer;
    private ConsoleColor[,] mapColorBuffer;
    private ConsoleColor[,] mapColorBufferPrevious;

    private static ServerView _instance;

    public bool ExistItems { get; set; }
    public bool ExistEnemies { get; set; }
    public bool ExistPotions { get; set; }

    private ServerView()
    {
        Console.Clear();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        FindMiddle();
        FindDrawingStartPoint();
        mapBuffer = new char[MapWidth + 2, MapHeight + 2];
        mapColorBuffer = new ConsoleColor[MapWidth + 2, MapHeight + 2];
        mapColorBufferPrevious = new ConsoleColor[MapWidth + 2, MapHeight + 2];
        SetConsoleSizeMax();
    }

    public static ServerView GetInstance()
    {
        if (_instance == null)
        {
            lock (typeof(ServerView)) // thread safety for future generations??? 
            {
                if (_instance == null)
                {
                    _instance = new ServerView();
                }
            }
        }
        return _instance;
    }

    public void SetConsoleSizeMax()
    {
        int maxWidth = Console.LargestWindowWidth;
        int maxHeight = Console.LargestWindowHeight;

        Console.SetWindowSize(maxWidth, maxHeight);
    }
    public void FindMiddle()
    {
        consoleMidX = Console.WindowWidth / 2;
        consoleMidY = Console.WindowHeight / 2;
    }
    public void FindDrawingStartPoint()
    {
        mapDrawingStartingPointX = 2;
        mapDrawingStartingPointY = 1;
    }

    public void CheckConsoleDimensions()
    {
        if (Console.WindowWidth < ConsoleMinimalSizeX || Console.WindowHeight < ConsoleMinialSizeY)
        {
            Console.Clear();
            Console.WriteLine("Console window is too small to fit the display!!!");
            Console.WriteLine();
            Console.WriteLine("ZOOM OUT CONSOLE WINDOW \n or \nINCREASE CONSOLE WINDOW SIZE");
            while (true)
            {
                Thread.Sleep(10);
                if (!(Console.WindowWidth < ConsoleMinimalSizeX || Console.WindowHeight < ConsoleMinialSizeY))
                {
                    Console.Clear();
                    return;
                }
            }
        }
    }
    public void ClearMapAndColorBuffers()
    {
        for (int y = 0; y < MapHeight + 2; y++)
        {
            for (int x = 0; x < MapWidth + 2; x++)
            {
                mapBuffer[x, y] = FloorSymbol;
                mapColorBuffer[x, y] = DefaulftForegroundColor;
            }
        }
    }
    public void UpdateMapAndColorBuffer(Map map, List<Player> players)
    {
        ClearMapAndColorBuffers();
        for (int x = 0; x < MapWidth + 2; x++)
        {
            mapBuffer[x, 0] = WallSymbol;
            mapBuffer[x, MapHeight + 1] = WallSymbol;

            mapColorBuffer[x, 0] = WallColor;
            mapColorBuffer[x, MapHeight + 1] = WallColor;

        }

        for (int y = 0; y < MapHeight + 2; y++)
        {
            mapBuffer[0, y] = WallSymbol;
            mapBuffer[MapWidth + 1, y] = WallSymbol;

            mapColorBuffer[0, y] = WallColor;
            mapColorBuffer[MapWidth + 1, y] = WallColor;

        }

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                switch (map.tileMap[x, y])
                {
                    case Tile.Wall:
                        mapBuffer[x + 1, y + 1] = WallSymbol;
                        mapColorBuffer[x + 1, y + 1] = WallColor;

                        break;

                    case Tile.Floor:
                        mapBuffer[x + 1, y + 1] = FloorSymbol;
                        break;

                    default:
                        throw new Exception("map is not tiled properly");
                }

            }
        }

        foreach (IItem item in map.items)
        {
            if (map.tileMap.GetLength(0) <= item.PosX || map.tileMap.GetLength(1) <= item.PosY)
            {
                Log.Logs.Add($"item beyond the borders {item.Name}");
                continue; // skip if is laying beyond the boundaries
            }
            if (map.tileMap[item.PosX, item.PosY] == Tile.Wall) continue; // skip if is laying on the wall
            switch (item.Type)
            {
                case ItemType.Weapon:
                    mapBuffer[item.PosX + 1, item.PosY + 1] = WeaponSymbol;
                    mapColorBuffer[item.PosX + 1, item.PosY + 1] = WeaponColor;
                    break;
                case ItemType.NonUsable:
                    mapBuffer[item.PosX + 1, item.PosY + 1] = NonusableItemSymbol;
                    mapColorBuffer[item.PosX + 1, item.PosY + 1] = NonusableColor;
                    break;
                case ItemType.Currency:
                    mapBuffer[item.PosX + 1, item.PosY + 1] = CurrencySymbol;
                    mapColorBuffer[item.PosX + 1, item.PosY + 1] = CurrencyColor;
                    break;
                case ItemType.Potion:
                    mapBuffer[item.PosX + 1, item.PosY + 1] = PotionSymbol;
                    mapColorBuffer[item.PosX + 1, item.PosY + 1] = PotionColor;

                    break;
                default: break;
            }

        }

        foreach (var enemy in map.enemies)
        {
            if (map.tileMap.GetLength(0) <= enemy.PosX || map.tileMap.GetLength(1) <= enemy.PosY)
            {
                Log.Logs.Add($"item beyond the borders {enemy.Name}");
                continue; // skip if is laying beyond the boundaries
            }
            if (map.tileMap[enemy.PosX, enemy.PosY] == Tile.Wall) continue; // skip if is laying on the wall
            mapBuffer[enemy.PosX + 1, enemy.PosY + 1] = EnemySymbol;
            mapColorBuffer[enemy.PosX + 1, enemy.PosY + 1] = EnemyColor;
        }

        foreach (var player in players)
        {
            if (player.IsAlive)
                mapBuffer[player.PosX + 1, player.PosY + 1] = PlayerSymbol;
            else
                mapBuffer[player.PosX + 1, player.PosY + 1] = DeadSymbol;
            mapColorBuffer[player.PosX + 1, player.PosY + 1] = PlayerColor;
        }

    }
    public void InsertStringIntoBuffer(char[,] buffer, int x, int y, string text)
    {
        if (y >= buffer.GetLength(1)) return;
        for (int i = 0; i < text.Length; i++)
        {
            if (x + i < buffer.GetLength(0)) // Prevent overflow
            {
                buffer[x + i, y] = text[i];
            }
        }
    }

    public void RenderHands(Player player)
    {
        Console.SetCursorPosition(mapDrawingStartingPointX, mapDrawingStartingPointY + MapHeight + 3);
        if (player.HandInUse == PointedHand.Left)
        {
            Console.ForegroundColor = PointingColor;
            Console.Write($"> Left Hand: ");
            Console.ForegroundColor = DefaulftForegroundColor;
        }
        else
            Console.Write($"  Left Hand:  ");

        if (player.LeftHand != null)
        {
            lastLeftHandLength = player.LeftHand.Name.Length;
            Console.Write($"{player.LeftHand.ToString()}" + new string(' ', 50));
        }
        else
            Console.Write(new string(' ', 50));




        Console.SetCursorPosition(mapDrawingStartingPointX, mapDrawingStartingPointY + MapHeight + 5);
        if (player.HandInUse == PointedHand.Right)
        {
            Console.ForegroundColor = PointingColor;
            Console.Write($"> Right Hand:  ");
            Console.ForegroundColor = DefaulftForegroundColor;
        }
        else
            Console.Write($"  Right Hand:  ");
        if (player.RightHand != null)
        {
            lastRightHandLength = player.RightHand.Name.Length;
            Console.Write($"{player.RightHand.ToString()}" + new string(' ', 50));
        }
        else
            Console.Write(new string(' ', 50));
    }

    public void RenderStatsAndInventory(Player player)
    {
        char[,] StatsAndInventoryBuffer = new char[StatsAndInventoryWidth, MapHeight + 2];
        ConsoleColor[] StatsAndInventoryLineColorBuffer = new ConsoleColor[MapHeight + 2];
        // Clear the buffer
        for (int y = 0; y < StatsAndInventoryBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < StatsAndInventoryBuffer.GetLength(0); x++)
            {
                StatsAndInventoryBuffer[x, y] = ' '; // Fill with spaces initially
            }
            StatsAndInventoryLineColorBuffer[y] = DefaulftForegroundColor;
        }

        int line = 0; // Track which row we are on
        InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, "================================================================");
        InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, "Stats:");

        var additionalEffects = new string[6];
        foreach (var potion in player.WorkingPotions)
        {
            foreach (var effect in potion.GetPotionEffects())
            {
                switch (effect.Key)
                {
                    case StatType.Strength: additionalEffects[0] += $" +{effect.Value}"; break;
                    case StatType.Dexterity: additionalEffects[1] += $" +{effect.Value}"; break;
                    case StatType.Health: additionalEffects[2] += $" +{effect.Value}"; break;
                    case StatType.Luck: additionalEffects[3] += $" +{effect.Value}"; break;
                    case StatType.Aggresion: additionalEffects[4] += $" +{effect.Value}"; break;
                    case StatType.Wisdom: additionalEffects[5] += $" +{effect.Value}"; break;
                }
            }
        }


        string[] stats = {
        $"STR: {player.Strength} {additionalEffects[0]}                 ",
        $"DEX: {player.Dexterity} {additionalEffects[1]}                ",
        $"HP: {player.Health} {additionalEffects[2]}                    ",
        $"LCK: {player.Luck} {additionalEffects[3]}                     ",
        $"AGG: {player.Aggresion} {additionalEffects[4]}                ",
        $"WIS: {player.Wisdom} {additionalEffects[5]}                   "
        };



        foreach (string stat in stats)
        {
            InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, stat);
        }

        InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, "================================================================");
        var GoldIngotNumber = player.Poach.Where(c => c._CurrencyType == CurrencyType.Gold).Count();
        InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, $"Gold Ingots: {GoldIngotNumber}");
        var CoinsNumber = player.Poach.Where(c => c._CurrencyType == CurrencyType.Coins).Count();
        InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, $"Coins: {CoinsNumber}");

        if (player.IsInInventory)
        {
            if (player.IsInInventory)
                StatsAndInventoryLineColorBuffer[line] = PointingColor;
            InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, "Inventory: <-");
        }
        else
            InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, "Inventory:");

        int remainingLines = MapHeight - line + 2;
        int maxVisibleItems = remainingLines;  // Max number of items visible at once

        // Ensuring InventoryIndex is within bounds
        if (player.Inventory.Count != 0)
            player.InventoryIndex = Math.Clamp(player.InventoryIndex, 0, player.Inventory.Count - 1);

        // Calculating scrolling offset
        if (player.InventoryIndex >= player.InventoryScrollOffset + maxVisibleItems)
        {
            player.InventoryScrollOffset = player.InventoryIndex - maxVisibleItems + 1;
        }
        else if (player.InventoryIndex < player.InventoryScrollOffset)
        {
            player.InventoryScrollOffset = player.InventoryIndex;
        }

        // Display only the visible part of the inventory
        for (int i = 0; i < maxVisibleItems; i++)
        {
            int inventoryPosition = i + player.InventoryScrollOffset;
            if (inventoryPosition >= player.Inventory.Count) break;

            string itemName = player.Inventory[inventoryPosition].ToString();

            // Highlight the selected item
            if (inventoryPosition == player.InventoryIndex && player.IsInInventory)
            {
                StatsAndInventoryLineColorBuffer[line] = PointingColor;
                itemName = $"> {itemName} <";
            }

            InsertStringIntoBuffer(StatsAndInventoryBuffer, 2, line++, itemName);
        }

        Console.SetCursorPosition(mapDrawingStartingPointX + MapWidth + 3, mapDrawingStartingPointY);
        for (int y = 0; y < StatsAndInventoryBuffer.GetLength(1); y++)
        {
            Console.ForegroundColor = StatsAndInventoryLineColorBuffer[y];
            for (int x = 0; x < StatsAndInventoryBuffer.GetLength(0); x++)
            {
                Console.Write(StatsAndInventoryBuffer[x, y]);
            }
            Console.SetCursorPosition(mapDrawingStartingPointX + MapWidth + 3, mapDrawingStartingPointY + y + 1);
        }
        Console.ForegroundColor = DefaulftForegroundColor;
    }

    public void RenderItemsOnTileAndHints(Map map, Player player)
    {
        char[,] ItemsOnTileAndLogBuffer = new char[StatsAndInventoryWidth, MapHeight + 2 + LogConsoleHeight + 4];
        int startX = mapDrawingStartingPointX + MapWidth + 3 + StatsAndInventoryWidth + 1;
        int startY = mapDrawingStartingPointY;

        // Clear the buffer 
        for (int y = 0; y < ItemsOnTileAndLogBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < ItemsOnTileAndLogBuffer.GetLength(0); x++)
            {
                ItemsOnTileAndLogBuffer[x, y] = ' ';
            }
        }

        int line = 0; // Track which row we are on
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "===============================================================");
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "Items on current tile:");

        var Items = map.items.Where(item => item.PosX == player.PosX && item.PosY == player.PosY).ToList();

        foreach (var Item in Items)
        {
            if (line >= ItemsOnTileAndLogBuffer.GetLength(1) / 2) break;
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, Item.ToString());
        }

        // closest enemies 
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line = ItemsOnTileAndLogBuffer.GetLength(1) / 2 - 1, "===============================================================");
        line++;
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "Closest enemies:");

        var closestEnemies = map.enemies.OrderBy(enemy => enemy.GetDistanceToPlayer(player.PosX, player.PosY))
                                         .Take(2)
                                         .ToList();
        if (closestEnemies.Count > 0)
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, $"{closestEnemies[0].Name} at: X:{closestEnemies[0].PosX - player.PosX}, Y:{closestEnemies[0].PosY - player.PosY} from player                       ");
        else
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, $"                                                          ");
        if (closestEnemies.Count > 1)
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, $"{closestEnemies[1].Name} at: X:{closestEnemies[1].PosX - player.PosX}, Y: {closestEnemies[1].PosY - player.PosY} from player                      ");
        else
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, $"                                                          ");
        // end of enemies

        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line = ItemsOnTileAndLogBuffer.GetLength(1) / 2 + 3, "===============================================================");
        line++;
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "Press:  W, S, A, D to move");
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  Spacebar to change hands");
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  Q to enter / exit inventory");
        if (ExistItems)
        {
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  E to pickup item, R to drop item from hand  ");
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "/from inventory (if player is in inventory)");
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  T to drop all items");
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  X to unequip item from current hand");
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "/equip (selected from inventory) to current hand");
        }
        if (ExistPotions)
        {
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  C to consume choosen potion from inventory");
        }
        if (ExistEnemies)
        {
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  To attack press: 1-normal, 2-stealth, 3-magic");
        }
        InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, "  Esc to exit game");

        Console.SetCursorPosition(startX, startY);
        for (int y = 0; y < ItemsOnTileAndLogBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < ItemsOnTileAndLogBuffer.GetLength(0); x++)
            {
                Console.Write(ItemsOnTileAndLogBuffer[x, y]);
            }
            Console.SetCursorPosition(startX, startY + 1 + y);
        }
    }
    public void RenderLog(IEnumerable<string> gameLogs)
    {
        char[,] LogBuffer = new char[MapWidth, LogConsoleHeight];
        int startX = mapDrawingStartingPointX;
        int startY = mapDrawingStartingPointY + MapHeight + 6;
        // Clear the buffer
        for (int y = 0; y < LogBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < LogBuffer.GetLength(0); x++)
            {
                LogBuffer[x, y] = ' ';
            }
        }

        int line = 0; // Track which row we are on
        InsertStringIntoBuffer(LogBuffer, 0, line++, "===============================================================");
        InsertStringIntoBuffer(LogBuffer, 0, line++, "Logs: ");

        List<string> lastLogs = gameLogs.TakeLast(LogConsoleHeight - 2).ToList();

        // Insert logs into the buffer, ensuring the latest log appears at the bottom
        int logStartLine = LogConsoleHeight - lastLogs.Count; // Start displaying from the bottom
        for (int i = 0; i < lastLogs.Count; i++)
        {
            InsertStringIntoBuffer(LogBuffer, 0, logStartLine + i, lastLogs[i]);
        }
        // Print buffer to console
        Console.SetCursorPosition(startX, startY);
        for (int y = 0; y < LogBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < LogBuffer.GetLength(0); x++)
            {
                Console.Write(LogBuffer[x, y]);
            }
            Console.SetCursorPosition(startX, startY + 1 + y);
        }
    }


    public void RenderWorld(Map map, List<Player> players, IEnumerable<string> logs)
    {
        Console.CursorVisible = false;
        //CheckConsoleDimensions();
        UpdateMapAndColorBuffer(map, players);

        Console.SetCursorPosition(mapDrawingStartingPointX, mapDrawingStartingPointY);
        for (int y = 0; y < MapHeight + 2; y++)
        {
            for (int x = 0; x < MapWidth + 2; x++)
            {
                Console.ForegroundColor = mapColorBuffer[x, y];
                Console.Write(mapBuffer[x, y]);
            }
            Console.SetCursorPosition(mapDrawingStartingPointX, mapDrawingStartingPointY + y + 1);
        }
        Console.ForegroundColor = DefaulftForegroundColor;
        RenderLog(logs);

        mapColorBufferPrevious = (ConsoleColor[,])mapColorBuffer.Clone();
    }
}
