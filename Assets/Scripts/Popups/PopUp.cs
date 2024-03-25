using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public TMP_Text popUpDescription;
    public Image img;

    public void SetUpPopUp(string desc, Sprite icon)
    {
        popUpDescription.text = desc;
        img.sprite = icon;
    }
}
