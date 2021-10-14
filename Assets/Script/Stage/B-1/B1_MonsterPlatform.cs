using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MonsterPlatform : B1_Platform
{
    public List<CommonDrone> drones = new List<CommonDrone>();
    public List<B1_Spider> spiders = new List<B1_Spider>();
    public MedusaInFallPoint_AI medusa;
    
    private bool _active = false; 

    public override void ApproachInitialize()
    {
        for(int i = 0; i < drones.Count; ++i)
        {
            drones[i].transform.SetParent(this.transform);
            drones[i].Respawn();
            drones[i].launch = false;
            
        }

        for(int i = 0; i < spiders.Count; ++i)
        {
            spiders[i].transform.SetParent(this.transform);
            spiders[i].Respawn();
            spiders[i].launch = false;
            
        }

        _active = true;
    }

    public override void WhenConnect()
    {
        base.WhenConnect();
        for(int i = 0; i < drones.Count; ++i)
        {
            drones[i].LaunchUp(3f);
        }

        for(int i = 0; i < spiders.Count; ++i)
        {
            spiders[i].launch = true;
            
        }
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if(_out || !_active)
        {
            return;
        }

        bool active = false;
        for(int i = 0; i < drones.Count; ++i)
        {
            if(drones[i].gameObject.activeInHierarchy)
            {
                active = true;
                break;
            }
        }

        for(int i = 0; i < spiders.Count; ++i)
        {
            if(spiders[i].gameObject.activeInHierarchy)
            {
                active = true;
                break;
            }
        }

        if(medusa != null)
            active = medusa.enabled;

        if(!active)
        {
            _active = false;
            Out(_opositeSpawnPosition);
        }
    }
}
