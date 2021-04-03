using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DroneHelper : MonoBehaviour
{
    [SerializeField] protected DroneHelperRoot root;

    private void Start()
    {
        root = GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneHelperRoot>();
        root.SetHelper(this);
    }

    public void InitHelper(DroneHelperRoot root)
    {
        this.root = root;
    }

    public abstract void HelperUpdate();
}
