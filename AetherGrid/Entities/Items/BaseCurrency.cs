using We_have_doom_at_home.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Items;

public class BaseCurrency : ICurrency
{
    public string Name { get; }
    public string Description { get; }
    public ItemType Type { get; }
    public CurrencyType _CurrencyType { get; }
    public int PosX { get; set; }
    public int PosY { get; set; }


    public BaseCurrency(
        string name, string description,
        int posX, int posY, CurrencyType currencyType)
    {
        Name = name;
        Description = description;
        Type = ItemType.Currency;
        _CurrencyType = currencyType;
        PosX = posX;
        PosY = posY;
    }
    public void PickMeUp(Player player)
    {
        player.Poach.Add(this);
    }
    public override string ToString()
    {
        return $"{Name}";
    }
}
