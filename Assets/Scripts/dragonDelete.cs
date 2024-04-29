using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonDelete : MonoBehaviour
{
    public GameObject miniPainting;

    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.Instance.getSmallDragonStatus())
        {
            Destroy(miniPainting);
        }
    }

}
