using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Entities.ObserverPattern;
using We_have_doom_at_home.World;
using We_have_doom_at_home.World.Builder;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.MVC.ServerMVC;

public class ServerModel : ISubjectPotion
{
    public bool isRunning { get; set; }
    public Map? map { get; }
    public List<Player> players { get; set; }


    private bool existItems;
    private bool existPotions;
    private bool existEnemies;

    private List<IPotionObserver> observersPotions = new List<IPotionObserver>();

    public ServerModel(Map map, bool existItems, bool existPotions, bool existEnemies)
    {
        players = new List<Player>();
        this.map = map;
        this.existItems = existItems;
        this.existPotions = existPotions;
        this.existEnemies = existEnemies;

        isRunning = true;
    }

    //public void GetNewMapFromBuilder()
    //{
    //    if (builder.GetMap() != null)
    //        map = builder.GetMap();
    //    else
    //        throw new Exception("Map is null");
    //}
    //public void SetMazeBuilder()
    //{
    //    builder = new MazeBuilder();
    //    if (director == null)
    //    {
    //        director = new Director(builder);
    //    }
    //    else
    //    {
    //        director.builder = builder;
    //    }    
    //}
    //public void SetInstructionBuilder()
    //{
    //    builder = new InstructionBuilder();
    //    if (director == null)
    //    {
    //        director = new Director(builder);
    //    }
    //    else
    //    {
    //        director.builder = builder;
    //    }
    //}

    public void AttachPotion(IPotionObserver observer) => observersPotions.Add(observer);
    public void DetachPotion(IPotionObserver observer) => observersPotions.Remove(observer);

    public void NotifyPotion()
    {
        foreach (var observer in observersPotions)
        {
            observer.Update();
        }
    }


    public void AddPlayer(int UniqueID)
    {
        var player = GetNewPlayer(UniqueID);
        this.players.Add(player);
    }
    public void RemovePlayer(int UniqueID)
    {
        players.RemoveAll(p => p.PlayerID == UniqueID);
    }
    private Player GetNewPlayer(int UniqueID)
    {
        int x = 0, y = 0;
        if (this.map != null)
        {
            FindRandomWalkable(out x, out y, this.map);
        }
        else
        {
            throw new Exception("cant add player couse map doesnt exist");
        }
        return new Player(x, y, this, UniqueID);
    }

    private void FindFirstWalkable(out int x, out int y, Map map)
    {
        for (y = 0; y < MapHeight; y++)
        {
            for (x = 0; x < MapWidth; x++)
            {
                if (map.tileMap[x, y] == Tile.Floor)
                    return;
            }
        }
        throw new Exception("No place for player");
    }

    private void FindRandomWalkable(out int x, out int y, Map map)
    {
        Random rand = new Random();
        int maxAttempts = MapWidth * MapHeight * 2;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            x = rand.Next(0, MapWidth);
            y = rand.Next(0, MapHeight);

            if (map.tileMap[x, y] == Tile.Floor)
                return;

            attempts++;
        }

        FindFirstWalkable(out x, out y, map); // if random didnt work, find first 
    }


}
