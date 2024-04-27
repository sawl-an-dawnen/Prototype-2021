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
		float redValue = redSlider.value / 255f;
		float greenValue = greenSlider.value / 255f;
		float blueValue = blueSlider.value / 255f;

		Color mixedColor = new Color(redValue, greenValue, blueValue, 0f);

		ApplyColorToGhostPrefab(mixedColor);
		Resume();
	}


	void ApplyColorToGhostPrefab(Color color)
	{
		var gm = GameManager.Instance;
		Renderer ghostRenderer = ghostPrefab.GetComponent<Renderer>();
		Material ghostMaterial = ghostRenderer.sharedMaterial;

		ghostMaterial.SetColor("_Color_Me", color);
		gm.SetPlayerColor(color);
		gm.SaveCheckpoint();
	}
}
