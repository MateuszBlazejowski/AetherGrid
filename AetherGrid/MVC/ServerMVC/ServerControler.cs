using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using We_have_doom_at_home.Connection;
using We_have_doom_at_home.CoreLogic.InputProcessingChain;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Rendering;
using We_have_doom_at_home.Technical;
using We_have_doom_at_home.World;
using We_have_doom_at_home.World.Builder;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
namespace We_have_doom_at_home.MVC.ServerMVC;

public class ServerControler
{
    private ServerView ServerView;
    private ServerModel ServerModel;
    public IServerInputHandler actionChain;
    public Director? director { get; set; }

    public ClientImageCreator ClientImageCreator;

    private readonly object CommandLock = new object();
    private PlayerAction? LastPlayerCommand;

    private ServerConnection ServerConnection;
    public int ActivePlayerId { get; private set; }


    private Thread _backgroundRenderThread;
    private readonly object _renderLock = new object();
    public ServerControler(IPAddress ip, int port = 5555)
    {

        ServerView = ServerView.GetInstance();

        var mazeBuilder = new MazeBuilder();
        director = new Director(mazeBuilder);

        director.BuildInstruction1();

        Map map;
        if ((map = mazeBuilder.GetMap()) != null)
            map = mazeBuilder.GetMap();
        else
            throw new Exception("Map is null");

        var instructionBuilder = new InstructionBuilder();
        director.builder = instructionBuilder;
        director.BuildInstruction1();

        ServerView.ExistItems = instructionBuilder.ExistItems;
        ServerView.ExistPotions = instructionBuilder.ExistPotions;
        ServerView.ExistEnemies = instructionBuilder.ExistEnemies;

        ServerModel = new ServerModel(map, instructionBuilder.ExistEnemies,
           instructionBuilder.ExistPotions, instructionBuilder.ExistItems);

        var inputProcessChainBuilder = new InputProcessChainBuilder(map);
        director.builder = inputProcessChainBuilder;
        director.BuildInstruction1();

        actionChain = inputProcessChainBuilder.GetInputHandlerChain();

        ServerConnection = new ServerConnection(this, ip, port);
        ServerConnection.Start();

        ClientImageCreator = new ClientImageCreator(ConsoleMinimalSizeX, ConsoleMinialSizeY);
        ClientImageCreator.ExistItems = instructionBuilder.ExistItems;
        ClientImageCreator.ExistPotions = instructionBuilder.ExistPotions;
        ClientImageCreator.ExistEnemies = instructionBuilder.ExistEnemies;


        _backgroundRenderThread = new Thread(BackgroundRenderLoop)
        {
            IsBackground = true,
            Name = "ServerRenderRefreshThread"
        };
        _backgroundRenderThread.Start();


    }
    public void AddPlayer(int UniqueID)
    {
        Logs.Add($"[Controller] AddPlayer({UniqueID}) called. Attempting to lock ServerModel.players...");

        lock (ServerModel.players)
        {
            Logs.Add($"[Controller] Lock acquired for Player {UniqueID}. Adding player to model...");
            ServerModel.AddPlayer(UniqueID);

            Logs.Add($"[Controller] Player {UniqueID} added to model. Attempting to send initial message to client...");

            var player = ServerModel.players.FirstOrDefault(p => p.PlayerID == UniqueID);
            if (player != null)
            {
                SendMessageToClient(player, ServerModel.players);
                Logs.Add($"[Controller] SendMessageToClient completed for player {UniqueID}.");
            }
            else
            {
                Logs.Add($"[Controller] Warning: Could not find Player {UniqueID} in ServerModel.players!");
            }
        }

        Logs.Add($"[Controller] AddPlayer({UniqueID}) completely finished. Lock released.");
    }

