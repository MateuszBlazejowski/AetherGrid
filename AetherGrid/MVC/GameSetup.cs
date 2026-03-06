using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Connection;
using We_have_doom_at_home.MVC.ServerMVC;
using We_have_doom_at_home.MVC.ClientMVC;
using System.Net;
namespace We_have_doom_at_home.MVC;

public class GameSetup
{
    public void Run()
    {
        // 1) Check command-line args first
        var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        if (args.Length > 0)
        {
            var mode = args[0].ToLower();
            if (mode == "--server" || mode == "server")
            {
                IPAddress ip = IPAddress.Parse("172.30.128.1"); // Default IP
                int port = 5555; // Default port

                if (args.Length > 1)
                {
                    // If IP and port are passed as `ip:port`
                    var parts = args[1].Split(':');
                    ip = IPAddress.Parse(parts[0]); // IP address
                    if (parts.Length > 1 && int.TryParse(parts[1], out var p))
                    {
                        port = p; // Parse port if provided
                    }
                }

                new ServerControler(ip, port).Run();
                return;
            }
            else if (mode == "--client" || mode == "client")
            {
                string ip = "127.0.0.1";
                int port = 5555;
                if (args.Length > 1)
                {
                    var parts = args[1].Split(':');
                    ip = parts[0];
                    if (parts.Length > 1 && int.TryParse(parts[1], out var p2))
                        port = p2;
                }
                new ClientControler().Run(ip, port);
                return;
            }
        }

        // 2) Fallback to interactive prompt
        while (true)
        {
            Console.Write("Start as (S)erver or (C)lient? ");
            var mode = Console.ReadLine()?.Trim().ToLower();
            if (mode == "s" || mode == "server")
            {
                Console.Write("Enter IP address (default 127.0.0.1): ");
                var ipInput = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(ipInput))
                    ipInput = "127.0.0.1"; // Use default IP if none is provided

                // Prompt for port, show default 5555
                Console.Write("Enter listening port (default 5555): ");
                var portInput = Console.ReadLine()?.Trim();
                if (!int.TryParse(portInput, out var port))
                    port = 5555; // Default port if input is invalid

                if (!IPAddress.TryParse(ipInput, out var ipAddress))
                {
                    Console.WriteLine("Invalid IP address format.");
                    continue; // Retry if IP format is invalid
                }

                new ServerControler(ipAddress, port).Run();
                break;
            }
            else if (mode == "c" || mode == "client")
            {
                // Prompt for ip:port, show default 127.0.0.1:5555
                Console.Write("Enter server IP and port (default 127.0.0.1:5555): ");
                var endpoint = Console.ReadLine()?.Trim();
                string ip = "127.0.0.1";
                int port = 5555;

                if (!string.IsNullOrEmpty(endpoint))
                {
                    var parts = endpoint.Split(':');
                    ip = parts[0];
                    if (parts.Length > 1 && int.TryParse(parts[1], out var p))
                        port = p;
                }

                new ClientControler().Run(ip, port);
                break;
            }
            else
            {
                Console.WriteLine("Invalid option, please enter S or C.");
            }
        }

    }

}
