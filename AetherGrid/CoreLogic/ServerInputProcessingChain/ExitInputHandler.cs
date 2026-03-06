using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

public class ExitInputHandler : AbstractServerInputHandler
{
    public ExitInputHandler()
    {
    }

    public override bool HandleInput(PlayerAction playerAction, Player player)
    {
        if (playerAction == PlayerAction.Exit)
        {
            // write sth like disconnecting the client or sth like that 
            return false;
        }
        else
        {
            return base.HandleInput(playerAction, player);
        }
    }
}
