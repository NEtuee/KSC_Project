using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyManager : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(respawnPoint != null)
            {
                other.transform.position = respawnPoint.position;
            }
        }
    }
}
