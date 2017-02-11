using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZoneCtrl : MonoBehaviour {

    new Rigidbody2D rigidbody;

    public float timeScaleMultiply = 1f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Obstacle"))
        {
            Debug.Log("Enter");
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            obstacle.timeScaleMultiply = timeScaleMultiply;
            obstacle.UpdateTimeScale();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Obstacle"))
        {
            Debug.Log("Exit");
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            obstacle.timeScaleMultiply = 1f;
            obstacle.UpdateTimeScale();
        }
    }
}
