using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

public class EquipInputHandler : AbstractServerInputHandler
{
    public EquipInputHandler()
    {
    }

    public override bool HandleInput(PlayerAction playerAction, Player player)
    {
        if (playerAction == PlayerAction.EquipItem && !player.IsInInventory)
        {
            player.UnequipItem();
            return false;
        }
        else if (playerAction == PlayerAction.EquipItem)
        {
            player.TryEquipItem();
            return false;
        }
        else
        {
            return base.HandleInput(playerAction, player);
        }
    }
}

