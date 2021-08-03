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
    public string levelObjectTransformGUID;
    public string levelObjectGUID;


    public LevelObjectGraph()
    {
        gameObjectGUID = AddExposedParameter("GameObject",typeof(GameObjectParameter),null,false);
        transformGUID = AddExposedParameter("Transform",typeof(TransformParameter),null,false);
        levelObjectTransformGUID = AddExposedParameter("LevelObject",typeof(GraphObjectParamter),null,false);
        levelObjectGUID = AddExposedParameter("LvObjTransform",typeof(GraphObjectTransformParamter),null,false);
    }
}
