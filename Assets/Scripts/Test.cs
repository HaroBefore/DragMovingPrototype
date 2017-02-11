using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Test : MonoBehaviour {

    DOTweenPath path;
    Tween tween;

    // Use this for initialization
    void Start () {
        path = GetComponent<DOTweenPath>();

        tween = path.tween;

        StartCoroutine(CoTween());
	}
	
	// Update is called once per frame
	void Update () {
        if (tween != null)
        {
            Debug.Log(tween.Elapsed());
        }
    }

    IEnumerator CoTween()
    {
        yield return new WaitForSeconds(2f);
        path.tween.timeScale = 5f;
        yield return new WaitForSeconds(1f);
        tween.timeScale = 1f;
    }
}
