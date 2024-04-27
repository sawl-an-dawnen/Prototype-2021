using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonPaintingUnlock : MonoBehaviour
{
    public int paintingNum;

    public bool hasSpokenToMain = false;
    public bool hasGrabbedMini = false;
    public GameObject miniPainting;

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void speakToMain()
    {
        hasSpokenToMain = true;
    }

    public void pickUpMini()
    {
        if(hasSpokenToMain == true)
        {
            hasGrabbedMini = true;
            miniPainting.SetActive(false);
        }
            
    }
}
