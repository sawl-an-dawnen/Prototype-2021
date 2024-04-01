using UnityEngine;
using UnityEngine.SceneManagement;

public class TmpChangeScene : MonoBehaviour
{
    public GameObject activeCue;
    public GameObject SlidingCanvas;

    private bool playerInTriggerZone = false;
    private bool puzzleActivated = false;


    private void Start()
    {
        SlidingCanvas.SetActive(false);
        activeCue.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Sliding in");
        activeCue.SetActive(true);
        if (other.CompareTag("Player") && !puzzleActivated)
        {
            // Player entered the trigger zone
            playerInTriggerZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activeCue.SetActive(false);
        if (other.CompareTag("Player"))
        {
            // Player exited the trigger zone
            playerInTriggerZone = false;
        }
    }

    private void Update()
    {
        // Check for Space key press and player in trigger zone
        if (playerInTriggerZone && Input.GetKeyDown(KeyCode.Space) && !puzzleActivated)
        {
            ActivatePuzzle();
        }
    }

private void ActivatePuzzle()
    {
        SlidingCanvas.SetActive(true);
        puzzleActivated = true;
    }

}
