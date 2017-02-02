using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCtrl : MonoBehaviour {

    public static bool isClick = false;

    [HideInInspector]
    public CircleCollider2D trigger;
    DontSpawnResionCtrl dontSpawnResion;
    TrailRenderer trail;

    private void Awake()
    {
        trigger = GetComponent<CircleCollider2D>();
        dontSpawnResion = GetComponent<DontSpawnResionCtrl>();
        trail = GetComponent<TrailRenderer>();
        transform.localScale = Vector3.zero;
        trail.enabled = false;
    }

    private void Start()
    {

    }

    public IEnumerator CoGameStart()
    {
        dontSpawnResion.enabled = false;
        transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 1f);
        yield return new WaitForSeconds(1f);
        trail.Clear();
        trail.enabled = true;
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
    }

    private void OnMouseUp()
    {
        isClick = false;

        if(GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            Die();
        }
    }
}
