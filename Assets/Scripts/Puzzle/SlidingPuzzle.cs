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
    private Camera cam;
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
    float winCheck;
    void Start()
    {
        cam = Camera.main;
        T1.Set(-20, 50, -1);
        T2.Set(30, 50, -1);
        T3.Set(80, 50, -1);
        T4.Set(-20, 0, -1);
        T5.Set(30, 0, -1);
        T6.Set(80, 0, -1);
        T7.Set(-20, -50, -1);
        T8.Set(30, -50, -1);
        //T1.Set((float)-20.24, (float)50.24, (float)-1.00);
        //T2.Set((float)30.50, (float)50.00, (float)-1.00);
        //T3.Set((float)80.44, (float)50.08, (float)-1.00);
        //T4.Set((float)-20.20, (float)0.20, (float)-1.00);
        //T5.Set((float)30, (float)0, (float)-1);
        //T6.Set((float)79.69, (float)0.31, (float)-1.00);
        //T7.Set((float)-20.28, (float)-49.72, (float)-1.00);
        //T8.Set((float)29.91, (float)-49.31, (float)-1.00);
        winCheck = 0f;
        audioSource = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        winCheck += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            Ray pointer = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pointer.origin, pointer.direction);
            if (hit && !Won)
            {
                if (Vector2.Distance(empty.position,hit.transform.position) < 60)
                {
                    audioSource.Stop();
                    audioSource.Play();
                    Vector3 lastEmptySpace = empty.position;
                    TileMovement thisTile = hit.transform.GetComponent<TileMovement>();
                    empty.position = thisTile.targetPosition;
                    thisTile.targetPosition = lastEmptySpace;
                    
                }
            }
        }
        while (winCheck >= 2)
        {
            CheckFinished();
            winCheck -= 2;
        }
    }

    void CheckFinished()
    {
        //Debug.Log("Tile1: " + Tile1.position);
        //Debug.Log("T1: " + T1);
        //Debug.Log("Tile2: " + Tile2.position);
        //Debug.Log("T2: " + T2);
        //Debug.Log("Tile3: " + Tile3.position);
        //Debug.Log("T3: " + T3);
        //Debug.Log("Tile4: " + Tile4.position);
        //Debug.Log("T4: " + T4);
        //Debug.Log("Tile5: " + Tile5.position);
        //Debug.Log("T5: " + T5);
        //Debug.Log("Tile6: " + Tile6.position);
        //Debug.Log("T6: " + T6);
        //Debug.Log("Tile7: " + Tile7.position);
        //Debug.Log("T7: " + T7);
        //Debug.Log("Tile8: " + Tile8.position);
        //Debug.Log("T8: " + T8);
        //Debug.Log(ComparePositions(Tile1.position, T1));
        //Debug.Log(ComparePositions(Tile2.position, T2));
        //Debug.Log(ComparePositions(Tile3.position, T3));
        //Debug.Log(ComparePositions(Tile4.position, T4));
        //Debug.Log(ComparePositions(Tile5.position, T5));
        //Debug.Log(ComparePositions(Tile6.position, T6));
        //Debug.Log(ComparePositions(Tile7.position, T7));
        //Debug.Log(ComparePositions(Tile8.position, T8));
        if (ComparePositions(Tile1.position, T1) && ComparePositions(Tile2.position, T2) && ComparePositions(Tile3.position, T3) && ComparePositions(Tile4.position, T4) && ComparePositions(Tile5.position, T5) && ComparePositions(Tile6.position, T6) && ComparePositions(Tile7.position, T7) && ComparePositions(Tile8.position, T8))
        {
            //audioSource.Stop();
            //audioSource.PlayOneShot(puzzleComplete);
            Debug.Log("Win");
            winUI.SetActive(true);  
            Won = true;
        }
    }

    bool ComparePositions(Vector3 A, Vector3 B)
    {
        if (Mathf.Round(A.x) == B.x && Mathf.Round(A.y) == B.y)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
