using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCtrl : MonoBehaviour {
    public delegate void EventArgs();
    public static event EventArgs EventBeginClickedPlayer;
    public static event EventArgs EventEndClickedPlayer;

    public static bool isClick = false;

    [HideInInspector]
    public CircleCollider2D trigger;
    DontSpawnResionCtrl dontSpawnResion;
    TrailRenderer trail;

    private void Awake()
    {
        trigger = GetComponent<CircleCollider2D>();
        dontSpawnResion = GetComponent<DontSpawnResionCtrl>();
        //trail = GetComponent<TrailRenderer>();
        transform.localScale = Vector3.zero;
        //trail.enabled = false;
    }

    private void Start()
    {

    }

    public IEnumerator CoGameStart()
    {
        dontSpawnResion.enabled = false;
        transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 1f);
        yield return new WaitForSeconds(1f);
        //trail.Clear();
        //trail.enabled = true;
    }

    private void FixedUpdate()
    {
        if(GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            if (isClick)
            {
                Vector3 pos = Vector3.zero;
                if (Input.touchCount > 0)
                {
                    pos = Input.GetTouch(0).position;
                }
#if UNITY_EDITOR
                pos = Input.mousePosition;
#endif
                pos.z = 10;

                //Debug.Log(Camera.main.ScreenToWorldPoint(mousePos));
                if(pos != Vector3.zero)
                    transform.position = Camera.main.ScreenToWorldPoint(pos);
            }
        }
    }
    
    public void Die()
    {
        StartCoroutine(GameManager.Instance.CoGameLose());
    }

    private void OnMouseDown()
    {
        isClick = true;
        if (EventBeginClickedPlayer != null)
            EventBeginClickedPlayer();
    }

    private void OnMouseUp()
    {
        isClick = false;

        if (EventEndClickedPlayer != null)
            EventEndClickedPlayer();

        if(GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            //Die();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            if (collision.CompareTag("Obstacle"))
            {
                StartCoroutine(GameManager.Instance.CoGameLose());
            }
        }
    }
}
