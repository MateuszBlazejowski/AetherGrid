using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.CoreLogic.InputProcessingChain;

namespace We_have_doom_at_home.World.Builder;

public interface IBuilder
{
    public IBuilder emptyDungeon();
    public IBuilder filledDungeon();
    public IBuilder addPaths();
    public IBuilder addChambers();
    public IBuilder addCentralChamber();
    public IBuilder addWeapons();
    public IBuilder addModifiedWeapons();
    public IBuilder addItems();
    public IBuilder addPotions();

    public IBuilder addEnemies();

    public Map? GetMap() { return null; }
    public IServerInputHandler? GetInputHandlerChain() { return null; }
}
