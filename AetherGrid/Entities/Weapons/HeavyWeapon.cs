using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Weapons;
using We_have_doom_at_home.Entities.VisitorPattern;

public class HeavyWeapon : Weapon
{
    // Constructor now includes posX and posY
    public HeavyWeapon(string name, string description, int posX, int posY, Dictionary<ItemProperty, int> itemProperties, Dictionary<StatType, int> statModifiers)
        : base(name, description, posX, posY, itemProperties, statModifiers)
    {
    }

    public override int Accept(IVisitor visitor, Player player)
    {
        return visitor.Visit(this, player); // Calls the appropriate method in the visitor for attack
    }

    public override int AcceptDefense(IVisitor visitor, Player player)
    {
        return visitor.VisitDefense(this, player); // Calls the appropriate method in the visitor for defense
    }
}