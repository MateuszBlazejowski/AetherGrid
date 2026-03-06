using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Items;
using We_have_doom_at_home.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
using System.Numerics;
using We_have_doom_at_home.CoreLogic;
using We_have_doom_at_home.MVC.ServerMVC; 

namespace We_have_doom_at_home.Entities;

public class Player
{
    public int PosX;
    public int PosY;
    public int PlayerID; 
    public bool IsInInventory;
    public int InventoryScrollOffset;
    public bool IsAlive; 

    // attributes: 
    public int Strength;
    public int Dexterity;
    public int Health;
    public int Luck;
    public int Aggresion;
    public int Wisdom;

    // Inventory as a list of items and two hands just as items 
    public List<IEquippable> Inventory;
    public IEquippable? LeftHand;
    public IEquippable? RightHand;
    public int InventoryIndex; // Pointer to selected inventory item
    public PointedHand HandInUse;

    public List<ICurrency> Poach;
    public List<BasePotion> WorkingPotions;

    ServerModel ServerModel;
    public Player(int posX, int posY, ServerModel ServerModel, int playerID) // starting positions
    {
        PosX = posX;
        PosY = posY;
        Inventory = new List<IEquippable>();
        Poach = new List<ICurrency>();
        InventoryScrollOffset = 0;
        LeftHand = null;
        RightHand = null;
        IsInInventory = false;
        InventoryIndex = 0;
        HandInUse = PointedHand.Right;
        IsAlive = true; 


        Strength = DefaultPlayerStatStrength;
        Dexterity = DefaultPlayerStatDexterity;
        Health = DefaultPlayerStatHealth;
        Luck = DefaultPlayerStatLuck;
        Aggresion = DefaultPlayerStatAgression;
        Wisdom = DefaultPlayerStatWisdom;
        WorkingPotions = new List<BasePotion>();

        this.ServerModel = ServerModel;
        PlayerID = playerID;
    }
    public void TryWalk(int deltaX, int deltaY, Map map)
    {
        Logs.Add($"Invoked TryWalk({deltaX} {deltaY})");
        if (!IsWalkable(PosX + deltaX, PosY + deltaY, map)) return;
        PosX += deltaX;
        PosY += deltaY;
    }
    public void TryWalkAbsolute(int x, int y, Map map)
    {
        Logs.Add($"Invoked TryWalkAbsolute({x} {y})");
        if (!IsWalkable(x, y, map)) return;
        PosX = x;
        PosY = y;
    }
    public bool IsWalkable(int x, int y, Map map)
    {
        if (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight) return false;
        if (map.tileMap[x, y] == Tile.Wall) return false;
        return true;
    }
    public List<IItem> GetItemsOnCurrentTile(Map map)
    {
        return map.items.Where(item => item.PosX == PosX && item.PosY == PosY).ToList();
    }
    public void IterateInventory(int deltaItem)
    {
        if (Inventory.Count == 0) return; // No items to iterate

        InventoryIndex += deltaItem;

        // Wrap around the inventory
        if (InventoryIndex < 0) InventoryIndex = Inventory.Count - 1;
        if (InventoryIndex >= Inventory.Count) InventoryIndex = 0;
    }

    public void EnterInventory()
    {
        Logs.Add("Entered Inventory");
        IsInInventory = true;
        InventoryIndex = 0;
    }
    public void ExitInventory()
    {
        Logs.Add("Exited Inventory");
        IsInInventory = false;
    }
    public void ChangeHand()
    {
        Logs.Add($"Changed hands");
        if (HandInUse == PointedHand.Left)
            HandInUse = PointedHand.Right;
        else
            HandInUse = PointedHand.Left;
    }


