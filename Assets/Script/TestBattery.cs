using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBattery : MonoBehaviour
{
    public void OnTriggerStay(Collider coll)
    {
        if(coll.tag == "Player")
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                var con = coll.GetComponent<TestPortalBatteryScript>();

                con.battery++;

                Destroy(this.gameObject);
            }
        }
    }
}
