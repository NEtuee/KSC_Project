using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : ManagerBase
{
    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("ObjectManager");
    }
}
