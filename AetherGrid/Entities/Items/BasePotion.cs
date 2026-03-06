using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.Entities.ObserverPattern;
using System.Data;

namespace We_have_doom_at_home.Entities.Items;

public class BasePotion : IEquippable, IPotionObserver 
{
    public string Name { get; }
    public string Description { get; }
    public ItemType Type { get; }
    public bool IsEquippable { get; }
    public bool IsTwoHanded { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; } 

    public bool IsAntitode { get; set; } 
    public int PotionDuration { get; set; }

    public int TurnsPassed { get; set; } = 0;

    private bool IsEternal { get; set; }

    private int Multiplier;

    public void Update()
    {
        if (!IsEternal)
        { 
            TurnsPassed++;
        }
    }
    //
    private Dictionary<ItemProperty, int> _itemProperties;
    private Dictionary<StatType, int> _statModifiers; // Holds standard player stat changes (applied by keeping in hand)
    private Dictionary<StatType, int> _potionEffects; // Holds player stat changes afetr consuming potions

    public BasePotion(
    string name, string description,
    int posX, int posY,
    Dictionary<StatType, int> statModifiers, Dictionary<StatType, int> potionEffects, int potionDuration, bool isEternal,int multipier, bool isAntidote)
    {
        Name = name;
        Description = description;
        Type = ItemType.Potion;
        IsEquippable = true; // always like that
        IsTwoHanded = false; // set by a decorator if wanted
        PosX = posX;
        PosY = posY;
        _itemProperties = new Dictionary<ItemProperty, int> { };
        _statModifiers = statModifiers;
        _potionEffects = potionEffects; 
        PotionDuration = potionDuration;    
        IsEternal = isEternal;
        Multiplier = multipier; 
        IsAntitode = isAntidote;
    }

    public virtual Dictionary<ItemProperty, int> GetItemProperties()
    {
        return new Dictionary<ItemProperty, int>(_itemProperties);
    }

    public virtual Dictionary<StatType, int> GetStatModifiers()
    {
        return new Dictionary<StatType, int>(_statModifiers);
    }
    public virtual Dictionary<StatType, int> GetPotionEffects()
    {
        var toReturn =  new Dictionary<StatType, int>();
        foreach (var item in _potionEffects)
        {
            toReturn.Add(item.Key, item.Value * Multiplier * (TurnsPassed + 1)); 
        }
        return toReturn;
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
