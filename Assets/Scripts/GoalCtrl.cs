using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MadLevelManager;
using TMPro;

public class GoalCtrl : MonoBehaviour {
    [HideInInspector]
    public CircleCollider2D trigger;

    public bool endLevel;
    public string level;

    public bool isTimeGoal;
    public TextMeshProUGUI timeText;
    public float timeLimit = 10f;

    private void Awake()
    {
        trigger = GetComponent<CircleCollider2D>();
        transform.localScale = Vector3.zero;
        if (isTimeGoal)
        {
            timeText.text = string.Format("{0:0.0}", timeLimit);
        }
        else
            timeText.text = "";
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

    public void BeginTimer()
    {
        StartCoroutine(CoTimer());
    }

    IEnumerator CoTimer()
    {
        while(timeLimit > 0f)
        {
            if(PlayerCtrl.isClick)
            {
                yield return new WaitForSeconds(0.01f);
                timeLimit -= 0.01f;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                timeLimit -= 0.1f;
            }
            timeText.text = string.Format("{0:0.0}", timeLimit);
        }
        timeLimit = 0f;
        timeText.text = string.Format("{0:0.0}", timeLimit);

        StartCoroutine(GameManager.Instance.CoGameLose());
    }
}
