using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VibrationSet
{
    public string key;
    public float leftSpeed;
    public float rightSpeed;
    public float time;
}

[CreateAssetMenu(fileName = "GamepabVibrationSet", menuName = "GamepabVibrationSet")]
public class GamepabVibrationSet : ScriptableObject
{
    public List<VibrationSet> vibrationSets = new List<VibrationSet>();
}
