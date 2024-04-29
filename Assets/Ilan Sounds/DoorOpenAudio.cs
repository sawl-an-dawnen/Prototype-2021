using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;
    //private BoxCollider bc;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<Animator>();
        //bc = GetComponent<BoxCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.enabled = true;
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.enabled = false;
            audioSource.Stop();
        }

    }
}
