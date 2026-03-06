using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

public class ConsumePotionInputHandler : AbstractServerInputHandler
{
    public ConsumePotionInputHandler()
    {
    }

    public override bool HandleInput(PlayerAction playerAction, Player player)
    {
        if (playerAction == PlayerAction.ConsumePotion && player.IsInInventory)
        {
            player.TryConsumePotionFromInventory();
            return true;
        }
        else if (playerAction == PlayerAction.ConsumePotion)
        {
            player.TryConsumePotionFromHand();
            return true;
        }
        else
        {
            return base.HandleInput(playerAction, player);
        }
    }
}

