using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonPaintingUnlock : MonoBehaviour
{
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
        if (GameManager.Instance.hasSpokeSmallDragon) //have we talked to small (1st floor)?
        {
            dialogueUI.ShowDialogue(BigSecond);
        }

        else
        {
            GameManager.Instance.setBigDragonStatus();
            dialogueUI.ShowDialogue(BigFirst);
        }

        }
        public void checkBigDragon() //when we talk to the small dragon
    {
        if (GameManager.Instance.hasSpokeBigDragon) //if we have already talked to big
        {
            miniPainting.SetActive(false);
            dialogueUI.ShowDialogue(smallSecond);
        }

        else 
        {
            GameManager.Instance.setSmallDragonStatus();
            dialogueUI.ShowDialogue(smallFirst);
        } 
            
            


    }

    
}
