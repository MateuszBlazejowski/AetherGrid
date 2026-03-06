using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Technical;
using We_have_doom_at_home.World;

namespace We_have_doom_at_home.Rendering;

public class ClientImageCreator
{
    public char[][] Buffer { get; private set; }

    public ClientImageCreator(int width, int height)
    {
        Buffer = new char[height][];
        for (int i = 0; i < height; i++)
            Buffer[i] = new char[width];
        ClearBuffer();

        mapBuffer = new char[MapWidth + 2, MapHeight + 2];
        mapColorBuffer = new ConsoleColor[MapWidth + 2, MapHeight + 2];
        mapColorBufferPrevious = new ConsoleColor[MapWidth + 2, MapHeight + 2];
    }

    public void ClearBuffer()
    {
        for (int y = 0; y < Buffer.Length; y++)
            for (int x = 0; x < Buffer[y].Length; x++)
                Buffer[y][x] = ' ';
    }

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


    public bool ExistItems { get; set; }
    public bool ExistEnemies { get; set; }
    public bool ExistPotions { get; set; }

  
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
    public void UpdateMapAndColorBuffer(Map map, Player player, List<Player> players)
    {
        ClearMapAndColorBuffers();
        for (int x = 0; x < MapWidth + 2; x++)
        {
            mapBuffer[x, 0] = WallSymbol;
            mapBuffer[x, MapHeight + 1] = WallSymbol;
            if (!player.IsInInventory)
            {
                mapColorBuffer[x, 0] = WallColor;
                mapColorBuffer[x, MapHeight + 1] = WallColor;
            }
        }

        for (int y = 0; y < MapHeight + 2; y++)
        {
            mapBuffer[0, y] = WallSymbol;
            mapBuffer[MapWidth + 1, y] = WallSymbol;
            if (!player.IsInInventory)
            {
                mapColorBuffer[0, y] = WallColor;
                mapColorBuffer[MapWidth + 1, y] = WallColor;
            }
        }

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                switch (map.tileMap[x, y])
                {
                    case Tile.Wall:
                        mapBuffer[x + 1, y + 1] = WallSymbol;
                        if (!player.IsInInventory)
                        {
                            mapColorBuffer[x + 1, y + 1] = WallColor;
                        }
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
                    if (!player.IsInInventory)
                    {
                        mapColorBuffer[item.PosX + 1, item.PosY + 1] = WeaponColor;
                    }
                    break;
                case ItemType.NonUsable:
                    mapBuffer[item.PosX + 1, item.PosY + 1] = NonusableItemSymbol;
                    if (!player.IsInInventory)
                    {
                        mapColorBuffer[item.PosX + 1, item.PosY + 1] = NonusableColor;
                    }
                    break;
                case ItemType.Currency:
                    mapBuffer[item.PosX + 1, item.PosY + 1] = CurrencySymbol;
                    if (!player.IsInInventory)
                    {
                        mapColorBuffer[item.PosX + 1, item.PosY + 1] = CurrencyColor;
                    }
                    break;
                case ItemType.Potion:
                    mapBuffer[item.PosX + 1, item.PosY + 1] = PotionSymbol;
                    if (!player.IsInInventory)
                    {
                        mapColorBuffer[item.PosX + 1, item.PosY + 1] = PotionColor;
                    }
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
            if (!player.IsInInventory)
            {
                mapColorBuffer[enemy.PosX + 1, enemy.PosY + 1] = EnemyColor;
            }


        }
        lock (players)
        {
            foreach (var playerOther in players)
            {
                if (playerOther.IsAlive)
                    mapBuffer[playerOther.PosX + 1, playerOther.PosY + 1] = PlayerSymbol;
                else
                    mapBuffer[playerOther.PosX + 1, playerOther.PosY + 1] = DeadSymbol; 
            }
        }

        if (player.IsAlive)
        {
            mapBuffer[player.PosX + 1, player.PosY + 1] = PlayerSymbol;
            if (!player.IsInInventory)
            {
                mapColorBuffer[player.PosX + 1, player.PosY + 1] = PlayerColor;
            }
        }
        else
        {
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
    private void WriteStringToBuffer(int x, int y, string text)
    {
        if (y < 0 || y >= Buffer.Length)
            return;
        for (int i = 0; i < text.Length && x + i < Buffer[y].Length; i++)
        {
            Buffer[y][x + i] = text[i];
        }
    }

    public void RenderHands(Player player)
    {
        int x = mapDrawingStartingPointX;
        int y = mapDrawingStartingPointY + MapHeight + 3;

        if (player.HandInUse == PointedHand.Left)
        {
            // Simulate color with no-op, skip
            WriteStringToBuffer(x, y, "> Left Hand: ");
            x += "> Left Hand: ".Length;
            WriteStringToBuffer(x, y, player.LeftHand?.ToString() ?? "");
            WriteStringToBuffer(x + (player.LeftHand?.ToString().Length ?? 0), y, new string(' ', 50));
            lastLeftHandLength = player.LeftHand?.Name.Length ?? 0;
        }
        else
        {
            WriteStringToBuffer(x, y, "  Left Hand:  ");
            x += "  Left Hand:  ".Length;
            WriteStringToBuffer(x, y, player.LeftHand?.ToString() ?? "");
            WriteStringToBuffer(x + (player.LeftHand?.ToString().Length ?? 0), y, new string(' ', 50));
            lastLeftHandLength = player.LeftHand?.Name.Length ?? 0;
        }

        x = mapDrawingStartingPointX;
        y = mapDrawingStartingPointY + MapHeight + 5;

        if (player.HandInUse == PointedHand.Right)
        {
            // Simulate color with no-op, skip
            WriteStringToBuffer(x, y, "> Right Hand:  ");
            x += "> Right Hand:  ".Length;
            WriteStringToBuffer(x, y, player.RightHand?.ToString() ?? "");
            WriteStringToBuffer(x + (player.RightHand?.ToString().Length ?? 0), y, new string(' ', 50));
            lastRightHandLength = player.RightHand?.Name.Length ?? 0;
        }
        else
        {
            WriteStringToBuffer(x, y, "  Right Hand:  ");
            x += "  Right Hand:  ".Length;
            WriteStringToBuffer(x, y, player.RightHand?.ToString() ?? "");
            WriteStringToBuffer(x + (player.RightHand?.ToString().Length ?? 0), y, new string(' ', 50));
            lastRightHandLength = player.RightHand?.Name.Length ?? 0;
        }
    }


    public void RenderStatsAndInventory(Player player)
    {
        char[,] StatsAndInventoryBuffer = new char[StatsAndInventoryWidth, MapHeight + 2];
        // Removed color buffer because you requested no colors

        // Clear the buffer
        for (int y = 0; y < StatsAndInventoryBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < StatsAndInventoryBuffer.GetLength(0); x++)
            {
                StatsAndInventoryBuffer[x, y] = ' '; // Fill with spaces initially
            }
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
            InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, "Inventory: <-");
        }
        else
        {
            InsertStringIntoBuffer(StatsAndInventoryBuffer, 0, line++, "Inventory:");
        }

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

            // Highlight the selected item - **color removed**, but add markers instead
            if (inventoryPosition == player.InventoryIndex && player.IsInInventory)
            {
                itemName = $"> {itemName} <";
            }

            InsertStringIntoBuffer(StatsAndInventoryBuffer, 2, line++, itemName);
        }