    public void RemovePlayer(int UniqueID)
    {
        lock (ServerModel.players)
        {
            ServerModel.RemovePlayer(UniqueID);
        }

    }
    public void Render()
    {
        lock (_renderLock)
        {
            ServerView.RenderWorld(ServerModel.map, ServerModel.players, Log.Logs);
        }
    }
    public void Run()
    {
        Render();
        while (ServerModel.isRunning)
        {
            if (ServerModel.players.Count == 0)
            {
                Thread.Sleep(100);
                continue;
            }
            // Create a snapshot copy to iterate safely
            List<Player> playersSnapshot;
            lock (ServerModel.players)
            {
                playersSnapshot = new List<Player>(ServerModel.players);
            }

            foreach (var player in playersSnapshot)
            {
                if (player.IsAlive)
                {
                    HandlePlayerTurn(player, playersSnapshot);
                }
            }
            HandleEnemiesTurns();
            ServerModel.NotifyPotion();
        }
    }

    public void HandlePlayerTurn(Player player, List<Player> playersSnapshot)
    {
        ActivePlayerId = player.PlayerID;
        bool CurrentPlayerTurnPassed = false;
        player.TakeDownPotionsEffects();
        while (!CurrentPlayerTurnPassed)
        {
            Thread.Sleep(25);
            PlayerAction? command;
            if ((command = GetLastPlayerCommand()) != null)
            {
                Log.Logs.Add(command.ToString());
                CurrentPlayerTurnPassed = actionChain.HandleInput(command.Value, player);

                foreach (var playerClient in playersSnapshot)
                {
                    SendMessageToClient(playerClient, playersSnapshot);
                }
            }
            NullPlayerLastCommand();
        }
        player.UpdateActivePotionList();
        player.ApplyPotionsEffects();
        Render();

    }

    public void HandleEnemiesTurns()
    {
        for (int i = ServerModel.map.enemies.Count - 1; i >= 0; i--)
        {
            var enemy = ServerModel.map.enemies[i];
            enemy.DoEnemyTurn(this.ServerModel.map, this.ServerModel.players);
            if (!enemy.IsAlive)
            {
                ServerModel.map.enemies.RemoveAt(i);
            }
            List<Player> playersSnapshot;
            lock (ServerModel.players)
            {
                playersSnapshot = new List<Player>(ServerModel.players);
            }
            foreach (var player in playersSnapshot)
            {
                SendMessageToClient(player, playersSnapshot);
            }
            Render();
            Thread.Sleep(10);
        }

    }

    private void SendMessageToClient(Player player, List<Player> playersSnapshot)
    {
        char[][] buffer;

        buffer = ClientImageCreator.RenderPlayerImage(ServerModel.map, player, playersSnapshot);
        var msg = new FrameMessage
        {
            PlayerX = player.PosX,
            PlayerY = player.PosY,
            Rows = buffer.Select(r => new string(r)).ToArray()
        };

        string json = JsonSerializer.Serialize(msg);
        ServerConnection.SendToClient(player.PlayerID, json);
    }

    public void SetPlayerLastCommand(int playerId, PlayerAction? playerAction)
    {
        lock (CommandLock)
        {
            if (playerId == ActivePlayerId)
                LastPlayerCommand = playerAction;
        }
    }

    public void NullPlayerLastCommand()
    {
        lock (CommandLock)
        {
            LastPlayerCommand = null;
        }
    }

    // The game loop can read _lastPlayerCommand when it wants to process input
    public PlayerAction? GetLastPlayerCommand()
    {
        lock (CommandLock)
        {
            return LastPlayerCommand;
        }
    }

    private void BackgroundRenderLoop()
    {
        // Adding a small initial delay to ensure the server is fully started
        Thread.Sleep(1000);

        try
        {
            while (ServerModel != null && ServerModel.isRunning)
            {
                Thread.Sleep(5000); // 5-second delay

                // We wrap this in a lock to prevent collection modified exceptions 
                // in case the main loop adds/removes players at the exact moment of rendering.
                lock (ServerModel.players)
                {
                    Render();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Logs.Add($"[Server] Background render thread stopped unexpectedly: {ex.Message}");
        }
    }
}


public class FrameMessage
{
    public int PlayerX { get; set; }
    public int PlayerY { get; set; }
    public string[] Rows { get; set; }
}