using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMessageTrigger : MonoBehaviour
{
    public LayerMask targetLayer;
    public Transform center;

    public void OnTriggerEnter(Collider coll)
    {
        if (targetLayer == (targetLayer | (1 << coll.gameObject.layer)))
        {
            if(coll.gameObject.TryGetComponent<MessageReceiver>(out var receiver))
            {
                var msg = MessagePool.GetMessage();
                msg.Set(MessageTitles.dash_trigger, receiver.uniqueNumber, center, null);
                receiver.ReceiveMessage(msg);
            }
        }
    }
}
