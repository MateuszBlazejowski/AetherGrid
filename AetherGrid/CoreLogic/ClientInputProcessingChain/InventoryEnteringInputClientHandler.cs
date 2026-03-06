using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Connection;
using We_have_doom_at_home.CoreLogic.InputProcessingChain;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Log;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.CoreLogic.ClientInputProcessingChain;

public class InventoryEnteringInputClientHandler : AbstractClientInputHandler
{
    public override void HandleInput(ConsoleKey key, ClientConnection Connection)
    {
        if (key == ConsoleKey.Q)
        {
            Connection.Send(PlayerAction.EnterExitInventory);
        }
        else
        {
            base.HandleInput(key, Connection);
        }
    }
}