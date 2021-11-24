using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Object = System.Object;

public class DroneHelperRoot : MonoBehaviour
{
    public class DescData
    {
        public string desc;
        public float duration;
        public AudioClip audio;
    }

    [SerializeField] public AudioSource audioPlay;

    [SerializeField] public Drone drone;
    [SerializeField] private Canvas droneDiscriptCanvas;
    [SerializeField] private EnumerateText descriptText;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private Vector3 helpStateScale;
    [SerializeField] private Vector3 aimHelpStateScale;

    [SerializeField] private DroneDescript droneDescript;
    [SerializeField] public float hintTime = 5f;
    [SerializeField] public bool helping = false;
    [SerializeField] public bool active = false;

    private Dictionary<string, DescData> descriptDictionary = new Dictionary<string, DescData>();

    public TimeCounterEx timer = new TimeCounterEx();

    [SerializeField] private DroneHelper currentHelper;


    public string NameText { set => nameText.text = value; }

    void Start()
    {
        droneDiscriptCanvas.enabled = false;

        for (int i = 0; i < droneDescript.descripts.Count; i++)
        {
            var item = new DescData();
            item.desc = droneDescript.descripts[i].descript;
            item.duration = droneDescript.descripts[i].duration;
            item.audio = droneDescript.descripts[i].audioData;

            descriptDictionary.Add(droneDescript.descripts[i].key, item);
        }

        //drone.whenHelp += () => { droneDiscriptCanvas.GetComponent<RectTransform>().localScale = helpStateScale; };
        //drone.whenAimHelp += () => { droneDiscriptCanvas.GetComponent<RectTransform>().localScale = aimHelpStateScale; };
    }

    // Update is called once per frame
    void Update()
    {
        if(ReferenceEquals(currentHelper, null) == false)
        {
            currentHelper.HelperUpdate();
        }

        if (helping == true)
        {
            bool limit;
            timer.IncreaseTimerSelf("Help", hintTime, out limit,Time.deltaTime);
            if (limit == true)
            {
                helping = false;
                ActiveDescriptCanvas(false);
                drone.OrderDefault();
            }
        }
    }

    public bool HelpEvent(string key)
    {
        if (descriptDictionary.ContainsKey(key) == false)
        {
            Debug.Log(key+" Not Exits Key");
            return false;
        }

        active = true;
        helping = true;
        descriptText.SetTargetString(descriptDictionary[key].desc);
        droneDiscriptCanvas.enabled = true;
        drone.OrderHelp();
        hintTime = descriptDictionary[key].duration;
        timer.InitTimer("Help", 0.0f, hintTime);

        var audio = descriptDictionary[key].audio;
        if(audio != null)
        {
            audioPlay.Stop();
            audioPlay.clip = audio;
            audioPlay.Play();
        }

        return true;
    }

    public bool HelpEvent(string key, float durationTime)
    {
        if (descriptDictionary.ContainsKey(key) == false)
        {
            Debug.Log(key + " Not Exits Key");
            return false;
        }

        active = true;
        helping = true;
        descriptText.SetTargetString(descriptDictionary[key].desc);
        droneDiscriptCanvas.enabled = true;
        drone.OrderHelp();
        hintTime = durationTime;
        timer.InitTimer("Help",0.0f,hintTime);

        var audio = descriptDictionary[key].audio;
        if (audio != null)
        {
            audioPlay.Stop();
            audioPlay.clip = audio;
            audioPlay.Play();
        }

        return true;
    }


    public bool ShowText(string text)
    {
        active = true;
        helping = true;
        descriptText.SetTargetString(text);
        droneDiscriptCanvas.enabled = true;
        drone.OrderHelp();
        timer.InitTimer("Help");
        return true;
    }

    public void ActiveDescriptCanvas(bool active)
    {
        droneDiscriptCanvas.enabled = active;
    }

    public void SetHelper(DroneHelper helper)
    {
        currentHelper = helper;
    }
}
