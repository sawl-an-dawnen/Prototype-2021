using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public SceneChangeInvokable DoorToOpen;

    public DialogueObject dialogueNo;
    public InventoryItem news1;
    public InventoryItem news2;
    public InventoryItem news3;
    public InventoryItem helmet;
    public InventoryItem boots;
    public InventoryItem axe;
    public InventoryItem sword;
    public DialogueUI dialogueUI;
    public GameObject toDestroy;

    private bool hasNews1;
    private bool hasNews2;
    private bool hasNews3;
    private bool condition;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.RegisterInventoryManager(this);
    }

    public void AddItem(InventoryItem item)
    {
        gameManager.AddItem(item);
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
}