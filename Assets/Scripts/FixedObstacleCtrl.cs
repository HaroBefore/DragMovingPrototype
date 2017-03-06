using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedObstacleCtrl : Obstacle
{
    public Vector2 destScale;
    bool isBeginToDest = true;

    Vector3 destScaleVec3;

    new private void Start()
    {
        GameManager.Instance.EventGameStart += OnGameStart;
        destScaleVec3 = destScale;
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

        StartCoroutine(CoPlay());
    }

    IEnumerator CoPlay()
    {
        yield return null;
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
        Vector3 scale = isBeginToDest ? destScaleVec3 : beginScale;
        var tween = transform.DOScale(scale, changeScaleSpeed)
            .SetEase(Ease.Linear)
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
            scaleTweener.timeScale = 1f;
        if (rotTweener != null)
            rotTweener.timeScale = originAngleTimeScale = normalAnglePerSec;

        base.UpdateTimeScale();

        if (PlayerCtrl.isClick)
        {
            if (scaleTweener != null)
                scaleTweener.timeScale = 1f * timeScaleMultiply;
            if (rotTweener != null)
                rotTweener.timeScale = originAngleTimeScale * timeScaleMultiply;
        }
    }
}
