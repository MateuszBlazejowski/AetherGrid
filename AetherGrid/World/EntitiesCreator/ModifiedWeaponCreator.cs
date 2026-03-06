using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Entities.Decorators;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Items;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.World.EntitiesCreator;

public class ModifiedWeaponCreator
{
    private WeaponCreator _weaponCreator;

    public ModifiedWeaponCreator()
    {
        _weaponCreator = new WeaponCreator(); // Instantiate the WeaponCreator
    }

    public IItem GetModifiedWeapon(int x, int y)
    {
        Random random = new Random();
        var weapon = _weaponCreator.GetWeapon(x, y); // Get a base weapon with position (x, y)

        int decoratorsNo = random.Next(MinWeaponDecorators, MaxWeaponDecorators + 1); // Random number of decorators to apply

        IEquippable product = weapon; // Start with the base weapon
        for (int i = 0; i < decoratorsNo; i++)
        {
            int decoratorType = random.Next(0, 5); // Randomly select a decorator
            switch (decoratorType)
            {
                case 0:
                    product = new LuckyEffect((IEquippable)product, 5);  // Apply Lucky effect
                    break;
                case 1:
                    product = new CursedEffect((IEquippable)product);   // Apply Cursed effect
                    break;
                case 2:
                    product = new PowerfulEffect((IEquippable)product, 5);  // Apply Powerful effect
                    break;
                case 3:
                    product = new ProtectiveEffect((IEquippable)product, 10);  // Apply Protective effect
                    break;
                case 4:
                    product = new TwoHanded((IEquippable)product);  // Mark as Two-Handed
                    break;
                default:
                    break;
            }
        }
        return product; // Return the modified weapon
    }
}

