using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GamePadKeyTranslatePair
{
    public KeyCode keycode;
    public string translateString;
}

[CreateAssetMenu(fileName = "GamePadKeyTranslate", menuName = "GamePadKeyTranslate")]
public class GamePadKeyTranslate : ScriptableObject
{
    public GamePadKeyTranslatePair[] keyTranslatePairs;
}
