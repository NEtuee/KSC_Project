using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuideText : MonoBehaviour
{
    public LoadingDescription loadingDescription;
    public TextMeshProUGUI targetText;

    private Dictionary<string, string> descritionDict = new Dictionary<string, string>();
    private void Start()
    {
        foreach(var description in loadingDescription.loadingDescription)
        {
            descritionDict.Add(description.key, description.description);
        }
    }

    public void SetDescription(string key)
    {
        if(descritionDict.ContainsKey(key) == false)
        {
            Debug.LogError("Not Exits " + key);
            return;
        }

        targetText.text = descritionDict[key];
    }

    public void SetSpace()
    {
        targetText.text = "";
    }
}
