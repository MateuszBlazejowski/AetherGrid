using We_have_doom_at_home.Technical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Interfaces;

public interface IItem
{
    string Name { get; }
    string Description { get; }
    ItemType Type { get; }
    int PosX { get; set; }
    int PosY { get; set; }

    void PickMeUp(Player player);
    string ToString();
}
