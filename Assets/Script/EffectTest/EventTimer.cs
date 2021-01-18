using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTimer : MonoBehaviour
{
    [System.Serializable]
    public class EventItem
    {
        public string name;
        public float activeTime = 0f;
        public UnityEvent events;

        public void Invoke() {events.Invoke();}
    }

    [SerializeField]private bool startPlay = true;
    [SerializeField]private bool loop = false;
    [SerializeField]private List<EventItem> eventList = new List<EventItem>();

    private int currentEvent = 0;
    private float currentEventTimer = 0f;
    private bool isEventEnd = true;

    private void Start()
    {
        if(startPlay)
            EventStart();
    }

    private void Update()
    {
        if(!isEventEnd)
        {
            currentEventTimer -= Time.deltaTime;
            if(currentEventTimer <= 0f)
            {
                eventList[currentEvent].Invoke();
                isEventEnd = SetNextEvent();
            }
        }
        
    }

    public void EventStart()
    {
        if(eventList.Count == 0)
            return;

        isEventEnd = false;
        currentEvent = -1;
        SetNextEvent();
    }

    public bool SetNextEvent()
    {
        ++currentEvent;
        if(eventList.Count <= currentEvent)
        {
            if(loop)
            {
                currentEvent = 0;
            }
            else
            {
                return true;
            }
        }

        currentEventTimer = eventList[currentEvent].activeTime;

        return false;
    }
}