using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.World;

namespace We_have_doom_at_home.Entities.Strategy;

public class CalmStrategy : IStrategy
{
    public void Behave(Map map, List<Player> players, Enemy enemy)
    {
        // doing absoulutely nothing  
    }
}
