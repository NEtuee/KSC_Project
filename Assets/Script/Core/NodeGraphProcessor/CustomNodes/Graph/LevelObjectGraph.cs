using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable]
public class LevelObjectGraph : StateMachineGraph
{
    public string gameObjectGUID;
    public string transformGUID;
    public string levelObjectGUID;


    public LevelObjectGraph()
    {
        gameObjectGUID = AddExposedParameter("GameObject",typeof(GameObjectParameter),null,false,true);
        transformGUID = AddExposedParameter("Transform",typeof(TransformParameter),null,false,true);
        levelObjectGUID = AddExposedParameter("LevelObject",typeof(GraphObjectParamter),null,false,true);
    }
}
