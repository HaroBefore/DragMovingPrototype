using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class DontSpawnResionCtrl : MonoBehaviour {
    
    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            //obstacle.isNeedRespawn = true;
        }
    }
}
