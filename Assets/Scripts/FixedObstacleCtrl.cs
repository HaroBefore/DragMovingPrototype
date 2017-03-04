using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedObstacleCtrl : Obstacle
{
    public Vector3 destScale;
    bool isBeginToDest = true;

    new private void Start()
    {
        GameManager.Instance.EventGameStart += OnGameStart;
    }

    public void OnDestroy()
    {
        GameManager.Instance.EventGameStart -= OnGameStart;
    }

    private void OnEnable()
    {
        PlayerCtrl.EventBeginClickedPlayer += UpdateTimeScale;
        PlayerCtrl.EventEndClickedPlayer += UpdateTimeScale;
    }

    private void OnDisable()
    {
        PlayerCtrl.EventBeginClickedPlayer -= UpdateTimeScale;
        PlayerCtrl.EventEndClickedPlayer += UpdateTimeScale;
    }

    public override void BeginObstacle()
    {
        rotTweener = transform.DORotate(transform.rotation.eulerAngles + new Vector3(0f, 0f, 360f), 1f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .SetLoops(-1)
            .Pause();
        rotTweener.timeScale = originAngleTimeScale = normalAnglePerSec;

        scaleTweener = ToScale();
        scaleTweener.timeScale = originChangeScaleSpeed = changeScaleSpeed;

        StartCoroutine(CoPlay());
    }

    IEnumerator CoPlay()
    {
        yield return null;
        //yield return new WaitForSeconds(0.5f);
        if (isBeginToPlay)
        {
            isPlayingTween = true;

            if (scaleTweener != null)
            {
                Debug.Log("Scale");
                scaleTweener.Play();
            }
            if (rotTweener != null)
                rotTweener.Play();
        }
    }

    public Tweener ToScale()
    {
        Vector3 scale = isBeginToDest ? destScale : beginScale;
        var tween = transform.DOScale(scale, 1f)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .Pause()
            .OnComplete(() =>
            {
                isBeginToDest = !isBeginToDest;
                scaleTweener = ToScale();
                UpdateTimeScale();
                scaleTweener.Play();
            });
        return tween;
    }

    public override void UpdateTimeScale()
    {
        if (zoneTimeScaleQueue.Count == 0)
            timeScaleMultiply = 1f;

        if (scaleTweener != null)
            scaleTweener.timeScale = originChangeScaleSpeed = changeScaleSpeed;
        if (rotTweener != null)
            rotTweener.timeScale = originAngleTimeScale = normalAnglePerSec;

        base.UpdateTimeScale();

        if (PlayerCtrl.isClick)
        {
            if (scaleTweener != null)
                scaleTweener.timeScale = originChangeScaleSpeed * timeScaleMultiply;
            if (rotTweener != null)
                rotTweener.timeScale = originAngleTimeScale * timeScaleMultiply;
        }
    }
}
