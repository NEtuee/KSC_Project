using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScale : UnTransfromObjectBase
{
    private RectTransform rect;

    [SerializeField] private Vector3 standardScale;
    private CameraManager _cameraManager;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setCameraManager, (msg) =>
         {
             _cameraManager = (CameraManager)msg.data;
         });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("UIManager"));

        SendMessageQuick(MessageTitles.cameramanager_getCameraManager, GetSavedNumber("CameraManager"), null);

        rect = GetComponent<RectTransform>();

        if(_cameraManager == null)
        {
            Debug.LogError("Not Set CameraManager");
        }
    }

    private void FixedUpdate()
    {
        if(rect == null || GameManager.Instance == null)
        {
            return;
        }

        float camDist = _cameraManager.GetCameraDistance();
        rect.localScale = standardScale * camDist;
    }
}
