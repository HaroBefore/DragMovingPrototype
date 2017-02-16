using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HeavyDutyInspector;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System;

[System.Serializable]
public struct WayPoint
{
    public Vector3 scale;
    [HideInInspector]
    public Vector3 pos;
    public float addSpeed;
    public float addAngle;
    public bool isFlipRotate;
    public WayPoint(Vector3 scale, Vector3 pos, float addSpeed, float addAngle, bool isFlipRotate)
    {
        this.scale = scale;
        this.pos = pos;
        this.addSpeed = addSpeed;
        this.addAngle = addAngle;
        this.isFlipRotate = isFlipRotate;
    }
}

public class ObstacleCtrl : MonoBehaviour {

    [ComponentSelection]
    public DOTweenPath doTweenPath;

    [Comment("WayPoint 설정 후 클릭")]
    [Button("Reset wayPointPath Position", "ResetArrWayPoint", true)]
    public bool hidden;

    [Comment("true 시 레벨 시작시 스케일 애니메이션을 실행합니다")]
    public bool isBeginAnimateScale = false;

    Vector3 beginScale;

    public float normalSpeedPerSec;
    public float normalAnglePerSec;
    public float changeScaleDuration = 0.2f;
    public float timeScaleMultiply = 1f;

    [Readonly]
    public bool isOnTimeZone = false;

    public AnimationCurve easeCurve;

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
    float originAngleTimeScale;
    float originSpeedTimeScale;
    float originChangeScaleDuration;

    TweenerCore<Vector3, Path, DG.Tweening.Plugins.Options.PathOptions> pathTweener;
    Tweener rotTweener;
    Tweener scaleTweener;

    private void Awake()
    {
        doTweenPath = GetComponent<DOTweenPath>();
        beginScale = transform.localScale;

        ResetArrWayPoint();

        WayPoint[] temp = arrWayPoint;
        arrWayPoint = new WayPoint[temp.Length+1];

        arrWayPoint[0].scale = transform.localScale;
        arrWayPoint[0].pos = transform.position;

        for (int i = 1; i < arrWayPoint.Length; i++)
        {
            arrWayPoint[i] = temp[i-1];
        }
    }
    // Use this for initialization
    void Start () {
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

        if (isBeginAnimateScale)
        {
            transform.localScale = Vector3.zero;
        }

        GameManager.Instance.EventGameStart += OnGameStart;
    }

    public void OnDestroy()
    {
        GameManager.Instance.EventGameStart -= OnGameStart;
    }

    public void OnGameStart()
    {
        if(isBeginAnimateScale)
            transform.DOScale(beginScale, 0.2f);

        rotTweener = transform.DORotate(transform.rotation.eulerAngles + new Vector3(0f, 0f, 360f), 1f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .SetLoops(-1)
            .Pause();
        rotTweener.timeScale = originAngleTimeScale = normalAnglePerSec;
        
        if(wayPointPath.Length > 1)
        {
            pathTweener = transform.DOPath(wayPointPath, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false)
                .SetEase(easeCurve)
                .SetSpeedBased(true)
                .Pause()
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
                    rotTweener = MakeRotTweener(arrWayPoint[idx].isFlipRotate, arrWayPoint[idx].addAngle);

                    UpdateTimeScale();
                });
            pathTweener.timeScale = originSpeedTimeScale = normalSpeedPerSec;
        }
        
        StartCoroutine(CoPlay(pathTweener, rotTweener));
    }

    IEnumerator CoPlay(TweenerCore<Vector3, Path, DG.Tweening.Plugins.Options.PathOptions> pathTweener, Tweener rotTweener)
    {
        yield return new WaitForSeconds(0.5f);
        if(pathTweener != null)
            pathTweener.Play();
        if(rotTweener != null)
            rotTweener.Play();
    }

    public void UpdateTimeScale()
    {
        if(scaleTweener != null)
            scaleTweener.timeScale = originChangeScaleDuration = (1f) * timeScaleMultiply;
        if(rotTweener != null)
            rotTweener.timeScale = originAngleTimeScale = (normalAnglePerSec + arrWayPoint[curWayPointIdx].addAngle) * timeScaleMultiply;
        if(pathTweener != null)
            pathTweener.timeScale = originSpeedTimeScale = (normalSpeedPerSec + arrWayPoint[curWayPointIdx].addSpeed) * timeScaleMultiply;

        if (PlayerCtrl.isClick && !isOnTimeZone)
        {
            if(scaleTweener != null)
                scaleTweener.timeScale = originChangeScaleDuration * 0.05f;
            if(rotTweener != null)
                rotTweener.timeScale = originAngleTimeScale * 0.05f;
            if(pathTweener != null)
                pathTweener.timeScale = originSpeedTimeScale * 0.05f;
        }
    }

    public Tweener MakeRotTweener(bool isRightRot, float addAngle)
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot = isRightRot ? rot - new Vector3(0f, 0f, 360f) : rot + new Vector3(0f, 0f, 360f);

        Tweener r = transform.DORotate(rot, 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .SetLoops(-1);
        return r;
    }

    public Tweener ScaleWayPoint(int idx)
    {
        if (idx >= maxWayPointCnt)
            return null;
        return transform.DOScale(arrWayPoint[idx].scale, changeScaleDuration).Pause();
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
