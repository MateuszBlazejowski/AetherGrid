using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Connection;
using We_have_doom_at_home.CoreLogic.InputProcessingChain;
using We_have_doom_at_home.Entities;

namespace We_have_doom_at_home.CoreLogic.ClientInputProcessingChain;

public interface IClientInputHandler
{
    IClientInputHandler SetNext(IClientInputHandler keyHandler);

    void HandleInput(ConsoleKey key, ClientConnection Connection);  
}
