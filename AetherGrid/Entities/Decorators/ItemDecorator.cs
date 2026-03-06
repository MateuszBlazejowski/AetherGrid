using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Decorators;

using We_have_doom_at_home.Entities.VisitorPattern;

public abstract class ItemDecorator : IEquippable
{
    protected IEquippable _item;
    protected Dictionary<ItemProperty, int> _propertyModifiers;
    protected Dictionary<StatType, int> _statModifiers;

    public ItemDecorator(IEquippable item, Dictionary<ItemProperty, int> propertyModifiers, Dictionary<StatType, int> statModifiers)
    {
        _item = item;
        _propertyModifiers = propertyModifiers;
        _statModifiers = statModifiers;
    }

    public virtual string Name => $"{_item.Name} ({string.Join(", ", _statModifiers.Keys.Select(k => k.ToString()).Concat(_propertyModifiers.Keys.Select(k => k.ToString())))})";
    public virtual string Description => _item.Description;
    public virtual ItemType Type => _item.Type;

    public virtual bool IsTwoHanded
    {
        get => _item.IsTwoHanded;
        set => _item.IsTwoHanded = value;
    }

    public virtual int PosX
    {
        get => _item.PosX;
        set => _item.PosX = value;
    }

    public virtual int PosY
    {
        get => _item.PosY;
        set => _item.PosY = value;
    }

    public virtual Dictionary<ItemProperty, int> GetItemProperties()
    {
        var baseProperties = _item.GetItemProperties();
        foreach (var mod in _propertyModifiers)
        {
            if (baseProperties.ContainsKey(mod.Key))
                baseProperties[mod.Key] += mod.Value;
            else
                baseProperties[mod.Key] = mod.Value;
        }
        return baseProperties;
    }

    public virtual Dictionary<StatType, int> GetStatModifiers()
    {
        var baseModifiers = _item.GetStatModifiers();
        foreach (var mod in _statModifiers)
        {
            if (baseModifiers.ContainsKey(mod.Key))
                baseModifiers[mod.Key] += mod.Value;
            else
                baseModifiers[mod.Key] = mod.Value;
        }
        return baseModifiers;
    }

    public void PickMeUp(Player player)
    {
        player.Inventory.Add(this);
    }

    public override string ToString()
    {
        return $"{_item.ToString()}";
    }

    // Delegate Accept method to the wrapped item (weapon or non-weapon)
    public virtual int Accept(IVisitor visitor, Player player)
    {
        return _item.Accept(visitor, player);
    }

    // Delegate AcceptDefense method to the wrapped item (weapon or non-weapon)
    public virtual int AcceptDefense(IVisitor visitor, Player player)
    {
        return _item.AcceptDefense(visitor, player);
    }
}



