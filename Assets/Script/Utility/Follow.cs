using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public enum Standard
    {
        ThisTransform,
        Camera
    }

    [SerializeField] private Standard standard;
    [SerializeField] private Transform target;
    [SerializeField] private float rightOffset = 0.0f;
    [SerializeField] private float upOffset = 0.0f;
    [SerializeField] private float forwardOffset = 0.0f;
    [SerializeField] private bool billBoard = false;
    [SerializeField] private bool flip = false;
    [SerializeField] private bool interpolation;
    private Transform mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod != UpdateMethod.FixedUpdate)
            return;
        Vector3 targetPosition;
        switch(standard)
        {
            case Standard.ThisTransform:
                targetPosition = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
                break;
            case Standard.Camera:
                Vector3 camForward = Camera.main.transform.forward;
                Vector3 camRight = Camera.main.transform.right;
                camForward.y = 0;
                camRight.y = 0;
                targetPosition = (camForward * forwardOffset + camRight * rightOffset + Vector3.up * upOffset) + target.position;
                break;
            default:
                targetPosition = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
                break;
        }

        if (interpolation == true)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 5f);
        else
            transform.position = targetPosition;

        if (billBoard == false)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.forward), Time.fixedDeltaTime * 3f);
            //transform.rotation = transform.rotation * Quaternion.AngleAxis(90.0f, transform.up);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-(mainCam.position - transform.position)), Time.fixedDeltaTime * 3f);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod != UpdateMethod.Update)
            return;

        Vector3 targetPosition;
        switch (standard)
        {
            case Standard.ThisTransform:
                targetPosition = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
                break;
            case Standard.Camera:
                Vector3 camForward = Camera.main.transform.forward;
                Vector3 camRight = Camera.main.transform.right;
                camForward.y = 0;
                camRight.y = 0;
                targetPosition = (camForward * forwardOffset + camRight * rightOffset + Vector3.up * upOffset) + target.position;
                break;
            default:
                targetPosition = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
                break;
        }

        if (interpolation == true)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 5f);
        else
            transform.position = targetPosition;

        if (billBoard == false)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.forward) * Quaternion.AngleAxis(180.0f, transform.up), Time.deltaTime * 3f);
            //transform.rotation = transform.rotation * Quaternion.AngleAxis(90.0f, transform.up);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-(mainCam.position - transform.position)), Time.deltaTime * 3f);
        }
    }
}
