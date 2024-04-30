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
		if(spell.weaknessIsFound){
			spellDescriptionText.text = spell.description + "\n" + "Strong Against:\n" + spell.strongAgainst;
		}
		else if (spell.name == "Dodge" || spell.name == "Heal" || spell.name =="Stun" || spell.name == "Earth Element" || spell.name == "Water Element"){
			spellDescriptionText.text = spell.description;
		}
		else{
			spellDescriptionText.text = spell.description + " Strong Against: ???";
		}

	}
}
