using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCos : MonoBehaviour
{

    private Transform _transform;
    public float maxScale;
    public float minScale;
    private Vector3 _maxScaleVector;
    private Vector3 _minScaleVector;
    public float speed;
    public bool end;
    private void Start()
    {
        _maxScaleVector = new Vector3(maxScale, maxScale, maxScale);
        _minScaleVector = new Vector3(minScale, minScale, minScale);
        _transform = GetComponent<Transform>();
        StartCoroutine(SacleFadeInOut());
    }

    public IEnumerator SacleFadeInOut()
    {

        while (!end)
        {
            float timeLast = Time.realtimeSinceStartup;
            float timeCurrent = timeLast;

            while (_transform.localScale.x <= _maxScaleVector.x)
            {
                timeCurrent = Time.realtimeSinceStartup;
                _transform.localScale = new Vector3(_transform.localScale.x + 1 * speed * (timeCurrent - timeLast)
                                                    , _transform.localScale.y + 1 * speed * (timeCurrent - timeLast)
                                                    , _transform.localScale.z + 1 * speed * (timeCurrent - timeLast));
                timeLast = timeCurrent;
                yield return null;
            }
            while (_transform.localScale.x >= _minScaleVector.x)
            {
                timeCurrent = Time.realtimeSinceStartup;
                _transform.localScale = new Vector3(_transform.localScale.x + -1 * speed * (timeCurrent - timeLast)
                                                    , _transform.localScale.y + -1 * speed * (timeCurrent - timeLast)
                                                    , _transform.localScale.z + -1 * speed * (timeCurrent - timeLast));
                timeLast = timeCurrent;

                yield return null;
            }
            yield return null;
        }


    }
}
