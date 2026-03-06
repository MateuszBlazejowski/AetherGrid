using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

using We_have_doom_at_home.Entities; 

public abstract class AbstractServerInputHandler : IServerInputHandler
{
    protected IServerInputHandler? nextHandler = null;

    public IServerInputHandler SetNext(IServerInputHandler handler)
    {
        if (nextHandler != null)
            nextHandler.SetNext(handler);
        else
        {
            this.nextHandler = handler;
        }
        return handler;
    }

    public virtual bool HandleInput(PlayerAction playerAction, Player player)
    {
        if (nextHandler != null)
        {
            return nextHandler.HandleInput(playerAction, player);
        }
        else
        {
           HandleGuardAction(playerAction, player);
           return false;
        }
    }

    protected void HandleGuardAction(PlayerAction playerAction, Player player)
    {
        
        Logs.Add($"{playerAction.ToString()} is not avaliable");
        
    }
}
