using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Items;
using We_have_doom_at_home.Entities.Weapons;
using static We_have_doom_at_home.Technical.Common;
namespace We_have_doom_at_home.World.EntitiesCreator;

public class WeaponCreator
{
    private string[] names = { "Sword", "Mace", "Spear" };

    public IEquippable GetWeapon(int x, int y)
    {
        Random random = new Random();
        int weaponNo = random.Next(0, names.Length);
        string name = names[weaponNo];

        // Randomly select the weapon type (Heavy, Light, Magic)
        int weaponType = random.Next(0, 3);  // 0 = Heavy, 1 = Light, 2 = Magic

        IEquippable weapon;
        int damage = random.Next(MinWeaponDamage, MaxWeaponDamage + 1);

        if (weaponType == 0)
            weapon = new HeavyWeapon(name, "Heavy", x, y, new Dictionary<ItemProperty, int> { { ItemProperty.Damage, damage } }, new Dictionary<StatType, int> { });
        else if (weaponType == 1)
            weapon = new LightWeapon(name, "Light", x, y, new Dictionary<ItemProperty, int> { { ItemProperty.Damage, damage } }, new Dictionary<StatType, int> { });
        else
            weapon = new MagicWeapon(name, "Magic", x, y, new Dictionary<ItemProperty, int> { { ItemProperty.Damage, damage } }, new Dictionary<StatType, int> { });

        return weapon;
    }
}

