using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoadingDescriptionType
{
    public string key;
    public string description;
}

[CreateAssetMenu(fileName = "LoadingDescription", menuName = "LoadingDescription")]
public class LoadingDescription : ScriptableObject
{
    public List<LoadingDescriptionType> loadingDescription;
}
