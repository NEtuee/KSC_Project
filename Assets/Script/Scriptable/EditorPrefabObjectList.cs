using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Options", menuName = "Options/PrefabList")]
public class EditorPrefabObjectList : ScriptableObject
{
    [SerializeField]
    public GameObject[] prefabs;
}
