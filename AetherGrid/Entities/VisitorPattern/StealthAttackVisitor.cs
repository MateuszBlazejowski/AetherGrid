using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Weapons;
using We_have_doom_at_home.Entities.Interfaces;


namespace We_have_doom_at_home.Entities.VisitorPattern;


public class StealthAttackVisitor : IVisitor
{
    public int Visit(HeavyWeapon weapon, Player player) => (player.Strength + player.Aggresion) / 2;
    public int Visit(LightWeapon weapon, Player player) => (player.Dexterity + player.Luck) * 2;
    public int Visit(MagicWeapon weapon, Player player) => 1;
    public int Visit(IEquippable item, Player player) => 0;

    public int VisitDefense(HeavyWeapon weapon, Player player) => player.Strength;
    public int VisitDefense(LightWeapon weapon, Player player) => player.Dexterity;
    public int VisitDefense(MagicWeapon weapon, Player player) => 0;
    public int VisitDefense(IEquippable item, Player player) => 0;
}

