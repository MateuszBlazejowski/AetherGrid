using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Weapons;
using We_have_doom_at_home.Entities.VisitorPattern;
public interface IEquippable : IItem
{
    bool IsTwoHanded { get; set; }
    Dictionary<ItemProperty, int> GetItemProperties();

    Dictionary<StatType, int> GetStatModifiers(); // Returns all affected stats

    int Accept(IVisitor visitor, Player player) 
    {
        return visitor.Visit(this, player);
    }
    int AcceptDefense(IVisitor visitor, Player player)
    {
        return visitor.Visit(this, player);
    }
}
