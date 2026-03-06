using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Decorators;

public class LuckyEffect : ItemDecorator
{
    public LuckyEffect(IEquippable item, int extraLuck)
        : base(item,
            new Dictionary<ItemProperty, int>(), // No change to item properties
            new Dictionary<StatType, int> { { StatType.Luck, extraLuck } }) // Increase player Luck
    {
    }

    public override string Name => $"{_item.Name} (Lucky)";

    public override string ToString()
    {
        return $"{_item.ToString()} (Lucky)";
    }
}



