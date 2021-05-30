using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialDataCenter : MonoBehaviour
{
    [SerializeField] private TutorialVideoClipAndDescriptSet tutorialVideoClipAndDescriptionSet;
    
    private Dictionary<string, VideoClipAndDescripts> _tutorialData = new Dictionary<string, VideoClipAndDescripts>();
    
    void Start()
    {
        for (int i = 0; i < tutorialVideoClipAndDescriptionSet.VideoClipAndDescripts.Length; i++)
        {
            _tutorialData.Add(tutorialVideoClipAndDescriptionSet.VideoClipAndDescripts[i].key,tutorialVideoClipAndDescriptionSet.VideoClipAndDescripts[i]);
        }
    }

    public void GetVideo(string key, out VideoClip videoClip, out string description)
    {
        if (_tutorialData.ContainsKey(key) == false)
        {
            videoClip = null;
            description = null;
            return;
        }    
        
        var data = _tutorialData[key];
        videoClip = data.videoClip;
        description = data.description;
        description = description.Replace("\\n", "\n");
    }
}
