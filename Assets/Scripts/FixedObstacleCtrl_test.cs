using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavyDutyInspector;

public enum Option
{
    Default =0,
    Phase = 1,
    Flip = 2
}


public class FixedObstacleCtrl_test : Obstacle
{
    [Header("회전")]
    [Space]
    public bool isRightRot = true;
    public float rotateOnAwakeDelay;
    public Option rotateOption;
    public AnimationCurve rotateEaseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [HideConditional(true, "rotateOption", (int)Option.Phase)]
    public float phaseRot;
    [HideConditional(true, "rotateOption", (int)Option.Phase)]
    public float phaseDuration;
    private bool isPhase;
    [HideConditional(true, "rotateOption", (int)Option.Flip)]
    public float flipRot;
    [HideConditional(true, "rotateOption", (int)Option.Flip)]
    public float flipDelay;
    [HideConditional(true, "rotateOption", (int)Option.Flip)]
    [Space]
    [Space]

    [Header("크기변환")]
    [Space]
    public Vector2 destScale = Vector3.one;
    public AnimationCurve scaleEaseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    bool isBeginToDest = true;

    Vector3 destScaleVec3;
    public float delayChangeScale = 0f;
    public float scaleOnAwakeDelay;

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

        rotTweener = ToRotate();

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
                scaleTweener.Play();
            }
            if (rotTweener != null)
                rotTweener.Play();
        }
    }
    public Tweener ToRotate()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        float _flipDelay;
        float _phaseDuration;
        Tweener tween;
        switch (rotateOption)
        {
            case Option.Phase:
                {
                    rot = isRightRot ? rot + new Vector3(0f, 0f, phaseRot) : rot - new Vector3(0f, 0f, phaseRot);
                    tween = transform.DORotate(transform.rotation.eulerAngles + rot*-1, 1f, RotateMode.LocalAxisAdd)
                        .SetEase(rotateEaseCurve)
                        .SetSpeedBased(true)
                        .Pause()
                        .SetDelay(_phaseDuration = rotateOnAwakeDelay > 0 ? (phaseDuration + rotateOnAwakeDelay)* normalAnglePerSec : phaseDuration * normalAnglePerSec)
                        .OnComplete(() =>
                        {
                            rotateOnAwakeDelay = 0;
                            rotTweener = ToRotate();
                            UpdateTimeScale();
                            onPause.Invoke();
                            rotTweener.Play();
                        });
                        isPhase = true;
                }
                break;
            case Option.Flip:
                {
                    rot = isRightRot ? rot + new Vector3(0f, 0f, flipRot) : rot - new Vector3(0f, 0f, flipRot);
                    tween = transform.DORotate(transform.rotation.eulerAngles + rot* -1, 1f, RotateMode.LocalAxisAdd)
                        .SetEase(rotateEaseCurve)
                        .SetSpeedBased(true)
                        .SetDelay(_flipDelay = rotateOnAwakeDelay>0 ? (flipDelay + rotateOnAwakeDelay) * normalAnglePerSec : flipDelay * normalAnglePerSec)
                        .OnComplete(() =>
                        {
                            rotateOnAwakeDelay = 0;
                            isRightRot = !isRightRot;
                            rotTweener = ToRotate();
                            UpdateTimeScale();
                            rotTweener.Play();
                        });

                }
                break;
            default:
                rot = isRightRot ? rot - new Vector3(0f, 0f, 360f) : rot + new Vector3(0f, 0f, 360f);
                tween = transform.DORotate(transform.rotation.eulerAngles + rot, 1f, RotateMode.LocalAxisAdd)
                        .SetEase(rotateEaseCurve)
                        .SetSpeedBased(true)
                        .SetDelay(rotateOnAwakeDelay * normalAnglePerSec)
                        .SetLoops(-1)
                        .Pause();
                break;
        }

        return tween;
    }
    public Tweener ToScale()
    {

        float delay = isOnSlow == false ? delayChangeScale :
            PlayerCtrl.isClick ? delayChangeScale * 5f : delayChangeScale;
        Vector3 scale = isBeginToDest ? destScaleVec3 : beginScale;
        Tweener tween = transform.DOScale(scale, changeScaleSpeed)
            .SetEase(scaleEaseCurve)
            .Pause()
            .SetDelay(delay = scaleOnAwakeDelay > 0 ? delay+ scaleOnAwakeDelay : delay)
            .OnComplete(() =>
            {
                scaleOnAwakeDelay = 0;
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
