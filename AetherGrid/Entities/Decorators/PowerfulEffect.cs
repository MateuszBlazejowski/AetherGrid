using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Decorators;

public class PowerfulEffect : ItemDecorator
{
    public PowerfulEffect(IEquippable item, int extraDamage)
        : base(item,
            new Dictionary<ItemProperty, int> { { ItemProperty.Damage, extraDamage } }, // Increase weapon damage
            new Dictionary<StatType, int>()) // No change to player stats   
    {
    }

    public override string Name => $"{_item.Name} (Powerful)";

    public override string ToString()
    {
        return $"{_item.ToString()} (Powerful)";
    }
}




