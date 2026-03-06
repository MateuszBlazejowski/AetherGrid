using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.ObserverPattern;
using We_have_doom_at_home.Entities.Strategy;
using We_have_doom_at_home.Technical;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;

namespace We_have_doom_at_home.Entities;

public class Enemy 
{
    public int PosX;
    public int PosY;
    public int Health;
    public int Attack;
    public int Armor;
    public string Name;
    public bool IsAlive; 

    private IStrategy Strategy;
    public Enemy(int posX, int posY, int health, int attack, string name, int armor, IStrategy strategy)
    {
        PosX = posX;
        PosY = posY;
        Health = health;
        Attack = attack;
        Name = name;
        Armor = armor;
        Strategy = strategy;
        IsAlive = true;
    }

    public void SetStrategy(IStrategy strategy)
    {
        Strategy = strategy;
    }

    public void DoEnemyTurn(Map map, List<Player>players)
    {
        Strategy.Behave(map, players, this);

    }

    public void TryWalk(int deltaX, int deltaY, Map map)
    {
        Logs.Add($"Invoked TryWalk({deltaX} {deltaY})");
        if (!IsWalkable(PosX + deltaX, PosY + deltaY, map)) return;
        PosX += deltaX;
        PosY += deltaY;
    }

    public bool IsWalkable(int x, int y, Map map)
    {
        if (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight) return false;
        if (map.tileMap[x, y] == Tile.Wall) return false;
        return true;
    }

    public void SubstractHealthPoints(int damage)
    {
        if (this.Health - damage < 0)
        {
            this.Health = 0;
        }
        else
        {
            this.Health -= damage;
        }
    }
    public double GetDistanceToPlayer(int playerPosX, int playerPosY)
    {
        return Math.Sqrt(Math.Pow(PosX - playerPosX, 2) + Math.Pow(PosY - playerPosY, 2));
    }


}
