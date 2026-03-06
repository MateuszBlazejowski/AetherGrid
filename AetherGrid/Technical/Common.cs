using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace We_have_doom_at_home.Technical;

public static class Common
{
    public static readonly int MapWidth = 41;
    public static readonly int MapHeight = 21; // not smaller than 9
    public static readonly int StatsAndInventoryWidth = 50;
    public static readonly int LogConsoleHeight = 6;
    public static readonly int ConsoleMinimalSizeX = 155;
    public static readonly int ConsoleMinialSizeY = 35;
    public static readonly char DeadSymbol = '+'; 
    public static readonly char PlayerSymbol = '¶'; // ¶
    public static readonly char WallSymbol = '█';
    public static readonly char FloorSymbol = ' ';
    public static readonly char WeaponSymbol = 'W';
    public static readonly char NonusableItemSymbol = '?';
    public static readonly char CurrencySymbol = '$';
    public static readonly char PotionSymbol = 'P';
    public static readonly char EnemySymbol = 'E';
    public static readonly ConsoleColor DefaulftForegroundColor = ConsoleColor.White;
    public static readonly ConsoleColor PlayerColor = ConsoleColor.Red;
    public static readonly ConsoleColor CurrencyColor = ConsoleColor.Yellow;
    public static readonly ConsoleColor NonusableColor = ConsoleColor.DarkMagenta;
    public static readonly ConsoleColor WeaponColor = ConsoleColor.DarkRed;
    public static readonly ConsoleColor PotionColor = ConsoleColor.Magenta;
    public static readonly ConsoleColor EnemyColor = ConsoleColor.DarkRed;

    public static readonly ConsoleColor WallColor = ConsoleColor.DarkCyan;
    public static readonly ConsoleColor PointingColor = ConsoleColor.Green;


    public static readonly int DefaultPlayerStatHealth = 10;
    public static readonly int DefaultPlayerStatAgression = 4;
    public static readonly int DefaultPlayerStatDexterity = 4;
    public static readonly int DefaultPlayerStatLuck = 4;
    public static readonly int DefaultPlayerStatWisdom = 4;
    public static readonly int DefaultPlayerStatStrength = 4;
    public enum Tile
    {
        Wall,
        Floor
    }
    public enum ItemType
    {
        Weapon,
        NonUsable,
        Currency,
        Potion
    }
    public enum CurrencyType
    {
        Gold,
        Coins
    }

    public enum StatType
    {
        Strength,    // Affects melee attack power
        Dexterity,   // Affects speed and agility
        Health,      // Determines player's HP
        Luck,        // Affects chance-based outcomes
        Aggresion,  // Could influence combat behavior
        Wisdom       // Could affect spellcasting or intelligence
    }
    public enum ItemProperty
    {
        Damage,   // Used for weapons
        Value     // Used for currency
    }
    public enum PointedHand
    {
        Left,
        Right
    }

    // BUILDERS: 

    public static readonly int BuilderMinChamberSize = 3;
    public static readonly int BuilderMaxChamberSize = 5;
    public static readonly int BuilderMinChambersNumber = 5;
    public static readonly int BuilderMaxChambersNumber = 20;

    public static readonly int BuilderMinCentralRoomSize = 8;
    public static readonly int BuilderMaxCentralRoomSize = 12;

    public static readonly int BuilderMinPathLength = 4;
    public static readonly int BuilderMaxPathLength = 12;
    public static readonly int BuilderMinPathsNumber = 5;
    public static readonly int BuilderMaxPathsNumber = 20;


    public static readonly int terminationCounterForItemInicialization = 100;
    public static readonly int MinItemNumber = 5;
    public static readonly int MaxItemNumber = 10;
   
    // enemies const 
    public static readonly int MinEnemyHealth = 5;
    public static readonly int MaxEnemyHealth = 10;
    public static readonly int MinEnemyDamage = 2;
    public static readonly int MaxEnemyDamage = 10;
    public static readonly int MinEnemyArmor = 0;
    public static readonly int MaxEnemyArmor = 5;
    public static readonly int MinEnemyNumber = 10;
    public static readonly int MaxEnemyNumber = 20;

    public static readonly int MaxConsideredSearchDistance = 5;
    public static readonly int MinHealthForAgressiveStrategy = 3; 

    // weapons const 
    public static readonly int MinWeaponDamage = 1;
    public static readonly int MaxWeaponDamage = 10;
    

    public static readonly int MinWeaponDecorators = 1;
    public static readonly int MaxWeaponDecorators = 3;

    // CONNECTION: 

    public static readonly int MaxClients = 9;

    
    public enum PlayerAction
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Attack1,
        Attack2,
        Attack3,
        UsePotion,
        EquipItem,
        ChangeHand,
        ConsumePotion,
        PickUpItems, 
        DropItem,
        DropAllItems, 
        EnterExitInventory,
        Exit  
    }





}
