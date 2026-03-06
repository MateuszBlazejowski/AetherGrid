using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

public class DropItemInputHandler : AbstractServerInputHandler
{
    private Map map;

    public DropItemInputHandler(Map _map)
    {
        map = _map;
    }

    public override bool HandleInput(PlayerAction playerAction, Player player)
    {
        if (playerAction == PlayerAction.DropItem && player.IsInInventory)
        {
            player.DropItemFromInventory(map);
            return false;
        }
        else if (playerAction == PlayerAction.DropItem && !player.IsInInventory)
        {
            player.DropItemFromHand(map);
            return false;
        }
        else if (playerAction == PlayerAction.DropAllItems)
        {
            player.DropAllItems(map);
            return false;
        }
        else
        {
            return base.HandleInput(playerAction, player);
        }
    }
}

