using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using We_have_doom_at_home.MVC.ServerMVC; 


using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
namespace We_have_doom_at_home.Connection;


public class ServerConnection
{
    private const int MaxPlayers = 9;
    private readonly TcpListener Listener;
    private readonly List<(TcpClient Client, int PlayerId)> Clients = new List<(TcpClient, int)>();
    private readonly Dictionary<int, Thread> InputThreads = new Dictionary<int, Thread>();
    private readonly ServerControler Controller;
    private volatile bool IsRunning;
    private int NextPlayerId = 1;

    public ServerConnection(ServerControler controller, IPAddress ip = null, int port = 5555)
    {
        Controller = controller ?? throw new ArgumentNullException(nameof(controller));

        // Use a default IP address if none is provided
        ip ??= IPAddress.Parse("172.30.128.1");

        Listener = new TcpListener(ip, port);
    }

    /// <summary>
    /// Starts listening for incoming player connections and begins the controller's turn-based loop.
    /// </summary>
    public void Start()
    {
        Listener.Start();
        IsRunning = true;
        Logs.Add($"[Server]port: {((IPEndPoint)Listener.LocalEndpoint).Port}, IP: {((IPEndPoint)Listener.LocalEndpoint).Address}");

        // Begin accepting connections
        var acceptThread = new Thread(AcceptLoop) { IsBackground = true };
        acceptThread.Start();
    }

    /// <summary>
    /// Accept loop: waits for new TcpClient, registers player, spawns input listener.
    /// </summary>
    private void AcceptLoop()
    {
        try
        {
            while (IsRunning)
            {
                if (Clients.Count >= MaxPlayers)
                {
                    Thread.Sleep(100);
                    continue;
                }

                var tcp = Listener.AcceptTcpClient();
                int playerId = NextPlayerId++;

                lock (Clients)
                {
                    Clients.Add((tcp, playerId));
                }

                Logs.Add($"[Server] Player {playerId} connected ({Clients.Count}/{MaxPlayers})");

                // Register the new player in the Controller's model
                Controller.AddPlayer(playerId);

                // Optionally send initial state to the new player
                // string initJson = SerializeFullStateForPlayer(playerId);
                // SendToClient(tcp, initJson);

                // Start a thread to listen for this player's inputs
                var inputThread = new Thread(() => ListenPlayerInputs(tcp, playerId))
                {
                    IsBackground = true,
                    Name = $"PlayerInputListener-{playerId}"
                };
                InputThreads[playerId] = inputThread;
                inputThread.Start();
            }
        }
        catch (SocketException ex)
        {
            Logs.Add($"[Server] Accept loop terminated: {ex.Message}");
        }
    }

    private void ListenPlayerInputs(TcpClient client, int playerId)
    {
        using var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
        try
        {
            while (IsRunning && client.Connected)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                PlayerAction? action = null;
                try
                {
                    action = JsonSerializer.Deserialize<PlayerAction>(line);
                }
                catch (JsonException ex)
                {
                    Logs.Add($"[Server] Invalid JSON from Player {playerId}: {line} - {ex.Message}");
                    continue; // Skip invalid input
                }

                if (action == null)
                {
                    Logs.Add($"[Server] Received null action from Player {playerId}");
                    continue;
                }

                // Only save if this player is currently active
                if (Controller.ActivePlayerId == playerId)
                {
                    Logs.Add($"[Server] Active Player {playerId} input saved: {action}");
                    Controller.SetPlayerLastCommand(playerId, action);
                    // Optionally, you can pass the deserialized action object to controller instead of raw JSON:
                    // Controller.SetPlayerLastCommandObject(playerId, action.Value);
                }
                else
                {
                    // Ignore or optionally log input from inactive players
                    // Console.WriteLine($"[Server] Ignored Player {playerId} input (not active)");
                }
            }
        }
        catch (IOException)
        {
            // Client disconnected
        }
        finally
        {
            client.Close();
            Clients.RemoveAll(t => t.PlayerId == playerId);
            Controller.RemovePlayer(playerId);
            Logs.Add($"[Server] Player {playerId} disconnected");
        }
    }

    /// <summary>
    /// Broadcasts a JSON message to all connected players.
    /// </summary>
    public void Broadcast(string json)
    {
        var data = Encoding.UTF8.GetBytes(json + "\n");
        lock (Clients)
        {
            foreach (var (client, id) in Clients)
            {
                if (client.Connected)
                {
                    try
                    {
                        client.GetStream().Write(data, 0, data.Length);
                    }
                    catch
                    {
                        // ignore broken pipe
                    }
                }
            }
        }
    }

    public void SendToClient(int playerId, string json)
    {
        var data = Encoding.UTF8.GetBytes(json + "\n");
        lock (Clients)
        {
            var clientTuple = Clients.FirstOrDefault(c => c.PlayerId == playerId);
            if (clientTuple.Client != null && clientTuple.Client.Connected)
            {
                try
                {
                    clientTuple.Client.GetStream().Write(data, 0, data.Length);
                }
                catch
                {
                    // ignore broken pipe or connection errors
                }
            }
        }
    }


    /// <summary>
    /// Stops accepting new players and shuts down all connections.
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
        Listener.Stop();

        lock (Clients)
        {
            foreach (var (c, _) in Clients)
                c.Close();
            Clients.Clear();
        }

        Logs.Add("[Server] Shutdown complete");
    }



}
