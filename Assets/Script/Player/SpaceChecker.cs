using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceChecker : MonoBehaviour
{
    [SerializeField] private bool overlap;

    private void OnTriggerStay(Collider other)
    {
        overlap = true;
    }

    private void OnTriggerExit(Collider other)
    {
        overlap = false;
    }

    public bool Overlapped() { return overlap; }
}
