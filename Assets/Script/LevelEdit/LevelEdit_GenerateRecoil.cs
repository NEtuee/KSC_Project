using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_GenerateRecoil : MonoBehaviour
{
    public void GenerateRecoilImpulse()
    {
        var msg = MessagePool.GetMessage();
        msg.Set(MessageTitles.cameramanager_generaterecoilimpluse,UniqueNumberBase.GetSavedNumberStatic("CameraManager"),null,null);
        MasterManager.instance.HandleMessage(msg);
        //GameManager.Instance.cameraManager.GenerateRecoilImpulse();
    }
}
