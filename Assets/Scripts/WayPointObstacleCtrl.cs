using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HeavyDutyInspector;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine.Events;

[System.Serializable]
public struct WayPoint
{
    public Vector3 scale;
    public Vector3 pos;
    public float addSpeed;
    public float addAngle;
    public bool isFlipRotate;
    public bool useScaleSetting;
    public WayPoint(Vector3 scale, Vector3 pos, float addSpeed, float addAngle, bool isFlipRotate)
    {
        this.scale = scale;
        this.pos = pos;
        this.addSpeed = addSpeed;
        this.addAngle = addAngle;
        this.isFlipRotate = isFlipRotate;
        useScaleSetting = true;
    }
}

[RequireComponent(typeof(DOTweenPath))]
public class WayPointObstacleCtrl : Obstacle
{
    [Comment("WayPoint Event Element 0 = 시작지점")]
    public List<UnityEvent> eventWayPointList;

    [ComponentSelection]
    public DOTweenPath doTweenPath;

    [Comment("WayPoint 설정 후 클릭")]
    [Button("Reset wayPointPath Position", "ResetArrWayPoint", true)]
    public bool hidden;

    [Comment("True 일 때 Local기반 움직임")]
    public bool isLocalMove = false;

    [Comment("True 일 때 속도(Seped) 기반 False 일 때 시간(Time) 기반")]
    public bool isSpeedBased = true;

    public float normalSpeedPerSec = 1f;
    public float delay = 0f;
    public LoopType looptype = LoopType.Yoyo;
    
    public AnimationCurve easeCurve = AnimationCurve.Linear(0f,0f,1f,1f);

    void ResetArrWayPoint()
    {

        WayPoint[] oldArrWayPoint = arrWayPoint;
        if (doTweenPath == null)
            Debug.Log("[Error]ObstacleCtrl - DoTweenPath가 null 입니다");
        int cnt = doTweenPath.wps.Count;
        arrWayPoint = new WayPoint[cnt];

        for (int i = 0; i < arrWayPoint.Length; i++)
        {
            arrWayPoint[i] = new WayPoint(Vector3.one, Vector3.zero, 0f, 0f, false);
        }

        for (int i = 0; i < cnt; i++)
        {

            if (oldArrWayPoint != null)
            {
                if (oldArrWayPoint.Length == i)
                    break;
                arrWayPoint[i] = oldArrWayPoint[i];
            }
            if (!oldArrWayPoint[i].useScaleSetting)
                arrWayPoint[i].scale = transform.localScale;

            arrWayPoint[i].pos = doTweenPath.wps[i];

        }
    }

    [Comment("[Readonly] Current WayPoint Index")]
    [Readonly]
    public int wayPointIdx = 0;

    [ReorderableArray]
    public WayPoint[] arrWayPoint;

    Vector3[] wayPointPath;
    int maxWayPointCnt;
    int curWayPointIdx;
    float originSpeedTimeScale;

    TweenerCore<Vector3, Path, DG.Tweening.Plugins.Options.PathOptions> pathTweener;

    new private void Awake()
    {
        base.Awake();

        doTweenPath = GetComponent<DOTweenPath>();
        beginScale = transform.localScale;

        if (!isLocalMove)
        {
            ResetArrWayPoint();
        }

        WayPoint[] temp = arrWayPoint;
        arrWayPoint = new WayPoint[temp.Length + 1];

        arrWayPoint[0].scale = transform.localScale;
        if(isLocalMove)
        {
            arrWayPoint[0].pos = transform.localPosition;
        }
        else
        {
            arrWayPoint[0].pos = transform.position;
        }

        for (int i = 1; i < arrWayPoint.Length; i++)
        {
            arrWayPoint[i] = temp[i - 1];
        }
    }
    // Use this for initialization
    new private void Start()
    {
        StartCoroutine(CoInit());
    }


    public IEnumerator CoInit()
    {
        yield return null;

        wayPointPath = new Vector3[arrWayPoint.Length];
        for (int i = 0; i < wayPointPath.Length; i++)
        {
            wayPointPath[i] = arrWayPoint[i].pos;
        }
        maxWayPointCnt = wayPointPath.Length;

        GameManager.Instance.EventGameStart += OnGameStart;
    }