    public void TryEquipItem()
    {
        Logs.Add($"Invoked TryEquipItem ()");
        if (!Inventory.Any()) return;
        IEquippable selectedItem = Inventory[InventoryIndex];

        if (selectedItem.IsTwoHanded)
        {
            UnequipItem(true);  // Unequip Left Hand
            UnequipItem(false);

            LeftHand = RightHand = selectedItem;
        }
        else
        {
            if (HandInUse == PointedHand.Left)
            {
                UnequipItem(true); // Unequip left hand
                LeftHand = selectedItem;
            }
            else
            {
                UnequipItem(false); // Unequip right hand
                RightHand = selectedItem;
            }
        }

        Inventory.RemoveAt(InventoryIndex);
        InventoryIndex = Math.Max(0, InventoryIndex - 1);

        ApplyItemEffects(selectedItem);
    }

    public void UnequipItem()
    {
        if (HandInUse == PointedHand.Left)
            UnequipItem(true); // unequip left
        else
            UnequipItem(false);
    }
    public void UnequipItem(bool unequipLeftHand)
    {
        IEquippable? itemToUnequip = unequipLeftHand ? LeftHand : RightHand;
        if (itemToUnequip == null) return;
        Logs.Add($"Invoked UnequipItem ({itemToUnequip.Name})");

        if (itemToUnequip.IsTwoHanded == true)
        {
            LeftHand = RightHand = null;
        }

        Inventory.Add(itemToUnequip);


        if (unequipLeftHand)
            LeftHand = null;
        else
            RightHand = null;

        TakeDownItemEffects(itemToUnequip);
    }

    public void TryPickupItem(Map map)
    {
        Logs.Add($"Invoked TryPickupItem()");
        List<IItem> itemsOnCurrentTile = GetItemsOnCurrentTile(map);
        if (!itemsOnCurrentTile.Any()) return;
        foreach (var item in itemsOnCurrentTile)
        {
            item.PickMeUp(this);
        }
        map.items.RemoveAll(item => item.PosX == PosX && item.PosY == PosY);
    }
    public void DropAllItems(Map map)
    {
        DropItemFromHand(map, true);
        DropItemFromHand(map, false);
        int inventoryCount = Inventory.Count;
        for (int i = 0; i < inventoryCount; i++)
        {
            DropItemFromInventory(map);
        }

    }
    public void DropItemFromInventory(Map map)
    {
        Logs.Add("Invoked DropItemFromInventory()");
        if (!Inventory.Any()) return;

        IItem itemToDrop = Inventory[InventoryIndex];

        itemToDrop.PosX = PosX;
        itemToDrop.PosY = PosY;
        map.items.Add(itemToDrop);
        Inventory.RemoveAt(InventoryIndex);

        if (Inventory.Count > 0)
            InventoryIndex = Math.Min(InventoryIndex, Inventory.Count - 1);
        else
            InventoryIndex = 0;
    }


    public void DropItemFromHand(Map map)
    {
        Logs.Add($"Invoked DropItemFromHand()");
        if (HandInUse == PointedHand.Left)
            DropItemFromHand(map, true); // drop left
        else
            DropItemFromHand(map, false);

    }

    public void DropItemFromHand(Map map, bool dropLeftHand)
    {
        IEquippable? itemToDrop = dropLeftHand ? LeftHand : RightHand;
        if (itemToDrop == null) return;

        if (itemToDrop.IsTwoHanded == true)
        {
            LeftHand = RightHand = null;

        }

        itemToDrop.PosX = PosX;
        itemToDrop.PosY = PosY;
        map.items.Add(itemToDrop);

        if (dropLeftHand)
            LeftHand = null;
        else
            RightHand = null;

        TakeDownItemEffects(itemToDrop);
    }

    public void ApplyItemEffects(IEquippable item)
    {
        foreach (var effect in item.GetStatModifiers())
        {
            ApplyStatEffect(effect.Key, effect.Value);
        }
    }

    public void TakeDownItemEffects(IEquippable item)
    {
        foreach (var effect in item.GetStatModifiers())
        {
            TakeDownStatEffect(effect.Key, effect.Value);
        }
    }