        // Instead of printing to console, copy this buffer to your main view buffer at proper offset.
        // For example, if you have _view.Buffer (char[][]), you would copy:
        // Here, I'll just show a placeholder how you could do that:

        int targetX = mapDrawingStartingPointX + MapWidth + 3;
        int targetY = mapDrawingStartingPointY;
        for (int y = 0; y < StatsAndInventoryBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < StatsAndInventoryBuffer.GetLength(0); x++)
            {

                  Buffer[targetY + y][targetX + x] = StatsAndInventoryBuffer[x, y];
            }
        }
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

        var closestEnemies = map.enemies.OrderBy(enemy => map.GetShortestDistance(player.PosX, player.PosY ,enemy.PosX, enemy.PosY, MaxConsideredSearchDistance))
                                        .Take(2)
                                        .ToList();
        if (closestEnemies.Count > 0)
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, $"{closestEnemies[0].Name} (HP:{closestEnemies[0].Health}, AT:{closestEnemies[0].Attack}, AR:{closestEnemies[0].Armor}) {map.GetShortestDistance(player.PosX, player.PosY, closestEnemies[0].PosX, closestEnemies[0].PosY, MaxConsideredSearchDistance)} moves from player                        ");
        else
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, $"                                                          ");
        if (closestEnemies.Count > 1)
            InsertStringIntoBuffer(ItemsOnTileAndLogBuffer, 0, line++, $"{closestEnemies[1].Name} (HP:{closestEnemies[1].Health}, AT:{closestEnemies[1].Attack}, AR:{closestEnemies[1].Armor}) {map.GetShortestDistance(player.PosX, player.PosY, closestEnemies[1].PosX, closestEnemies[1].PosY, MaxConsideredSearchDistance)} moves from player                        ");
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

        // Copy buffer content to main _view.Buffer at correct position
        for (int y = 0; y < ItemsOnTileAndLogBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < ItemsOnTileAndLogBuffer.GetLength(0); x++)
            {
                Buffer[startY + y][startX + x] = ItemsOnTileAndLogBuffer[x, y];
            }
        }
    }

    public void RenderDeathMessage()
    {
        Console.Clear();
        Console.Write(@"
                                 _____  _____
                                <     `/     |
                                 >          (
                                |   _     _  |
                                |  |_) | |_) |
                                |  | \ | |   |
                                |            |
                 ______.______%_|            |__________  _____
               _/                                       \|     |
              |             B R A V E  P L A Y E R            <
              |_____.-._________              ____/|___________|
                                |            |
                                |            |
                                |            |
                                |            |
                                |   _        <
                                |__/         |
                                 / `--.      |
                               %|            |%
                           |/.%%|          -< @%%%
                           `\%`@|     v      |@@%@%%   
                         .%%%@@@|%    |    % @@@%%@%%%%
                    _.%%%%%%@@@@@@%%_/%\_%@@%%@@@@@@@%%%%%%                                                                               
                                                                                                  
                                                                                                  ");

    }

    public char[][] RenderPlayerImage(Map map, Player player, List<Player> players)
    {
        Console.CursorVisible = false;

        // 1. Update your map buffer (remove colors, update only _view.Buffer chars)
        UpdateMapAndColorBuffer(map, player, players);
        for (int y = 0; y < MapHeight + 2; y++)
        {
            for (int x = 0; x < MapWidth + 2; x++)
            {
                // mapDrawingStartingPointX/Y are the top-left console coords for the map drawing
                Buffer[mapDrawingStartingPointY + y][mapDrawingStartingPointX + x] = mapBuffer[x, y];
            }
        }
        // 2. Render other parts into the shared buffer (they each write to _view.Buffer):
        RenderHands(player);
        RenderStatsAndInventory(player);
        RenderItemsOnTileAndHints(map, player);

        return Buffer; 
        // If you want to keep previous color buffer for diffing, you can keep this line (but color is removed)
        // mapColorBufferPrevious = (ConsoleColor[,])mapColorBuffer.Clone();
    }
    public void FlushBufferToConsole()
    {
        Console.Clear();
        for (int y = 0; y < Buffer.Length; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(Buffer[y]);
        }
    }


}
