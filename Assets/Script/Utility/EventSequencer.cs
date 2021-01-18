using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSequencer : MonoBehaviour
{
    [System.Serializable]
    public class EventItem
    {
        public string name;
        public UnityEvent events;
        public float timer = 0f;

        public void Invoke()
        {
            events.Invoke();
        }
    }

    [SerializeField]private bool loop = true;
    [SerializeField]private List<EventItem> eventList = new List<EventItem>();

    private bool progress = false;
    private int currentEventPos = 0;
    private float currentTimer = 0f;

    public void StartEvent()
    {
        progress = true;
        this.enabled = true;
        currentEventPos = -1;
        GetNextEvent();
    }

    private void Update()
    {
        if(progress)
        {
            currentTimer -= Time.deltaTime;
            if(currentTimer <= 0f)
            {
                if(!GetNextEvent())
                {
                    if(loop)
                    {
                        currentEventPos = -1;
                        GetNextEvent();
                    }
                    else
                    {
                        progress = false;
                        this.enabled = false;
                    }
                }


            }


        }



    }

    private bool GetNextEvent()
    {
        if(++currentEventPos >= eventList.Count)
            return false;
        
        currentTimer = eventList[currentEventPos].timer;
        eventList[currentEventPos].Invoke();

        return true;
    }
}
