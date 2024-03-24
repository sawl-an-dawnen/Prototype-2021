using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    private TMP_Text hpText, attackText, defenseText;

    // Start is called before the first frame update
    void Start()
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
}
