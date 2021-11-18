using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Genie_DroneLinePool : ObjectPoolBase<Genie_DroneLine>
{
    public UnityEngine.Events.UnityEvent whenDestroy;
    public void Awake()
    {
        _activeDelegate += (t, position, rotation) =>
        {
            t.gameObject.SetActive(true);
            
        };

        _deleteProgressDelegate += t =>
        {
            
        };

        _deleteCondition += t =>
        {
            return (!t.gameObject.activeInHierarchy);
        };
    }

    public void AddCreateDelegate(Genie_Phase_AI genie, float spin, float apear, float startWait, float endWait)
    {
        _createDelegate += (t) =>
        {
            // Debug.Log(whenDestroy.GetPersistentMethodName (0));
            // var info = UnityEventBase.GetValidMethodInfo (whenDestroy.GetPersistentTarget (0), whenDestroy.GetPersistentMethodName (0), new System.Type[] { typeof (Transform) });
            // UnityAction execute = () => info.Invoke (whenDestroy.GetPersistentTarget (0), new object[] {transform});
            //event2.AddListener (execute);

            var shiled = t.coreDrone.shield;
            //shiled.whenDestroy .AddListener(execute);
            shiled.whenDestroy .AddListener(()=>{genie.ChangeAnimation(1);});
            shiled.whenDestroy .AddListener(()=>{genie.stateProcessor.StateChange("Angry");});
            t.empShield.whenDestroy.AddListener(() => { genie.ChangeAnimation(1); });
            t.empShield.whenDestroy.AddListener(() => { genie.stateProcessor.StateChange("Angry"); });
            t.Create(spin,apear,startWait,endWait);
        };
    }
};