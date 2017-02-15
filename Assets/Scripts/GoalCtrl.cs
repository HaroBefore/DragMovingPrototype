using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MadLevelManager;

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
        transform.DOScale(Vector3.one, 1f);
        yield return new WaitForSeconds(0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_1", true);
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_2", true);
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_3", true);
            MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);

            StartCoroutine(GameManager.Instance.CoGameWin());
        }
    }
}
