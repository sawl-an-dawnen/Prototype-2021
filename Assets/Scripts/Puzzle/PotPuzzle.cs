using UnityEngine;
using UnityEngine.UI;

public class PotPuzzle : MonoBehaviour
{
	public Slider redSlider;
	public Slider greenSlider;
	public Slider blueSlider;
	public Button mixButton;

	public GameObject ghostPrefab;
	public GameObject panel;

	public void Resume()
	{
		panel.SetActive(false);
		Time.timeScale = 1f;
	}

	public void Pause()
	{
		panel.SetActive(true);
		Time.timeScale = 0f;
	}

	void Start()
	{
		panel.SetActive(false);
	}

	void Update()
	{
		if (panel.activeSelf)
		{
			Pause();
		}
	}
		public void OnMixButtonClick()
	{
		float redValue = redSlider.value;
		float greenValue = greenSlider.value;
		float blueValue = blueSlider.value;

		Color mixedColor = new Color(redValue, greenValue, blueValue);

		ApplyColorToGhostPrefab(mixedColor);
		Resume();
	}

	void ApplyColorToGhostPrefab(Color color)
	{
		Renderer ghostRenderer = ghostPrefab.GetComponent<Renderer>();
		Material ghostMaterial = ghostRenderer.sharedMaterial;

		ghostMaterial.SetColor("_Color_Me", color);
	}
}
