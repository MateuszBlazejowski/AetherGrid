using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Weapons;

namespace We_have_doom_at_home.Entities.VisitorPattern;

public interface IVisitor
{
    int Visit(HeavyWeapon weapon, Player player);
    int Visit(LightWeapon weapon, Player player);
    int Visit(MagicWeapon weapon, Player player);
    int Visit(IEquippable item, Player player);

    int VisitDefense(HeavyWeapon weapon, Player player);
    int VisitDefense(LightWeapon weapon, Player player);
    int VisitDefense(MagicWeapon weapon, Player player);
    int VisitDefense(IEquippable item, Player player);
}
