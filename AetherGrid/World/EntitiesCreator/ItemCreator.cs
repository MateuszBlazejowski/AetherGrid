using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.Items;
using We_have_doom_at_home.Entities.Interfaces;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.World.EntitiesCreator;

public class ItemCreator
{
    private string[] names = {"Stone", "Totem", "String" };
    public IItem GetItem(int x, int y)
    {
        Random random = new Random();
        int currencyOrItem = random.Next(0, 2);

        if (currencyOrItem == 0)
        {
            currencyOrItem = random.Next(0, 2);
            if (currencyOrItem == 0)
                return new BaseCurrency("Gold", "", x, y, CurrencyType.Gold);
            else
                return new BaseCurrency("Coin", "", x, y, CurrencyType.Coins);
        }

        int itemNo = random.Next(0, names.Count());
        string name = names[itemNo];
        return new BaseNonUsable(name, "", x, y, new Dictionary<ItemProperty, int>{ }, new Dictionary<StatType, int> { }) ;

    }
}
