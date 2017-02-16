using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZoneCtrl : MonoBehaviour {

    new Rigidbody2D rigidbody;
    Collider2D trigger;

    public float timeScaleMultiply = 1f;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        trigger = GetComponent<Collider2D>();
    }

    void Start () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
           {
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            if(obstacle != null)
            {
                obstacle.zoneTimeScaleQueue.Enqueue(timeScaleMultiply);
                obstacle.UpdateTimeScale();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            if(obstacle != null)
            {
                obstacle.zoneTimeScaleQueue.Dequeue();
                obstacle.UpdateTimeScale();
            }
        }
    }
}
