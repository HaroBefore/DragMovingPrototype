using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavyDutyInspector;
using DG.Tweening;
using UnityEngine.Events;
using System;

//이벤트
public abstract class Obstacle : MonoBehaviour {
    public bool isBeginToPlay = true;

    protected Tweener scaleTweener;

    protected Tweener rotTweener;

    protected Vector3 beginScale;

    public float changeScaleSpeed;
    public float normalAnglePerSec;

    [HideInInspector]
    public Queue<float> zoneTimeScaleQueue;

    [Comment("true 시 레벨 시작시 스케일 애니메이션을 실행합니다")]
    public bool isBeginAnimateScale = false;

    public float timeScaleMultiply = 1f;
   // [Readonly]
    public bool isOnSlow = false;

    protected bool isPlayingTween = false;

    protected float originAngleTimeScale;
    protected float originChangeScaleSpeed;

    public UnityEvent onPause;
    public UnityEvent onResume;

    protected void Awake()
    {
        zoneTimeScaleQueue = new Queue<float>();
        beginScale = transform.localScale;
    }

    // Use this for initialization
    protected void Start () {
        if (isBeginAnimateScale)
        {
            transform.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update () {

	}

    public void OnGameStart()
    {
        if (isBeginAnimateScale)
            transform.DOScale(beginScale, 0.1f);

        BeginObstacle();
    }

    public abstract void BeginObstacle();

    public virtual void OnPauseObstacle()
    {
        if (!isPlayingTween)
            return;

        isPlayingTween = false;

        if (rotTweener != null)
        {
            if (rotTweener.IsPlaying())
                rotTweener.Pause();
        }

        if (scaleTweener != null)
        {
            if (scaleTweener.IsPlaying())
                scaleTweener.Pause();
        }

        if (onPause != null)
        {
            onPause.Invoke();
        }
    }

    public virtual void OnResumeObstacle()
    {
        if (isPlayingTween)
            return;

        isPlayingTween = true;

        if (rotTweener != null)
        {
            if (!rotTweener.IsPlaying())
                rotTweener.Play();
        }

        if (scaleTweener != null)
        {
            if (!scaleTweener.IsPlaying())
                scaleTweener.Play();
        }

        if (onResume != null)
        {
            onResume.Invoke();
        }
    }

    public Tweener MakeRotTweener(bool isRightRot)
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot = isRightRot ? rot - new Vector3(0f, 0f, 360f) : rot + new Vector3(0f, 0f, 360f);

        Tweener r = transform.DORotate(rot, 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .SetLoops(-1)
            .Pause();
        return r;
    }

    public Tweener MakeScaleTweener(Vector3 scale)
    {
        Tweener s = transform.DOScale(scale, 1f)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .SetLoops(-1)
            .Pause();
        return s;
    }

    public virtual void UpdateTimeScale()
    {
        if (PlayerCtrl.isClick)
        {
            timeScaleMultiply = isOnSlow ? GameManager.Instance.baseicClickedTimeScaleMultiply : 1 ;
        }
    }


}
