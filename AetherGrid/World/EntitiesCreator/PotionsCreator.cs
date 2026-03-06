using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Entities.Items;
using static We_have_doom_at_home.Technical.Common;
namespace We_have_doom_at_home.World.EntitiesCreator;

public class PotionsCreator
{
    private string[] names = { "Mamma's soup", "Wisdom Potion", "Sussy water", "Antidote" };
    private List<Dictionary<StatType, int>> stats;
    public PotionsCreator()
    {
        stats = new List<Dictionary<StatType, int>>()
        {
            new Dictionary<StatType, int> { { StatType.Strength, 10 } },
            new Dictionary<StatType, int> { { StatType.Wisdom, 5 } },
            new Dictionary<StatType, int> { { StatType.Strength, -5 } }, 
            new Dictionary<StatType, int> ()
        };
    }
    public BasePotion GetPotion(int x, int y)
    {
        Random random = new Random();
        int potionNo = random.Next(0, names.Count());
        string name = names[potionNo];
        int potionDuration = random.Next(3, 15);
        int multiplier = random.Next(1, 4);
        bool isEternal = false;
        bool isAntidote = false;
        if(potionNo == 2)
            isEternal =true;
        if (potionNo == 3)
            isAntidote = true;
        return new BasePotion(name, "", x, y, new Dictionary<StatType, int> { }, stats[potionNo], potionDuration, isEternal, multiplier, isAntidote);

    }
}
