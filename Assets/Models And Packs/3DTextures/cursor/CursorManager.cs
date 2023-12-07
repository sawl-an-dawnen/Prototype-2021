using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D[] cursorTexture;

    private int currFrame;
    private float frameTimer;
    [SerializeField] private int frameCount;
    [SerializeField] private float frameRate;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture[0], new Vector2(10, 10), CursorMode.Auto);
    }

    private void Update()
    {
        frameTimer -= Time.deltaTime;
        if(frameTimer <= 0f)
        {
            frameTimer += frameRate;
            currFrame = (currFrame + 1) % frameCount;
            Cursor.SetCursor(cursorTexture[currFrame], new Vector2(10, 10), CursorMode.Auto);
        }
    }

}
