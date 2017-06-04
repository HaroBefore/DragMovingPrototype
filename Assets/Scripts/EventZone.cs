using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class EventHolder
{
    public bool useOnce;
    public UnityEvent _event;

    private bool close;

    public void SetClose()
    {
        close = true;
    }
    public bool IsAvailable()
    {
        if (close == true)
            return false;
        else
            return true;
    }
}


public class EventZone : MonoBehaviour
{
    public float enterDelay;
    public EventHolder[] enterEvent;
    [Space]
    public float stayDelay;
    public float stayTick;
    public EventHolder[] stayEvent;    
    [Space]
    public float exitDelay;
    public EventHolder[] exitEvent;
    private bool isEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainPlayer") || collision.CompareTag("CustomPlayer"))
        {
            StartCoroutine(InvokeEvents(enterEvent,enterDelay));
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("MainPlayer") || collision.CompareTag("CustomPlayer"))
        {
            if (!isEvent)
            {
                StartCoroutine(InvokeEvents(stayEvent, stayDelay));
                StartCoroutine(Delay(stayTick));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MainPlayer") || collision.CompareTag("CustomPlayer"))
        {
            StartCoroutine(InvokeEvents(exitEvent, exitDelay));
        }
    }

    IEnumerator Delay(float tick)
    {
        yield return new WaitForSeconds(tick);
        isEvent = true;
    }
    IEnumerator InvokeEvents(EventHolder[] targetEvent, float Delay = 0)
    {
        if (Delay != 0)
            yield return new WaitForSeconds(Delay);

        foreach (EventHolder e in targetEvent)
        {
            if (e.IsAvailable())
            {
                if (e.useOnce)
                    e.SetClose();
                e._event.Invoke();
            }
        }

    }
}
