using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HeavyDutyInspector;

public class PlayerCtrl : MonoBehaviour
{
    public delegate void EventArgs();
    public static event EventArgs EventBeginClickedPlayer;  //플레이어 클릭 시작시 이벤트
    public static event EventArgs EventEndClickedPlayer;    //플레이어 클릭 종료시 이벤트

    public static bool isClick = false;

    [Comment("True 시 터치 시작 후 때는순간 죽음")]
    public bool isDieIfRelaseTouch = false;

    Vector3 offset;
    Touch touch;

    Vector3 originScale;

    TrailRenderer trail;

    private void Awake()
    {


        //trail = GetComponent<TrailRenderer>();
        originScale = transform.localScale;
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
        if (GameManager.Instance.useMultiPlayer == false)
        {
            if (CompareTag("CustomPlayer"))
                Destroy(this.gameObject);
        }
        else
        {
            if (CompareTag("MainPlayer"))
                Destroy(this.gameObject);
        }

        //Tweener tweener = transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 1f);
        yield return transform.DOScale(Vector3.one * 0.3f, 1.5f).WaitForCompletion();
        //trail.Clear();
        //trail.enabled = true;

    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            Vector3 pos = Vector3.zero;
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    BeginClick();
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    EndClick();
                }

                if (isClick)
                {
                    pos = Input.GetTouch(0).position;
                    pos.z = 10;
                }
            }
#if UNITY_EDITOR
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    BeginClick();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    EndClick();
                }

                if (isClick)
                {
                    pos = Input.mousePosition;
                    pos.z = 10;
                    Debug.Log("Clicking");
                }
            }
#endif

            //Debug.Log(Camera.main.ScreenToWorldPoint(mousePos));
            if (pos != Vector3.zero)
            {
                Vector3 movePos = Camera.main.ScreenToWorldPoint(pos);
                transform.position = movePos + offset;// + offset;
                //if ((movePos - transform.position).sqrMagnitude < 2.5f * 2.5f)
                //    transform.position = movePos;
            }
        }
    }

    public void BeginClick()
    {
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        offset = transform.position - touchPos;
        isClick = true;
        if (EventBeginClickedPlayer != null)
            EventBeginClickedPlayer();
    }

    public void EndClick()
    {
        isClick = false;
        offset = Vector3.zero;
        if (EventEndClickedPlayer != null)
            EventEndClickedPlayer();

        if (isDieIfRelaseTouch)
        {
            if (GameManager.Instance.gameState == eGameState.gamePlaying)
            {
                Die();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Die");
        if (GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            if (collision.CompareTag("Obstacle"))
            {
                StartCoroutine(GameManager.Instance.CoGameLose());
            }
        }
    }

    public void Die()
    {
        StartCoroutine(GameManager.Instance.CoGameLose());
    }
}
