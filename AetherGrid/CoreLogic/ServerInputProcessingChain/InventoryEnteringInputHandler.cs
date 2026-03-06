using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Technical;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;

namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

internal class InventoryEnteringInputHandler : AbstractServerInputHandler
{
    public InventoryEnteringInputHandler()
    {
    }

    public override bool HandleInput(PlayerAction playerAction, Player player)
    {
        if (playerAction == PlayerAction.EnterExitInventory && !player.IsInInventory)
        {
            player.EnterInventory();
            return false;
        }
        else if (playerAction == PlayerAction.EnterExitInventory)
        {
            player.ExitInventory();
            return false;
        }
        else
        {
            return base.HandleInput(playerAction, player);
        }
    }
}

