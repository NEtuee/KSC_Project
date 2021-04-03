using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DroneHelper_Medusa : MonoBehaviour
{
    [SerializeField] private Drone drone;
    [SerializeField] private Canvas droneDiscriptCanvas;
    [SerializeField] private TextMeshProUGUI descriptText;

    [SerializeField] private Vector3 helpStateScale;
    [SerializeField] private Vector3 aimHelpStateScale;

    [SerializeField] private DroneDescript droneDescript;
    [SerializeField] private float hintTime = 5f;
    [SerializeField] private bool helping = false;
    [SerializeField] private bool scanned = false;
    [SerializeField] private bool hint1Complete = false;
    [SerializeField] private bool hint2Complete = false;
    [SerializeField] private bool active = false;

    private Dictionary<string, string> descriptDictionary = new Dictionary<string, string>();

    private TimeCounterEx timer = new TimeCounterEx();

    void Start()
    {
        droneDiscriptCanvas.enabled = false;

        for(int i = 0; i<droneDescript.descripts.Length;i++)
        {
            descriptDictionary.Add(droneDescript.descripts[i].key, droneDescript.descripts[i].descript);
        }

        drone.whenHelp += () => { droneDiscriptCanvas.GetComponent<RectTransform>().localScale = helpStateScale; };
        drone.whenAimHelp += () => { droneDiscriptCanvas.GetComponent<RectTransform>().localScale = aimHelpStateScale; };

    }

    void Update()
    {
        if (active == false)
            return;

        if(helping == true)
        {
            bool limit;
            timer.IncreaseTimer("Help", hintTime, out limit);
            if(limit == true)
            {
                helping = false;
                droneDiscriptCanvas.enabled = false;
                drone.OrderDefault();
                timer.InitTimer("HintTime");
            }
        }
        else
        {
            bool limit;
            timer.IncreaseTimer("HintTime", 10.0f, out limit);
            if(limit)
            {
                if(hint1Complete == false)
                {
                    HelpEvent("Hint1");
                    Hint1Flag();
                }
                else
                {
                    HelpEvent("Hint2");
                    Hint2Flag();
                }
            }
        }
    }

    public bool HelpEvent(string key)
    {
        if (helping == true)
            return false;

        active = true;
        helping = true;
        descriptText.text = descriptDictionary[key];
        droneDiscriptCanvas.enabled = true;
        drone.OrderHelp();
        timer.InitTimer("Help");
        return true;
    }

    public void ScanFlag()
    {
        scanned = true;
    }

    public void Hint1Flag()
    {
        hint1Complete = true;
    }

    public void Hint2Flag()
    {
        hint2Complete = true;
    }
}
