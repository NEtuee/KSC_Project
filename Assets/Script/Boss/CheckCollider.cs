using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    [SerializeField] private List<string> checkTags = new List<string>();
    public delegate void EnterEvent();
    public EnterEvent enterEvent;

    private void OnTriggerEnter(Collider other)
    {

        if(checkTags.Count == 0)
        {
            enterEvent?.Invoke();
            return;
        }

        foreach(var tag in checkTags)
        {
            if(other.CompareTag(tag) == true)
            {
                enterEvent?.Invoke();
                return;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}
