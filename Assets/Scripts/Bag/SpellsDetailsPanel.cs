using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SpellsDetailsPanel : MonoBehaviour
{
    public TextMeshProUGUI spellNameText;
    public Image spellImage;
    public TextMeshProUGUI spellCostText;
	public TextMeshProUGUI spellDescriptionText;

	private string GetElementName(int index)
	{
		switch (index)
		{
			case 0: return "Water";
			case 1: return "Fire";
			case 2: return "Earth";
			case 3: return "Wind";
			default: return "Unknown";
		}
	}

	private string GetElementColor(int index)
	{
		switch (index)
		{
			case 0: return "Blue";
			case 1: return "Red";
			case 2: return "Green";
			case 3: return "Yellow";
			default: return "Unknown";
		}
	}

	public void ShowDetails(GameManager.Spell spell)
    {
        spellNameText.text = spell.name;
        spellImage.sprite = spell.prefabButton.GetComponent<Image>().sprite; 
		spellCostText.text = "Cost:\n" + string.Join("\n", spell.cost.Select((c, i) => $"{GetElementName(i)}({GetElementColor(i)}): {c}"));
		spellDescriptionText.text = spell.description;

	}
}
