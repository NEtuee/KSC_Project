using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialPage
{
    public string pageName;
    public List<string> tutorialList = new List<string>();
}

[CreateAssetMenu(fileName = "TutorialPageSet", menuName = "TutorialPageSet")]
public class TutorialPageSet : ScriptableObject
{
    public TutorialPage[] pages;
}
