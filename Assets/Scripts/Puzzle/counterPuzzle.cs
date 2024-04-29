//counterPuzzle.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class counterPuzzle : MonoBehaviour
{
    public TextMeshProUGUI numberText1;
    public TextMeshProUGUI numberText2;
    public TextMeshProUGUI numberText3;
    public TextMeshProUGUI numberText4;
    public TextMeshProUGUI successText;
    public Image successImage;
    public AudioClip counterTick;
    //public AudioClip counterPuzzleComplete;

    int counter1;
    int counter2;
    int counter3;
    int counter4;
	public GameObject panel;
    private bool puzzleSolved = false;

    public string spellToGet;
    private Sprite img;
    private AudioSource audioSource;

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

	public void upBottonPressed1()
    {

        if (counter1 < 10)
        {
            audioSource.PlayOneShot(counterTick);
            counter1++;
            numberText1.text = counter1.ToString();
        }
    }

    public void downBottonPressed1()
    {
        if (counter1 > 0)
        {
            audioSource.PlayOneShot(counterTick);
            counter1--;
            numberText1.text = counter1.ToString();
        }
    }
    public void upBottonPressed2()
    {
        if (counter2 < 10)
        {
            audioSource.PlayOneShot(counterTick);
            counter2++;
            numberText2.text = counter2.ToString();
        }
    }

    public void downBottonPressed2()
    {
        if (counter2 > 0)
        {
            audioSource.PlayOneShot(counterTick);
            counter2--;
            numberText2.text = counter2.ToString();
        }
    }
    public void upBottonPressed3()
    {
        if (counter3 < 10)
        {
            audioSource.PlayOneShot(counterTick);
            counter3++;
            numberText3.text = counter3.ToString();
        }
    }

    public void downBottonPressed3()
    {
        if (counter3 > 0)
        {
            audioSource.PlayOneShot(counterTick);
            counter3--;
            numberText3.text = counter3.ToString();
        }
    }
    public void upBottonPressed4()
    {
        if (counter4 < 10)
        {
            audioSource.PlayOneShot(counterTick);
            counter4++;
            numberText4.text = counter4.ToString();
        }
    }

    public void downBottonPressed4()
    {
        if (counter4 > 0)
        {
            audioSource.PlayOneShot(counterTick);
            counter4--;
            numberText4.text = counter4.ToString();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
		successText.gameObject.SetActive(false);
        successImage.gameObject.SetActive(false);
    }

	void Update()
	{
		if (panel.activeSelf)
		{
			Pause();
		}

		if (counter1 == 1 && counter2 == 3 && counter3 == 1 && counter4 == 4 && !puzzleSolved)
		{
            puzzleSolved = true;
            successText.gameObject.SetActive(true);
			successImage.gameObject.SetActive(true);
            audioSource.Play();

            GameManager.Instance.AddSpell(spellToGet);
        }
	}

}
