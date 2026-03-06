using We_have_doom_at_home.CoreLogic;
using We_have_doom_at_home.Entities.Decorators;
using We_have_doom_at_home.Entities.Items;
using We_have_doom_at_home.Entities;
using System.Runtime.InteropServices;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.Connection; 
using We_have_doom_at_home.MVC.ServerMVC;
using We_have_doom_at_home.MVC;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace We_have_doom_at_home;

internal class Program
{
    static void Main(string[] args)
    {
        //GameEngine engine = new GameEngine();
        //engine.Run();

        GameSetup game = new GameSetup();
        game.Run();

        //ServerControler serverControler = new ServerControler();
        //serverControler.Run();

    }
}




