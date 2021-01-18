using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_RayFixParticle : MonoBehaviour
{
    [SerializeField]public ParticleSystem tp;
    [SerializeField]public Transform tpPoint;
    [SerializeField]private LayerMask layerMask;
    [SerializeField]private Vector3 size;
    [SerializeField]private float rayDist;

    private List<ParticleSystem> childList;

    private BoxRayEx ray;
    private bool triggered = false;
    public bool stop = false;

    private void Start()
    {
        ray = new BoxRayEx(new Ray(Vector3.zero,Vector3.zero),rayDist,size,layerMask);

        if(tpPoint != null)
        {
            var particles = tpPoint.GetComponentsInChildren<ParticleSystem>();
            childList = new List<ParticleSystem>(particles);
        }
        
    }

    void Update()
    {
        if(stop)
        {
            return;
        }
        ray.SetDirection(-transform.up);

        RaycastHit hit;
        if(ray.Cast(transform.position,out hit))
        {
            if(tp != null)
                tp.transform.position = hit.point;
            if(tpPoint != null)
            {
                tpPoint.position = hit.point + Vector3.up * 2;
            }

            if(!triggered)
            {
                triggered = true;
                if(tp != null)
                    tp.Play(true);
                PlayChilds();
            }
        }
        else
        {
            if(triggered)
            {
                triggered = false;
                if(tp != null)
                    tp.Stop(true);
                StopChilds();
            }
            

        }

    }

    public void PlayChilds()
    {
        if(childList != null)
        {
            foreach(var child in childList)
            {
                child.Play(true);
            }
        }
    }

    public void StopChilds()
    {
        if(childList != null)
        {
            foreach(var child in childList)
            {
                child.Stop(true);
            }
        }
    }

    public void StopParticle()
    {
        if(tp != null)
            tp.Stop(true);
        StopChilds();
        stop = true;
    }

    public void PlayParticle()
    {
        stop = false;
        triggered = false;
    }
}
