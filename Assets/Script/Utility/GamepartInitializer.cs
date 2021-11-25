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
            {
                item.SetFloat(targetProperty, factor);
            }
                
        }
    }

    public MaterialProperty[] targetMaterials;
    public Drone droneTarget;

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

            foreach(var item in droneTarget.disapearTargets)
            {
                item.SetActive(true);
            }

            droneTarget.UpdateDissolve(droneTarget.dissolveTime);
        });
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if (LevelEdit_TimelinePlayer.CUTSCENEPLAY)
        {
            foreach (var item in targetMaterials)
            {
                item.Process();
            }
        }
    }
}
