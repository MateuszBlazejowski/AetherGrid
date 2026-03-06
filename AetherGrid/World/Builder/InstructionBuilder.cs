using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace We_have_doom_at_home.World.Builder;

public class InstructionBuilder : IBuilder
{
    public IBuilder emptyDungeon() 
    {
        return this;
    }
    public IBuilder filledDungeon() 
    {
        return this;
    }

    public IBuilder addPaths() 
    {
        return this;
    }
    public IBuilder addChambers() 
    {
        return this;
    }

    public IBuilder addCentralChamber() 
    {
        return this;
    }
    public IBuilder addWeapons()
    {
        ExistItems = true;
        return this;
    }

    public IBuilder addModifiedWeapons()
    {
        ExistItems = true;
        return this;
    }

    public IBuilder addItems()
    {
        ExistItems = true;
        return this;
    }

    public IBuilder addPotions()
    {
        ExistItems = true;
        ExistPotions = true;
        return this;
    }

    public IBuilder addEnemies()
    {
        ExistEnemies = true;
        return this;
    }

    public bool ExistItems { get; set; } = false;
    public bool ExistEnemies { get; set; } = false;
    public bool ExistPotions { get; set; } = false;
}
