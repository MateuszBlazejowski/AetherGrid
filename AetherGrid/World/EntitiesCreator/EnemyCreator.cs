using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Entities.Strategy;
namespace We_have_doom_at_home.World.EntitiesCreator;

public class EnemyCreator
{
    private string[] names = { "Werewolf", "Gnome", "Manticore" };
    public EnemyCreator() { }
    public Enemy GetEnemy(int x, int y)
    {
        Random random = new Random();
        int nameNo = random.Next(0, names.Count());
        string name = names[nameNo];
        int hp = random.Next(MinEnemyHealth, MaxEnemyHealth + 1);
        int damage = random.Next(MinEnemyDamage, MaxEnemyDamage + 1);
        int armor = random.Next(MinEnemyArmor, MaxEnemyArmor + 1);

        switch (0)
        {
            case 0:
                return new Enemy(x, y, hp, damage, name, armor, new SkittishStrategy());
            case 1:
                return new Enemy(x, y, hp, damage, name, armor, new CalmStrategy());
            case 2:
                return new Enemy(x, y, hp, damage, name, armor, new AggresiveStrategy());
            default:
                throw new Exception("Creating Enemy issue");
        }
    }
}
