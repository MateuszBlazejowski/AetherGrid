using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Decorators;

public class TwoHanded : ItemDecorator
{
    public TwoHanded(IEquippable item)
       : base(item,
           new Dictionary<ItemProperty, int>(), // No change to item properties
           new Dictionary<StatType, int>()) // No change
    {
        item.IsTwoHanded = true;
    }

    public override string Name => $"{_item.Name} (Two Handed)";

    public override string ToString()
    {
        return $"{_item.ToString()} (Two Handed)";
    }
}


