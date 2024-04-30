using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public SceneChangeInvokable DoorToOpen;

    public DialogueObject dialogueNo;
    public InventoryItem news1;
    public InventoryItem news2;
    public InventoryItem news3;
    public InventoryItem crowbar;
    public InventoryItem helmet;
    public InventoryItem boots;
    public InventoryItem axe;
    public InventoryItem sword;
    public DialogueUI dialogueUI;
    public GameObject toDestroy;
    public InventoryItem[] equipmentArray;

    private bool hasNews1;
    private bool hasNews2;
    private bool hasNews3;
    private bool hasCrowbar;
    private bool condition;
    private bool condition2;

    private GameManager gameManager;
    private PopUpManager pm;
    private SpellsManager spellsManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        spellsManager = GameObject.Find("SpellsManager").GetComponent<SpellsManager>();
        gameManager.RegisterInventoryManager(this);
    }

    public void AddItem(InventoryItem item)
    {
        gameManager.AddItem(item);
        if (item.isEquip)
        {
            spellsManager.AddEquipment(item);
        }
        pm = GameObject.FindGameObjectWithTag("PopMan").GetComponent<PopUpManager>();
        pm.CreatePopUp("You Found " + item.itemName + ", press 'I' to check your inventory", item.itemIcon);
    }

    public void AddEquipFromBattle(InventoryItem item)
    {
        gameManager.AddItem(item);
        if (item.isEquip)
        {
            spellsManager.AddEquipment(item);
        }
    }

    public InventoryItem GiveRandomEquipment()
    {
        // Array of available equipment
        //InventoryItem[] equipmentArray = { helmet, boots, axe, sword };

        // Calculate total weights based on rarity
        int totalWeight = 0;
        foreach (InventoryItem item in equipmentArray)
        {
            totalWeight += GetRarityWeight(item.equipRarity);
        }

        Debug.Log("Now the total weight is: " + totalWeight);

        // Generate a random number within totalWeight range
        int randomNumber = Random.Range(0, totalWeight);

        Debug.Log("Random Number: " + randomNumber);

        // Choose an equipment based on random number
        InventoryItem chosenEquipment = null;
        int cumulativeWeight = 0;
        foreach (InventoryItem item in equipmentArray)
        {
            cumulativeWeight += GetRarityWeight(item.equipRarity);
            if (randomNumber < cumulativeWeight)
            {
                chosenEquipment = item;
                break;
            }
        }

        // Give the chosen equipment to the player
        if (chosenEquipment != null)
        {
            AddEquipFromBattle(chosenEquipment);
            Debug.Log("Chosen Equipment: " + chosenEquipment.itemName);
            return chosenEquipment;
        }
        else
        {
            Debug.LogWarning("No equipment chosen.");
            return null;
        }
    }

    // Helper function to get the weight of each rarity
    private int GetRarityWeight(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.N:
                return 50; // Adjust weight as needed
            case Rarity.R:
                return 30;
            case Rarity.SR:
                return 15;
            case Rarity.UR:
                return 5;
            default:
                return 0;
        }
    }

    public void RemoveItem(InventoryItem item)
    {
        gameManager.RemoveItem(item);
    }

    public void CheckNewspaper()
    {
        List<string> items = gameManager.GetItmes();

        hasNews1 = false;
        hasNews2 = false;
        hasNews3 = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == "News1")
            {
                hasNews1 = true;
            }
            else if (items[i] == "News2")
            {
                hasNews2 = true;
            }
            else if (items[i] == "News3")
            {
                hasNews3 = true;
            }
        }
        if (hasNews1 && hasNews2 && hasNews3)
        {
            condition = true;
        }
        else
        {
            condition = false;
        }

        if (condition)
        {
            DoorToOpen.CanEnter = true;
            gameManager.OpenDoor();
            Destroy(toDestroy);
        }
        else
        {
            dialogueUI.ShowDialogue(dialogueNo); 
        }

    }
    public void CheckCworbar()
    {
        List<string> items = gameManager.GetItmes();

        hasCrowbar = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == "Crowbar")
            {
                hasCrowbar = true;
            }
        }
        if (hasCrowbar)
        {
            condition2 = true;
        }
        else
        {
            condition2 = false;
        }

        if (condition2)
        {
            DoorToOpen.CanEnter = true;
            gameManager.OpenDoor();
            Destroy(toDestroy);
        }
        else
        {
            dialogueUI.ShowDialogue(dialogueNo);
        }

    }
}