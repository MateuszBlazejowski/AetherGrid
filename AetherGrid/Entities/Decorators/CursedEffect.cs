using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Decorators;

public class CursedEffect : ItemDecorator
{
    public CursedEffect(IEquippable item)
        : base(item,
            new Dictionary<ItemProperty, int> { { ItemProperty.Damage, -3 } },
            new Dictionary<StatType, int>
            {
                { StatType.Strength, -5 },
                { StatType.Health, -7 },
                { StatType.Luck, -5 }
            })
    {
    }

    public override string Name => $"{_item.Name} (Cursed)";

    public override string ToString()
    {
        return $"{_item.ToString()} (Cursed)";
    }
}

