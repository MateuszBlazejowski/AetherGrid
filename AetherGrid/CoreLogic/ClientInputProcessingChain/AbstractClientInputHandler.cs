using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.CoreLogic.InputProcessingChain;
using We_have_doom_at_home.Entities;
using static We_have_doom_at_home.Technical.Log;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.Connection;

namespace We_have_doom_at_home.CoreLogic.ClientInputProcessingChain;

public class AbstractClientInputHandler : IClientInputHandler
{
    protected IClientInputHandler? nextHandler = null;

    public IClientInputHandler SetNext(IClientInputHandler handler)
    {
        if (nextHandler != null)
            nextHandler.SetNext(handler);
        else
        {
            this.nextHandler = handler;
        }
        return handler;
    }

    public virtual void HandleInput(ConsoleKey key, ClientConnection Connection)
    {
        if (nextHandler != null)
        {
            nextHandler.HandleInput(key, Connection);
        }
        else
        {
            HandleGuardAction(key);
        }
    }

    protected void HandleGuardAction(ConsoleKey key)
    {

        Logs.Add($"{key} is not a supported inout key");

    }
}
