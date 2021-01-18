using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEdit_TriggerManager : MonoBehaviour
{
    [System.Serializable]
    public class TriggerItem
    {
        public int triggerCount = 0;
        public UnityEvent allTriggered = new UnityEvent();
        public UnityEvent affterTriggered = new UnityEvent();
        public float afterEventTimer = 0f;

        public bool TriggerCheck()
        {
            if(triggerCount == 0)
                return false;
            

            if(--triggerCount == 0)
            {
                allTriggered.Invoke();

                return true;
            }

            return false;
        }

        public IEnumerator AfterTriggerProgress()
        {
            yield return new WaitForSeconds(afterEventTimer);

            affterTriggered.Invoke();
        }
    }

    [SerializeField]private List<TriggerItem> triggerItems = new List<TriggerItem>();

    public void InvokeTrigger(int code)
    {
        triggerItems[code].triggerCount = 1;
        triggerItems[code].TriggerCheck();
    }

    public void TriggerCheck(int code)
    {
        if(triggerItems[code].TriggerCheck())
        {
            StartCoroutine(triggerItems[code].AfterTriggerProgress());
        }
    }
}
