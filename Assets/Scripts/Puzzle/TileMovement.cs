using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMovement : MonoBehaviour
{
    public Vector3 targetPosition;
    [SerializeField] private Transform empty;
    private bool moving = false;
    private bool cheating = false;
    private SlidingPuzzle sp;

    void Start()
    {
        targetPosition = transform.position;
        sp = GameObject.FindGameObjectWithTag("SlidingPuzzle").GetComponent<SlidingPuzzle>();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            cheating = true;
        }
        if (!cheating)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.05f);
        }
        if (Vector3.Distance(transform.position, targetPosition) < 1 && moving)
        {
            moving = false;
        }
    }

    public void checkvalid()
    {
        if (Vector3.Distance(transform.position, empty.position) < 370 && !moving)
        {
            sp.letTheSoundPlay();
            moving = true;
            Vector3 target = empty.position;
            empty.position = transform.position;
            targetPosition = target;
        }
    }

}
