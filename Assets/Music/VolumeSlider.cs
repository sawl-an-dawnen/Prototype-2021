using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
	public OptionsMenu optionsMenu;
	private Slider slider;

	private void Start()
	{
		slider = GetComponent<Slider>();
		InitializeSliderValue();
		slider.onValueChanged.AddListener(OnSliderValueChanged);
		OnSliderValueChanged(slider.value);
	}

	private void InitializeSliderValue()
	{
		float currentVolume = PlayerPrefs.GetFloat("volume", 0.0f);
		slider.value = currentVolume;
	}

	private void OnSliderValueChanged(float value)
	{
		optionsMenu.SetVolume(value);
		PlayerPrefs.SetFloat("volume", value);
	}
}
