using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum Option
{
    Phase = 0,
    Flip = 1
}

public class RotateExtraOptions : MonoBehaviour {

    private FixedObstacleCtrl _fixedObstacleCtrl;
    private float delay;
    private bool isPlaying;
    public Option _option;
    public float degree;

    [Header("일시정지")]
    public float pauseDelay;

    private void Start()
    {
        if (GetComponent<FixedObstacleCtrl>())
        {
            _fixedObstacleCtrl = GetComponent<FixedObstacleCtrl>();
        }
        delay = (degree / _fixedObstacleCtrl.normalAnglePerSec);

        StartCoroutine(WaitForPause());

    }

    public IEnumerator WaitForPause()
    {
        while (_fixedObstacleCtrl.rotTweener == null)
        {
            yield return null;
        }
        yield return _fixedObstacleCtrl.rotTweener.WaitForStart();
        yield return new WaitForSeconds(delay-0.0215f);
        _fixedObstacleCtrl.rotTweener.Pause();
        yield return new WaitForSeconds(pauseDelay);
        _fixedObstacleCtrl.rotTweener.Play();
        while (true)
        {
            yield return new WaitForSeconds(delay);
            _fixedObstacleCtrl.rotTweener.Pause();
            yield return new WaitForSeconds(pauseDelay);
            _fixedObstacleCtrl.rotTweener.Play();
            
        }


    }



}
