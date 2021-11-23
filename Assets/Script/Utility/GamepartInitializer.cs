using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepartInitializer : ManagerBase
{
    [System.Serializable]
    public class MaterialProperty
    {
        public Material[] targetMaterials;
        public string targetProperty;
        public float factor;

        public void Process()
        {
            foreach(var item in targetMaterials)
                item.SetFloat(targetProperty, factor);
        }
    }

    public MaterialProperty[] targetMaterials;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("PartInit");

        AddAction(MessageTitles.scene_sceneChanged, (x) =>
        {
            foreach(var item in targetMaterials)
            {
                item.Process();
            }
        });
    }
}