    // Helper Method: Modifies the correct stat
    private void ApplyStatEffect(StatType stat, int value)
    {
        switch (stat)
        {
            case StatType.Strength: Strength += value; break;
            case StatType.Dexterity: Dexterity += value; break;
            case StatType.Health: Health += value; break;
            case StatType.Luck: Luck += value; break;
            case StatType.Aggresion: Aggresion += value; break;
            case StatType.Wisdom: Wisdom += value; break;
        }
    }

    private void TakeDownStatEffect(StatType stat, int value)
    {
        switch (stat)
        {
            case StatType.Strength: Strength -= value; break;
            case StatType.Dexterity: Dexterity -= value; break;
            case StatType.Health: Health -= value; break;
            case StatType.Luck: Luck -= value; break;
            case StatType.Aggresion: Aggresion -= value; break;
            case StatType.Wisdom: Wisdom -= value; break;
        }
    }
    public void TryConsumePotionFromInventory()
    {
        Logs.Add("TryConsumePotionFromInventory Invoked");
        if (!IsInInventory)
        {
            Logs.Add("Enter inventory to consume a potion");
            return;
        }
        if (Inventory.Count == 0) return;
        if (Inventory[InventoryIndex].Type == ItemType.Potion)
        {
            BasePotion itemToDrink = Inventory[InventoryIndex] as BasePotion; // checked above 
            WorkingPotions.Add(itemToDrink);
            Inventory.RemoveAt(InventoryIndex);
            this.ServerModel.AttachPotion(itemToDrink);
            Logs.Add("You drank a potion");
            if (Inventory.Count > 0)
                InventoryIndex = Math.Min(InventoryIndex, Inventory.Count - 1);
            else
                InventoryIndex = 0;
        }
        else
            Logs.Add("You are not pointing at a potion");

    }
    public void TryConsumePotionFromHand()
    {
        Logs.Add("TryConsumePotionFromHand Invoked");
        if (HandInUse == PointedHand.Left)
        {
            if (LeftHand == null) return;
            if (LeftHand.Type == ItemType.Potion)
            {
                BasePotion itemToUnequip = LeftHand as BasePotion; // checked above 
                WorkingPotions.Add(itemToUnequip);
                TakeDownItemEffects(itemToUnequip);
                this.ServerModel.AttachPotion(itemToUnequip);
                LeftHand = null;
            }
        }
        else
        {
            if (RightHand == null) return;
            if (RightHand.Type == ItemType.Potion)
            {
                BasePotion itemToUnequip = RightHand as BasePotion; // checked above 
                WorkingPotions.Add(itemToUnequip);
                TakeDownItemEffects(itemToUnequip);
                this.ServerModel.AttachPotion(itemToUnequip);
                RightHand = null;
            }
        }
    }

    public void UpdateActivePotionList()
    {
        foreach (var potion in WorkingPotions.ToList())
        {
            if (potion.TurnsPassed >= potion.PotionDuration)
            {
                this.ServerModel.DetachPotion(potion);
                WorkingPotions.Remove(potion);
            }
        }
    }
    public void ApplyPotionsEffects()
    {
        foreach (var potion in WorkingPotions)
        {
            ApplyPotionEffects(potion);
        }
    }
    public void TakeDownPotionsEffects()
    {
        foreach (var potion in WorkingPotions)
        {
            TakeDownPotionEffects(potion);
        }
    }
    private void ApplyPotionEffects(BasePotion potion)
    {
        foreach (var effect in potion.GetPotionEffects())
        {
            ApplyStatEffect(effect.Key, effect.Value);
        }
    }

    private void TakeDownPotionEffects(BasePotion potion)
    {
        foreach (var effect in potion.GetPotionEffects())
        {
            TakeDownStatEffect(effect.Key, effect.Value);
        }
    }

    public void SubstractHealthPoints(int damage)
    {
        if (this.Health - damage < 0)
        {
            this.Health = 0;
        }
        else
        {
            this.Health -= damage;
        }
    }

}
