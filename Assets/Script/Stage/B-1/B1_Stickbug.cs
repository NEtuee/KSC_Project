using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_Stickbug : ObjectBase
{
    public Core core;
    public EMPShield shield;
    public void Respawn()
    {
        core.Reactive();
        shield.Reactive();
    }
}
