using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Platformer.Mechanics;

public class RoomEnemySpawner : RoomSpawner
{

    public override void Start()
    {
        base.Start();
        
        GetComponentInChildren<Enemy>().uID = uID;
        if (!GameManager.Instance.CanSpawn(uID))
        {
            transform.GameObject().SetActive(false);
        }
    }

}
