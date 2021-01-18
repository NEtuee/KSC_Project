using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTrigger : MonoBehaviour
{
    [SerializeField] private bool isDetect;
    [SerializeField] private string detectTag;
    void Start()
    {
        if(GetComponent<Collider>() == null)
        {
            Debug.LogWarning("Not Set Collider");
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(detectTag))
        {
            isDetect = true;
        }
        else
        {
            isDetect = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(detectTag))
        {
            isDetect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(detectTag))
        {
            isDetect = false;
        }
    }

    public bool GetIsDetect() { return isDetect; }
}