    public void OnDestroy()
    {
        GameManager.Instance.EventGameStart -= OnGameStart;
    }

    public override void BeginObstacle()
    {
        rotTweener = MakeRotTweener(arrWayPoint[0].isFlipRotate);
        rotTweener.Play();
        rotTweener.timeScale = originAngleTimeScale = normalAnglePerSec;

        if(isLocalMove)
        {
            for (int i = 0; i < wayPointPath.Length; i++)
            {
                Debug.Log(wayPointPath[i]);
            }
        }
        

        if (wayPointPath.Length > 1)
        {
            pathTweener = transform.DOLocalPath(wayPointPath, 1f, gizmoColor: Color.red)
                .SetSpeedBased(isSpeedBased)
                .SetLoops(-1, looptype)
                .SetAutoKill(false)
                .SetEase(easeCurve)
                .Pause()
                .SetDelay(delay * normalSpeedPerSec)
                .OnWaypointChange(idx =>
                {
                    curWayPointIdx = idx;
                    if (scaleTweener != null)
                    {
                        scaleTweener.Kill();
                        scaleTweener = null;
                    }
                    scaleTweener = ScaleWayPoint(idx);
                    scaleTweener.Play();

                    if (rotTweener != null)
                    {
                        rotTweener.Kill();
                        rotTweener = null;
                    }
                    rotTweener = MakeRotTweener(arrWayPoint[idx].isFlipRotate);
                    rotTweener.Play();

                    if(eventWayPointList.Count > 0)
                    {
                        UnityEvent unityEvent = eventWayPointList[curWayPointIdx];
                        if (unityEvent != null)
                        {
                            unityEvent.Invoke();
                        }
                    }

                    UpdateTimeScale();
                });
            pathTweener.timeScale = originSpeedTimeScale = normalSpeedPerSec;
        }

        StartCoroutine(CoPlay(pathTweener, rotTweener));
    }

    public override void OnPauseObstacle()
    {
        if (!isPlayingTween)
            return;

        if (pathTweener != null)
        {
            if (pathTweener.IsPlaying())
                pathTweener.Pause();
        }
        base.OnPauseObstacle();
    }

    public override void OnResumeObstacle()
    {
        if (isPlayingTween)
            return;

        if (pathTweener != null)
        {
            if (!pathTweener.IsPlaying())
                pathTweener.Play();
        }
        base.OnResumeObstacle();
    }

    IEnumerator CoPlay(TweenerCore<Vector3, Path, DG.Tweening.Plugins.Options.PathOptions> pathTweener, Tweener rotTweener)
    {
        yield return new WaitForSeconds(0.5f);
        if(isBeginToPlay)
        {
            isPlayingTween = true;

            if (pathTweener != null)
                pathTweener.Play();
            if (rotTweener != null)
                rotTweener.Play();
        }
    }

    public override void UpdateTimeScale()
    {
        if (scaleTweener != null)
            scaleTweener.timeScale = originChangeScaleSpeed = 1f;
        if (pathTweener != null)
            pathTweener.timeScale = originSpeedTimeScale = (normalSpeedPerSec + arrWayPoint[curWayPointIdx].addSpeed);
        if (rotTweener != null)
            rotTweener.timeScale = originAngleTimeScale = (normalAnglePerSec + arrWayPoint[curWayPointIdx].addAngle);

        base.UpdateTimeScale();

        if (PlayerCtrl.isClick)
        {
            if (pathTweener != null)
                pathTweener.timeScale = originSpeedTimeScale * timeScaleMultiply;
            if (scaleTweener != null)
                scaleTweener.timeScale = originChangeScaleSpeed * timeScaleMultiply;
            if (rotTweener != null)
                rotTweener.timeScale = originAngleTimeScale * timeScaleMultiply;
        }
    }

    public Tweener ScaleWayPoint(int idx)
    {
        if (idx >= maxWayPointCnt)
            return null;
        return transform.DOScale(arrWayPoint[idx].scale, changeScaleSpeed).Pause();
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
}
