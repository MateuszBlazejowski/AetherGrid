using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using static We_have_doom_at_home.Technical.Log;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities.Items;
using We_have_doom_at_home.World;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.CoreLogic.InputProcessingChain;
namespace We_have_doom_at_home.World.Builder;

public class InputProcessChainBuilder : IBuilder 
{
    private Map map;

    private bool ExitInputHandlerAdded { get; set; } = false;
    private bool BasicHandlersAdded { get; set; } = false;
    private bool ExistItems { get; set; } = false;
    private bool ExistPotions { get; set; } = false;

    IServerInputHandler? firstChainLinkInputHandler;

    public InputProcessChainBuilder(Map _map)
    { 
        map = _map;
    }

    private void AddBasicsLinks()
    {
        if (BasicHandlersAdded)
            return;

        var exitInputHandler = new ExitInputHandler();
        var movementInputHandler = new MovementInputHandler(map);

        if (firstChainLinkInputHandler != null)
            firstChainLinkInputHandler.SetNext(movementInputHandler);

        if (!ExitInputHandlerAdded && firstChainLinkInputHandler!= null )
            firstChainLinkInputHandler.SetNext(exitInputHandler); 


        BasicHandlersAdded = true;
        ExitInputHandlerAdded = true;
    }

    private void AddItemsHandlingLinks()
    {
        if (ExistItems)
            return;

        var pickupHandler = new PickUpInputHandler(map);
        var changeHandHandler = new ChangeHandInputHandler();
        var equipItemHandler = new EquipInputHandler();
        var inventoryEnteringInputHandler = new InventoryEnteringInputHandler();
        var inventoryIteratingInputHandler = new InventoryIteratingInputHandler();
        var dropItemInputHandler = new DropItemInputHandler(map);

        if (firstChainLinkInputHandler == null)
            firstChainLinkInputHandler = pickupHandler;
        else
            firstChainLinkInputHandler.SetNext(pickupHandler);

        firstChainLinkInputHandler?.SetNext(changeHandHandler)
                                   .SetNext(equipItemHandler)
                                   .SetNext(inventoryEnteringInputHandler)
                                   .SetNext(inventoryIteratingInputHandler)
                                   .SetNext(dropItemInputHandler); 

        ExistItems = true; 
    }
    public IBuilder addCentralChamber()
    {
        AddBasicsLinks();
        return this;
    }

    public IBuilder addChambers()
    {
        AddBasicsLinks();
        return this;
    }

    public IBuilder addEnemies()
    {
        AddBasicsLinks();
        var attackInputHandler = new AttackInputHandler(map);
        firstChainLinkInputHandler?.SetNext(attackInputHandler);
        return this;
    }

    public IBuilder addItems()
    {
        AddBasicsLinks();
        AddItemsHandlingLinks();
        return this;
    }

    public IBuilder addModifiedWeapons()
    {
        AddBasicsLinks();
        AddItemsHandlingLinks();
        return this;
    }

    public IBuilder addPaths()
    {
        AddBasicsLinks();
        return this;
    }

    public IBuilder addPotions()
    {
        AddBasicsLinks();
        AddItemsHandlingLinks();
        if(firstChainLinkInputHandler!= null)
            firstChainLinkInputHandler.SetNext(new ConsumePotionInputHandler());
        return this;
    }

    public IBuilder addWeapons()
    {
        AddBasicsLinks();
        AddItemsHandlingLinks();
        return this;
    }

    public IBuilder emptyDungeon()
    {
        AddBasicsLinks();
       return this;
    }

    public IBuilder filledDungeon()
    {
        ExistItems = false;
        ExistPotions = false;
        BasicHandlersAdded = false;
        firstChainLinkInputHandler = null;
        var exitInputHandler = new ExitInputHandler();
        firstChainLinkInputHandler = exitInputHandler;
        ExitInputHandlerAdded = true;
        return this;
    }

    public IServerInputHandler GetInputHandlerChain()
    {
        if (firstChainLinkInputHandler != null)
            return firstChainLinkInputHandler;
        else
            throw new Exception("Builder didnt build input chain of responsibility");
    }

}
