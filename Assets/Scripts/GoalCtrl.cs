using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GoalCtrl : MonoBehaviour {
    [HideInInspector]
    public CircleCollider2D trigger;

    public bool endLevel;
    public string level;

    private void Awake()
    {
        trigger = GetComponent<CircleCollider2D>();
        transform.localScale = Vector3.zero;
    }

    public IEnumerator CoGameStart()
    {
        //transform.position = new Vector3(UnityEngine.Random.Range(-4f, 4f), 8f, 0f);
        transform.DOScale(Vector3.one, 1f);
        yield return new WaitForSeconds(0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(endLevel)
            {
                StartCoroutine(GameManager.Instance.CoGameWin());
            }
            else
            {
                SceneManager.LoadScene(level);
            }            
        }
    }
}
