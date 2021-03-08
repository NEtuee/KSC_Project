using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSoundChecker : MonoBehaviour
{
    public int soundCode;

    FMODSoundManager _manager;
    void Start()
    {
        _manager = FindObjectOfType(typeof(FMODSoundManager)) as FMODSoundManager;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            var screen = Input.mousePosition;
            var point = Camera.main.ScreenToWorldPoint(screen);

            Ray ray = Camera.main.ScreenPointToRay(screen);

            if(Physics.Raycast(ray,out RaycastHit hit,1000f))
            {
                if(hit.collider.gameObject == gameObject)
                {
                    _manager.Play(soundCode,Vector3.zero,transform);
                }
            }
        }
    }
}
