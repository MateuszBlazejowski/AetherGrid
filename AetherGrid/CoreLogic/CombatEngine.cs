using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities.VisitorPattern;
using We_have_doom_at_home.Entities;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Strategy;
using We_have_doom_at_home.Rendering;
using We_have_doom_at_home.World;

namespace We_have_doom_at_home.CoreLogic;


public class CombatEngine
{
    private static CombatEngine _instance;

    private CombatEngine() { }

    public static CombatEngine GetInstance()
    {
        if (_instance == null)
        {
            lock (typeof(CombatEngine))
            {
                if (_instance == null)
                {
                    _instance = new CombatEngine();
                }
            }
        }
        return _instance;
    }


    public void ProcessPlayerCombat(Player player, Enemy enemy, IVisitor attackVisitor, Map map)
    {
        int playerDamage = 0;

        IEquippable? selectedWeapon = null; 

        if (player.HandInUse == PointedHand.Left)
        {
            selectedWeapon = player.LeftHand;
        }
        else
        {
            selectedWeapon = player.RightHand;
        }

        if (selectedWeapon != null)
            playerDamage += selectedWeapon.Accept(attackVisitor, player);

        int damageToEnemy = Math.Max(0, playerDamage - enemy.Armor);
        enemy.SubstractHealthPoints(damageToEnemy);

        Logs.Add($"You dealt {damageToEnemy} to {enemy.Name}. It has {enemy.Health}HP");




        int enemyDamage = enemy.Attack;
        if (selectedWeapon != null)
            enemyDamage -= selectedWeapon.AcceptDefense(attackVisitor, player);
        enemyDamage = Math.Max(0, enemyDamage);  
        player.SubstractHealthPoints(enemyDamage);

        Logs.Add($"{enemy.Name} dealt {enemyDamage}. You have {player.Health}HP");



        
        // Check if the enemy is defeated
        if (enemy.Health <= 0)
        {
            Logs.Add($"{enemy.Name} has been defeated!");
            map.enemies.Remove(enemy);
        }
        // Check if the player is defeated
        if (player.Health <= 0)
        {
            player.IsAlive = false;
            Logs.Add($"The{player.PlayerID}player has been defeated!");
        }


    }

    public void ProcessEnemyCombat(Enemy enemy, Player player, IVisitor attackVisitor, Map map)
    {
        int enemyDamage = 0;

        enemyDamage = enemy.Attack;

        IEquippable? playerWeapon = (player.HandInUse == PointedHand.Left) ? player.LeftHand : player.RightHand;
        if (playerWeapon != null)
        {
            enemyDamage -= playerWeapon.AcceptDefense(attackVisitor, player);
        }
        enemyDamage = Math.Max(0, enemyDamage);
        player.SubstractHealthPoints(enemyDamage);

        Logs.Add($"{enemy.Name} dealt {enemyDamage}. You have {player.Health}HP");

        int playerDamage = 0;
        if (playerWeapon != null)
        {
            playerDamage += playerWeapon.Accept(attackVisitor, player);
        }
        int damageToEnemy = Math.Max(0, playerDamage - enemy.Armor);
        enemy.SubstractHealthPoints(damageToEnemy);

        Logs.Add($"You dealt {damageToEnemy} to {enemy.Name}. It has {enemy.Health}HP");



        if (enemy.Health <= 0)
        {
            Logs.Add($"{enemy.Name} has been defeated!");
            map.enemies.Remove(enemy);
        }
        if (player.Health <= 0)
        {
            player.IsAlive = false;
            Logs.Add($"The player {player.PlayerID} has been defeated!");
        }
    }



}

