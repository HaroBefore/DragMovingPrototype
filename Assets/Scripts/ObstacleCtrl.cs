using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public struct WayPoint
{
    public Vector3 scale;
    public Vector3 pos;
}

public class ObstacleCtrl : MonoBehaviour {

    new SpriteRenderer renderer;
    public bool isNeedRespawn = false;

    WayPoint[] wayPoints;
    public int wayPointCnt = 2;
    public int wayPointIdx = 0;

    Tweener moveTween;
    Tweener scaleTween;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
    }
    // Use this for initialization
    void Start () {
        //StartCoroutine(CoInit());
        StartCoroutine(CoClickCheck());
    }

    public IEnumerator CoInit()
    {
        Vector3 scale = Vector3.one * Random.Range(0.5f, 1.5f);
        transform.localScale = scale;
        yield return null;

        if (isNeedRespawn)
        {
            SpawnManager.spawnCnt--;
            Debug.Log("삭제");
            Destroy(this.gameObject, 0.1f);
            yield break;
        }

        wayPoints = new WayPoint[wayPointCnt];
        wayPoints[0].pos = transform.position;
        wayPoints[0].scale = scale;
        for (int i = 1; i < wayPointCnt; i++)
        {
            float x = transform.position.x;
            float y = transform.position.y;
            float tmpX = x + Random.Range(-4f, 4f);
            float tmpY = y + Random.Range(-5f, 5f);
            x = tmpX < -5f ? -5f
                : tmpX > 5f ? 5f
                : tmpX;
            y = tmpY < -6f ? -6f
                : tmpY > 8.5f ? 8.5f
                : tmpY;

            wayPoints[i].pos = new Vector3(x, y, 0f);
            wayPoints[i].scale = Vector3.one * Random.Range(0.5f, 1.5f);

            transform.localScale = wayPoints[i].scale;
            transform.position = wayPoints[i].pos;
            yield return null;
            if(isNeedRespawn)
            {
                transform.position = wayPoints[0].pos;
                transform.localScale = wayPoints[0].scale;
                i--;
                isNeedRespawn = false;
                Debug.Log("경로 재설정");
            }
        }

        transform.localScale = Vector3.zero;

        renderer.enabled = true;
        GameManager.Instance.EventGameStart += OnGameStart;
    }

    public void OnDestroy()
    {
        GameManager.Instance.EventGameStart -= OnGameStart;
    }

    public void OnGameStart()
    {
        MoveNextWayPoint(out moveTween, out scaleTween);
    }

    public void MoveNextWayPoint(out Tweener move, out Tweener scale)
    {
        wayPointIdx = wayPointIdx + 1 == wayPointCnt ? 0 : wayPointIdx + 1;
        move = MoveWayPoint(wayPointIdx);
        scale = ScaleWayPoint(wayPointIdx);
    }

    public Tweener MoveWayPoint(int idx)
    {
        if (idx >= wayPointCnt)
            return null;
        return transform.DOMove(wayPoints[idx].pos, Random.Range(0.5f,2f)).SetSpeedBased(true).SetAutoKill(false).SetEase(Ease.Linear);
    }

    public Tweener ScaleWayPoint(int idx)
    {
        if (idx >= wayPointCnt)
            return null;
        return transform.DOScale(wayPoints[idx].scale, 1f).SetAutoKill(false);
    }

    IEnumerator CoClickCheck()
    {
        yield return new WaitForSeconds(0.2f);
        if(moveTween != null)
        {
            if (PlayerCtrl.isClick)
            {
                moveTween.timeScale = scaleTween.timeScale = 0.05f;
            }
            else
            {
                moveTween.timeScale = scaleTween.timeScale = 1f;
            }
        }
        StartCoroutine(CoClickCheck());
    }

	// Update is called once per frame
	void Update () {
		if(moveTween != null)
        {
            if (moveTween.IsComplete())
            {
                moveTween.Kill();
                scaleTween.Kill();
                moveTween = scaleTween = null;
                MoveNextWayPoint(out moveTween, out scaleTween);
            }
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(GameManager.Instance.CoGameLose());
            }
        }
    }
}
