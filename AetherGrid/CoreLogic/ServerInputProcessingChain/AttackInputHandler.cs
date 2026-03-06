using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.Entities.VisitorPattern;
using We_have_doom_at_home.Technical;
using We_have_doom_at_home.World;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
namespace We_have_doom_at_home.CoreLogic.InputProcessingChain;

public class AttackInputHandler : AbstractServerInputHandler
{
    private Map map;

    public AttackInputHandler(Map _map)
    {
        map = _map;
    }

    public override bool HandleInput(PlayerAction playerAction, Player player)
    {
        IVisitor visitor;

        CombatEngine combatEngine = CombatEngine.GetInstance();

        var currentEnemy = map.enemies.FirstOrDefault(enemy => enemy.PosX == player.PosX && enemy.PosY == player.PosY);

        if (playerAction == PlayerAction.Attack1 && !player.IsInInventory)
        {
            if (currentEnemy != null)
            {
                visitor = new NormalAttackVisitor();
                combatEngine.ProcessPlayerCombat(player, currentEnemy, visitor, map);
                return true;
            }
            else
            {
                Logs.Add("No enemy to attack");
                return false;
            }
        }
        else if (playerAction == PlayerAction.Attack2 && !player.IsInInventory)
        {
            if (currentEnemy != null)
            {
                visitor = new StealthAttackVisitor();
                combatEngine.ProcessPlayerCombat(player, currentEnemy, visitor, map);
                return true;
            }
            else
            {
                Logs.Add("No enemy to attack");
                return false;
            }
        }
        else if (playerAction == PlayerAction.Attack3 && !player.IsInInventory)
        {
            if (currentEnemy != null)
            {
                visitor = new MagicAttackVisitor();
                combatEngine.ProcessPlayerCombat(player, currentEnemy, visitor, map);
                return true;
            }
            else
            {
                Logs.Add("No enemy to attack");
                return false;
            }
        }
        else
        {
            return base.HandleInput(playerAction, player);
        }
    }
}
