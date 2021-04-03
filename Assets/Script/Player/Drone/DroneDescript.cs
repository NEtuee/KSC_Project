using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Descript
{
    public string key;
    public string descript;
}

[CreateAssetMenu(fileName = "DroneDescript", menuName = "DroneDescript")]
public class DroneDescript : ScriptableObject
{
    public Descript[] descripts;
    
}
