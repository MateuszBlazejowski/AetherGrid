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

public class MovementInputClientHandler : AbstractClientInputHandler
{
    public override void HandleInput(ConsoleKey key, ClientConnection Connection)
    {
        switch (key)
        {
            case ConsoleKey.W:
                Connection.Send(PlayerAction.MoveUp);
                break;
            case ConsoleKey.S:
                Connection.Send(PlayerAction.MoveDown);
                break;
            case ConsoleKey.A:
                Connection.Send(PlayerAction.MoveLeft);
                break;
            case ConsoleKey.D:
                Connection.Send(PlayerAction.MoveRight);
                break;
            default:
                base.HandleInput(key, Connection);
                break;
        }
    }
}

