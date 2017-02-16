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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        PlayerCtrl.EventBeginClickedPlayer += OnBeginClickedPlayer;
        PlayerCtrl.EventEndClickedPlayer += OnEndClickedPlayer;
    }

    private void OnDisable()
    {
        PlayerCtrl.EventBeginClickedPlayer -= OnBeginClickedPlayer;
        PlayerCtrl.EventEndClickedPlayer -= OnEndClickedPlayer;
    }

    private void OnBeginClickedPlayer()
    {
        trigger.enabled = false;
        trigger.enabled = true;
    }

    private void OnEndClickedPlayer()
    {
        trigger.enabled = false;
        trigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
           {
            Debug.Log("Enter");
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            if(obstacle != null)
            {
                //obstacle.timeScaleMultiply = timeScaleMultiply;
                obstacle.zoneTimeScaleQueue.Enqueue(timeScaleMultiply);
                Debug.Log("Enqueue : " + timeScaleMultiply);
                //obstacle.isOnTimeZone = true;
                obstacle.UpdateTimeScale();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Exit");
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            if(obstacle != null)
            {
                //obstacle.timeScaleMultiply = 1f;
                float t = obstacle.zoneTimeScaleQueue.Dequeue();
                Debug.Log("Dequeue : " + t);
                //obstacle.isOnTimeZone = false;
                obstacle.UpdateTimeScale();
            }
        }
    }
}
