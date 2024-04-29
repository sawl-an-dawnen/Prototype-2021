using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;
using UnityEngine.Events;


public class MainMenuButtons : MonoBehaviour
{   
    Color originalColor = new Color (204,124,58,255);
    Color mouseOverColor = new Color (234,171,118,255);
    Color mouseClickColor = new Color (248,212,182,255);

    //public UnityEvent customCallback;



    MeshRenderer m_Renderer;
    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        //customCallback = new UnityEvent();
    }

    void OnMouseOver()
    {
        Debug.Log("Mouse is over GameObject.");
        //m_Renderer.material.color = mouseOverColor;
        m_Renderer.material.SetColor("_BaseColor", mouseOverColor); 
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse is no longer on GameObject.");
        //m_Renderer.material.color = originalColor;
        m_Renderer.material.SetColor("_BaseColor", originalColor);
    }

    private void OnMouseUpAsButton() {
        Debug.Log("Mouse Clicked");
        //m_Renderer.material.color = mouseClickColor;
        m_Renderer.material.SetColor("_BaseColor", mouseClickColor);
        //customCallback.Invoke();
    }
}
