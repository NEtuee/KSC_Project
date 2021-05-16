using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class VideoClipAndDescripts
{
    public string key;
    public VideoClip videoClip;
    [TextArea]
    public string description;
}

[CreateAssetMenu(fileName = "TutorialVideoClipAndDescriptSet", menuName = "TutorialVideoClipAndDescriptSet")]
public class TutorialVideoClipAndDescriptSet : ScriptableObject
{
    public VideoClipAndDescripts[] VideoClipAndDescripts;
}
