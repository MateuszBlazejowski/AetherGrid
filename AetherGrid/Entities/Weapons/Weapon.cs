using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Weapons;

using We_have_doom_at_home.Entities.VisitorPattern;

public abstract class Weapon : IEquippable
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public ItemType Type { get; protected set; } = ItemType.Weapon; // Common to all weapons
    public bool IsEquippable { get; protected set; } = true; // Common to all weapons
    public bool IsTwoHanded { get; set; } // Can be overridden
    public int PosX { get; set; }
    public int PosY { get; set; }

    protected Dictionary<ItemProperty, int> _itemProperties;
    protected Dictionary<StatType, int> _statModifiers;

    // Constructor now includes x and y for position
    public Weapon(string name, string description, int posX, int posY, Dictionary<ItemProperty, int> itemProperties, Dictionary<StatType, int> statModifiers)
    {
        Name = name;
        Description = description;
        PosX = posX;
        PosY = posY;
        _itemProperties = itemProperties;
        _statModifiers = statModifiers;
    }

    public virtual Dictionary<ItemProperty, int> GetItemProperties()
    {
        return new Dictionary<ItemProperty, int>(_itemProperties);
    }

    public virtual Dictionary<StatType, int> GetStatModifiers()
    {
        return new Dictionary<StatType, int>(_statModifiers);
    }

    public void PickMeUp(Player player)
    {
        player.Inventory.Add(this);
    }

    // Abstract methods to be implemented by subclasses
    public abstract int Accept(IVisitor visitor, Player player);
    public abstract int AcceptDefense(IVisitor visitor, Player player);

    public override string ToString()
    {
        return $"{Description} {Name}";
    }
}
