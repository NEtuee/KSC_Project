using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rightOffset = 0.0f;
    [SerializeField] private float upOffset = 0.0f;
    [SerializeField] private float forwardOffset = 0.0f;
    [SerializeField] private bool billBoard = false;
    private Transform mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod != UpdateMethod.FixedUpdate)
            return;

        Vector3 targetPosition = (target.forward*forwardOffset+target.right * rightOffset + target.up * upOffset)+target.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 5f);
        if(billBoard == false)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.forward), Time.fixedDeltaTime * 3f);
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(mainCam.position - transform.position), Time.fixedDeltaTime * 3f);
        }
    }

    private void Update()
    {
        if (((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod != UpdateMethod.Update)
            return;

        Vector3 targetPosition = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
        if (billBoard == false)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.forward), Time.deltaTime * 3f);
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mainCam.position - transform.position), Time.deltaTime * 3f);
        }
    }
}
