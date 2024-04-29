using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonPaintingUnlock : MonoBehaviour
{
    public GameManager manager;
    public DialogueUI dialogueUI;

    public DialogueObject smallFirst;
    public DialogueObject smallSecond;

    public DialogueObject BigFirst;
    public DialogueObject BigSecond;


    //public bool hasSpokenToMain = false;
    //public bool hasGrabbedMini = false;
    public GameObject miniPainting;

    // Update is called once per frame
    void Update()
    {
        
    }
   

    public void checkSmallDragon() //when we talk to the big dragon
    {
        if (manager.hasSpokeSmallDragon) //have we talked to small (1st floor)?
        {
            dialogueUI.ShowDialogue(BigSecond);
        }

        else dialogueUI.ShowDialogue(BigFirst); 

    }
    public void checkBigDragon() //when we talk to the small dragon
    {
        if (manager.hasSpokeBigDragon) //if we have already talked to big
        {
            miniPainting.SetActive(false);
            dialogueUI.ShowDialogue(smallSecond);
        }

        else dialogueUI.ShowDialogue(smallFirst);


    }

    /*
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
    */
}
