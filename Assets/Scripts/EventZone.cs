using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EventHolder
{
    public UnityEvent _event;
    public bool useOnce;
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


public class EventZone : MonoBehaviour {

    public EventHolder[] enterEvent;
    public float stayTick;
    public EventHolder[] stayEvent;
    public EventHolder[] exitEvent;
    private bool isEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(EventHolder e in enterEvent)
        {
            if(e.IsAvailable())
            {
                if (e.useOnce)
                    e.SetClose();
                e._event.Invoke();
            }
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isEvent)
        {
            isEvent = false;
            StartCoroutine(Delay(stayTick));
            foreach (EventHolder e in stayEvent)
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (EventHolder e in exitEvent)
        {
            if (e.IsAvailable())
            {
                if (e.useOnce)
                    e.SetClose();
                e._event.Invoke();
            }
        }
    }

    IEnumerator Delay(float tick)
    {
        yield return new WaitForSeconds(tick);
        isEvent = true;
    }

}
