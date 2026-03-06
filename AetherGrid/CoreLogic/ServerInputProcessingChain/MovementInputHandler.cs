using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

public class MovementInputHandler : AbstractServerInputHandler
{
    private Map map;

    public MovementInputHandler(Map _map)
    {
        map = _map;
    }

    public override bool HandleInput(PlayerAction playerAction, Player player)
    {
        if (playerAction == PlayerAction.MoveUp && !player.IsInInventory)
        {
            player.TryWalk(0, -1, map);
            return true;
        }
        else if (playerAction == PlayerAction.MoveDown && !player.IsInInventory)
        {
            player.TryWalk(0, 1, map);
            return true;
        }
        else if (playerAction == PlayerAction.MoveLeft && !player.IsInInventory)
        {
            player.TryWalk(-1, 0, map);
            return true;
        }
        else if (playerAction == PlayerAction.MoveRight && !player.IsInInventory)
        {
            player.TryWalk(1, 0, map);
            return true;
        }
        else
        {
            return base.HandleInput(playerAction, player);
        }
    }
}

