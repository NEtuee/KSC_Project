using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBack : MonoBehaviour
{
    public bool playerCheck = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerCheck = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCheck = false;
        }
    }
}
