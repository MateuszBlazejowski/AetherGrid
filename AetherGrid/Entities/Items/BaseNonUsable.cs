using We_have_doom_at_home.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Items;

public class BaseNonUsable : IEquippable
{
    public string Name { get; }
    public string Description { get; }
    public ItemType Type { get; }
    public bool IsEquippable { get; }
    public bool IsTwoHanded { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }

    private Dictionary<ItemProperty, int> _itemProperties; // Holds weapon damage, armor value, etc.
    private Dictionary<StatType, int> _statModifiers; // Holds player stat changes

    public BaseNonUsable(
        string name, string description,
        int posX, int posY,
        Dictionary<ItemProperty, int> itemProperties,
        Dictionary<StatType, int> statModifiers)
    {
        Name = name;
        Description = description;
        Type = ItemType.NonUsable;
        IsEquippable = true; // always like that
        IsTwoHanded = false; // set by a decorator if wanted
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
    public override string ToString()
    {
        return $"{Name}";
    }
}
