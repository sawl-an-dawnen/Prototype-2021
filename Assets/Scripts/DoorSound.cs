using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class DoorSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var gm = GameManager.Instance;
        var entryExists = gm.PlayDoorSound.TryGetValue(gm.SceneName, out bool playSound);
        if (entryExists && playSound)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
