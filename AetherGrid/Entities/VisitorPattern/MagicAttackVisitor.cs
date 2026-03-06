using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Weapons;

namespace We_have_doom_at_home.Entities.VisitorPattern;

public class MagicAttackVisitor : IVisitor
{
    public int Visit(HeavyWeapon weapon, Player player) => 1;
    public int Visit(LightWeapon weapon, Player player) => 1;
    public int Visit(MagicWeapon weapon, Player player) => player.Wisdom * 2;
    public int Visit(IEquippable item, Player player) => 0;

    public int VisitDefense(HeavyWeapon weapon, Player player) => player.Luck;
    public int VisitDefense(LightWeapon weapon, Player player) => player.Luck;
    public int VisitDefense(MagicWeapon weapon, Player player) => player.Wisdom * 2;
    public int VisitDefense(IEquippable item, Player player) => player.Luck;
}
