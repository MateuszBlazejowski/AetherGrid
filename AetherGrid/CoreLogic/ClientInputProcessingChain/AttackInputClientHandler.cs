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

public class AttackInputClientHandler : AbstractClientInputHandler
{
    public override void HandleInput(ConsoleKey key, ClientConnection Connection)
    {
        if (key == ConsoleKey.D1)
        {
            Connection.Send(PlayerAction.Attack1);
        }
        else if (key == ConsoleKey.D2)
        {
            Connection.Send(PlayerAction.Attack2);
        }
        else if (key == ConsoleKey.D3)
        {
            Connection.Send(PlayerAction.Attack3);
        }
        else
        {
            base.HandleInput(key, Connection);
        }
    }
}
