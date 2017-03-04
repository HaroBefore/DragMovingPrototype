using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HeavyDutyInspector;

public class PlayerCtrl : MonoBehaviour {
    public delegate void EventArgs();
    public static event EventArgs EventBeginClickedPlayer;  //플레이어 클릭 시작시 이벤트
    public static event EventArgs EventEndClickedPlayer;    //플레이어 클릭 종료시 이벤트

    public static bool isClick = false;

    [Comment("True 시 터치 시작 후 때는순간 죽음")]
    public bool isDieIfRelaseTouch = false;

    Vector3 offset;
    Touch touch;

    TrailRenderer trail;

    private void Awake()
    {
        //trail = GetComponent<TrailRenderer>();
        transform.localScale = Vector3.zero;
        //trail.enabled = false;

        isClick = false;
        offset = Vector3.zero;
    }

    private void Start()
    {

    }

    public IEnumerator CoGameStart()
    {
        //Tweener tweener = transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 1f);
        yield return transform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 1f).WaitForCompletion();
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
                if (pos != Vector3.zero)
                {
                    Vector3 movePos = Camera.main.ScreenToWorldPoint(pos);
                    if ((movePos - transform.position).sqrMagnitude < 2.5f * 2.5f)
                        transform.position = movePos;
                }
            }
        }
    }
    
    public void Die()
    {
        StartCoroutine(GameManager.Instance.CoGameLose());
    }

    private void OnMouseDown()
    {
        offset = transform.position;
        isClick = true;
        if (EventBeginClickedPlayer != null)
            EventBeginClickedPlayer();
    }


    private void OnMouseUp()
    {
        isClick = false;
        offset = Vector3.zero;
        if (EventEndClickedPlayer != null)
            EventEndClickedPlayer();

        if(isDieIfRelaseTouch)
        {
            if (GameManager.Instance.gameState == eGameState.gamePlaying)
            {
                Die();   
            }
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
