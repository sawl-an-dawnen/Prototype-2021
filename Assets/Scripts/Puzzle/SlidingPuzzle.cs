using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzle : MonoBehaviour
{
    [SerializeField] private Transform empty;
    [SerializeField] private GameObject winUI;
    [SerializeField] private Transform Tile1;
    [SerializeField] private Transform Tile2;
    [SerializeField] private Transform Tile3;
    [SerializeField] private Transform Tile4;
    [SerializeField] private Transform Tile5;
    [SerializeField] private Transform Tile6;
    [SerializeField] private Transform Tile7;
    [SerializeField] private Transform Tile8;
    private AudioSource audioSource;
    public AudioClip puzzleComplete;
    private Vector3 T1;
    private Vector3 T2;
    private Vector3 T3;
    private Vector3 T4;
    private Vector3 T5;
    private Vector3 T6;
    private Vector3 T7;
    private Vector3 T8;
    private bool Won = false;
    private bool exiting = false;
    float winCheck;
    public GameObject panel;
    public InventoryItem rewardItem;

    void Start()
    {
        T1.Set(820, 870, -1);
        T2.Set(1160, 870, -1);
        T3.Set(1500, 870, -1);
        T4.Set(820, 530, -1);
        T5.Set(1160, 530, -1);
        T6.Set(1500, 530, -1);
        T7.Set(820, 190, -1);
        T8.Set(1160, 190, -1);
        winCheck = 0f;
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    
    void Update()
    {
        if (panel.activeSelf && !Won && !exiting)
		{
			Pause();
		}
        else if (!exiting)
        {
            exiting = true;
            StartCoroutine(returnToGame(2f));
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Tile1.position = T1;
            Tile2.position = T2;
            Tile3.position = T3;
            Tile4.position = T4;
            Tile5.position = T5;
            Tile6.position = T6;
            Tile7.position = T7;
            Tile8.position = T8;
            Debug.Log("Cheater found");
        }

        winCheck += Time.unscaledDeltaTime;
        while (winCheck >= 2)
        {
            CheckFinished();
            winCheck -= 2;
        }
    }

    void CheckFinished()
    {
        Debug.Log(Vector3.Distance(Tile1.position, T1));
        Debug.Log(Vector3.Distance(Tile2.position, T2));
        Debug.Log(Vector3.Distance(Tile3.position, T3));
        Debug.Log(Vector3.Distance(Tile4.position, T4));
        Debug.Log(Vector3.Distance(Tile5.position, T5));
        Debug.Log(Vector3.Distance(Tile6.position, T6));
        Debug.Log(Vector3.Distance(Tile7.position, T7));
        Debug.Log(Vector3.Distance(Tile8.position, T8));

        if (Vector3.Distance(Tile1.position, T1) < 2 && Vector3.Distance(Tile2.position, T2) < 2 && Vector3.Distance(Tile3.position, T3) < 2 && Vector3.Distance(Tile4.position, T4) < 2 && Vector3.Distance(Tile5.position, T5) < 2 && Vector3.Distance(Tile6.position, T6) < 2 && Vector3.Distance(Tile7.position, T7) < 2 && Vector3.Distance(Tile8.position, T8) < 2 && !Won)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(puzzleComplete);
            Debug.Log("Win");
            winUI.SetActive(true);  
            Won = true;
            GameObject.Find("InventoryManager").GetComponent<InventoryManager>().AddItem(rewardItem, false);
        }
    }

    public void letTheSoundPlay()
    {
        audioSource.Stop();
        audioSource.Play();
    }

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

    public IEnumerator returnToGame(float f)
    {
        yield return new WaitForSecondsRealtime(f);
        Resume();
    }
}
