using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManagerEx : ManagerBase
{
    public Elevator entranceElevator;
    public Elevator exitElevator;
    public Transform loadedPlayerPosition;
    
    protected override void Awake()
    {
        base.Awake();

        SaveMyNumber("StageManager",true);
        RegisterRequest();
    }

    public override void Assign()
    {
        
    }
}
