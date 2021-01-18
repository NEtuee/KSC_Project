using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGenerator : MonoBehaviour
{
    [System.Serializable]
    public class EffectItem
    {
        public GameObject effectObject;
        public float deleteTimer = 0f;
        public bool rotate = true;
        public bool orphan = true;
    }

    [SerializeField]private List<EffectItem> effectList = new List<EffectItem>();

    public void CreateEffectObject(int pos)
    {
        var obj = Instantiate(effectList[pos].effectObject,transform.position,Quaternion.identity);

        if(effectList[pos].rotate)
        {
            obj.transform.rotation = transform.rotation;
        }

        if(!effectList[pos].orphan)
            obj.transform.parent = transform;

        if(effectList[pos].deleteTimer != 0)
            Destroy(obj,effectList[pos].deleteTimer);
    }


}
