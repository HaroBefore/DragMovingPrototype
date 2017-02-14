using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZoneCtrl : MonoBehaviour {

    new Rigidbody2D rigidbody;
    Collider2D trigger;

    public float timeScaleMultiply = 1f;
    public bool isActiveOnlyIfClicked = false;

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
        if (isActiveOnlyIfClicked)
        {
            Debug.Log("active");
            if (!PlayerCtrl.isClick)
                return;
        }
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Enter");
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            obstacle.timeScaleMultiply = timeScaleMultiply;
            if (isActiveOnlyIfClicked)
                obstacle.isOnTimeZone = true;
            obstacle.UpdateTimeScale();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(isActiveOnlyIfClicked)
        {
            if (!PlayerCtrl.isClick)
                return;
            
        }
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Exit");
            ObstacleCtrl obstacle = collision.GetComponent<ObstacleCtrl>();
            obstacle.timeScaleMultiply = 1f;

            if (isActiveOnlyIfClicked)
                obstacle.isOnTimeZone = false;
            obstacle.UpdateTimeScale();
        }
    }
}
