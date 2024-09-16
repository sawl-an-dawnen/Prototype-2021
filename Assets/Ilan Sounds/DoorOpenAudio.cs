using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenAudio : MonoBehaviour
{
    public float audioDelay;
    private AudioSource audioSource;
    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayDelayed(audioDelay);

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    
}
