using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using static We_have_doom_at_home.Technical.Log;
using static We_have_doom_at_home.Technical.Common;
namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

public interface IServerInputHandler
{
    IServerInputHandler SetNext(IServerInputHandler actionHandler);

    bool HandleInput(PlayerAction playerAction, Player player);  // returns true if a turn passed
}
