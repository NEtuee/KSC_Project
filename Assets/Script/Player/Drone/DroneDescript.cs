using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Descript
{
    public string key;
    [TextArea]public string descript;
    public AudioClip audioData;
}

[CreateAssetMenu(fileName = "DroneDescript", menuName = "DroneDescript")]
public class DroneDescript : ScriptableObject
{
    public List<Descript> descripts = new List<Descript>();
    
}
