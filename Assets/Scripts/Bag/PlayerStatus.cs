using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    private TMP_Text hpText, attackText, defenseText;

    [SerializeField]
    private TMP_Text namePreText, attackPreText, defensePreText, descriptionPreText;
    [SerializeField]
    private Image previewImage;
    [SerializeField]
    private GameObject selectedItemStatus;
    [SerializeField]
    private GameObject selectedItemImage;

    // Start is called before the first frame update
    void Update()
    {
        UpdateEquipmentStatus();
    }

    // Update is called once per frame
    public void UpdateEquipmentStatus()
    {
        hpText.text = GameManager.Instance.GetPlayerHealth().ToString() + "/100";
        attackText.text = GameManager.Instance.GetPlayerAttack().ToString();
        defenseText.text = GameManager.Instance.GetPlayerDefense().ToString();
    }

    public void PreviewEquipmentStatus(Sprite equipSprite, string name,  int attack, int defense, string description)
    {
        namePreText.text = name;
        attackPreText.text = attack.ToString();
        defensePreText.text = defense.ToString();
        descriptionPreText.text = description.ToString();
        previewImage.sprite = equipSprite;
        selectedItemStatus.SetActive(true);
        selectedItemImage.SetActive(true);
    }

    public void TurnOffPreviewStatus()
    {
        namePreText.text = "";
        selectedItemStatus.SetActive(false);
        selectedItemImage.SetActive(false);
    }
}
