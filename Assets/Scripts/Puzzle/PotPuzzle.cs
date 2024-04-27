using UnityEngine;
using UnityEngine.UI;

public class PotPuzzle : MonoBehaviour
{
	public Slider redSlider;
	public Slider greenSlider;
	public Slider blueSlider;
	public Button mixButton;

	public GameObject ghostPrefab;


	public void OnMixButtonClick()
	{
		float redValue = redSlider.value;
		float greenValue = greenSlider.value;
		float blueValue = blueSlider.value;

		Color mixedColor = new Color(redValue, greenValue, blueValue);

		ApplyColorToGhostPrefab(mixedColor);
	}

	void ApplyColorToGhostPrefab(Color color)
	{
		Renderer ghostRenderer = ghostPrefab.GetComponent<Renderer>();
		Material ghostMaterial = ghostRenderer.material;

		ghostMaterial.SetColor("_Color_Me", color);
	}
}
