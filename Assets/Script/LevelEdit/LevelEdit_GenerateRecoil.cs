using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_GenerateRecoil : MonoBehaviour
{
    public void GenerateRecoilImpulse()
    {
        GameManager.Instance.cameraManager.GenerateRecoilImpulse();
    }
}
