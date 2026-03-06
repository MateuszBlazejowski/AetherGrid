using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace We_have_doom_at_home.World.Builder;

public class Director
{
    public IBuilder builder { get; set; }

    public Director(IBuilder _builder)
    {
        builder = _builder;
    }

    public void BuildInstruction1()
    {
        builder.filledDungeon()
        .addCentralChamber()
        .addChambers()
        .addPaths()
        .addWeapons()
        .addEnemies()
        .addPotions()
        .addItems()
        .addModifiedWeapons();
    }
    public void BuildInstruction2()
    {
        builder.emptyDungeon();
    }
}
