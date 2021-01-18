using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtil : MonoBehaviour
{
    public LevelEdit_TriggerManager bossKillTrigger;
    public AbsorbEvent bossKillTrigger2;
    void Update()
    {
        //kill Boss
        if(Input.GetKeyDown(KeyCode.Z))
        {
            bossKillTrigger.InvokeTrigger(0);
            bossKillTrigger2.GetPullEvent().Invoke();
        }
    }
}
