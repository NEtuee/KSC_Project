using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapObject : MonoBehaviour
{
    public delegate void Delegate();
    public bool isReady = false;

    public Delegate whenEat = ()=>{};

    public void Eat()
    {
        isReady = false;
        whenEat();
        //this.gameObject.SetActive(false);
    }
}
