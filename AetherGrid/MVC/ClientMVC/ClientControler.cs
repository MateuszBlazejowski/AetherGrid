using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Connection;
using We_have_doom_at_home.CoreLogic.ClientInputProcessingChain;
using We_have_doom_at_home.Technical;

namespace We_have_doom_at_home.MVC.ClientMVC;

public class ClientControler
{
    private readonly ClientConnection Connection = new();
    private ClientView ClientView; 

    // The head of the chain
    private readonly IClientInputHandler InputHandlerChain;

    public ClientControler()
    {
        ClientView = ClientView.GetInstance();
        // Build the chain of responsibility
        InputHandlerChain = new MovementInputClientHandler();
        InputHandlerChain
            .SetNext(new AttackInputClientHandler())
            .SetNext(new ChangeHandInputClientHandler())
            .SetNext(new ConsumePotionInputClientHandler())
            .SetNext(new DropItemInputClientHandler())
            .SetNext(new EquipInputClientHandler())
            .SetNext(new ExitInputClientHandler())
            .SetNext(new InventoryEnteringInputClientHandler())
            .SetNext(new PickUpInputClientHandler());
        // Add more handlers here if you have them
    }
    public void TriggerDisplay(char[][] Buffer, int posX, int posY)
    {
        ClientView.DisplayPlayerImage(Buffer, Log.Logs, posX, posY );
    }
    public void Run(string ip = "127.0.0.1", int port = 5555)
    {
        Connection.Connect(this, ip, port);

        Console.WriteLine("Press keys to send commands. Press 'Esc' or type 'quit' to exit.");

        while (true)
        {
            if (!Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(10);
                continue;
            }

            var keyInfo = Console.ReadKey(true);
  
            // Pass key to the input handler chain, along with connection to send JSON
            InputHandlerChain.HandleInput(keyInfo.Key, Connection);
            if (keyInfo.Key == ConsoleKey.Escape)
                break;

        }

        Connection.Disconnect();
        Console.WriteLine("Disconnected from server.");
    }
}
