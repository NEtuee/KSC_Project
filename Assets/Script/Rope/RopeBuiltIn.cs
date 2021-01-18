using System.Collections.Generic;
using UnityEngine;

public class RopeBuiltIn : MonoBehaviour
{
    public Rigidbody spearBody;
    [SerializeField]private LayerMask ropeCollisionLayer;
    [SerializeField]private float ropeLength = 1f;
    [SerializeField]private float ropeThickness = 0.1f;

    private ConfigurableJoint joint;
    private LineRenderer lineRenderer;
    private Rigidbody rig;
    private RopeEx rope;

    private SphereRayEx rayEx;

    private List<Vector3> edgePoints = new List<Vector3>();

    private float currentLimit;
    private float currentLength;

    public bool canHanging = false;

    void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        lineRenderer = GetComponent<LineRenderer>();
        rig = GetComponent<Rigidbody>();
        rope = GetComponent<RopeEx>();

        rayEx = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),currentLength,0.5f,ropeCollisionLayer);

        if(!lineRenderer)
        {
            lineRenderer = transform.GetComponentInChildren<LineRenderer>();
        }

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = rope.jointCount + 1;
    }

    public void Start()
    {
        InitializeRope();
    }


    void LateUpdate()
    {
        UpdateValues();

        // edgePoints.Clear();
        // if(ropeSolver.FindEdgePoint(GetAnchorPos(),transform.position,ref edgePoints,6,ropeCollisionLayer))
        // {
        //     Debug.Log("Check");
        // }

    }

    public void InitializeRope()
    {
        SetRopeThickness(ropeThickness);
        UpdateRope(ropeLength);
        UpdateValues();
    }

    public void InitializeRope(Vector3 grabPos)
    {
        float dist = (transform.position - grabPos).magnitude;

        transform.position = grabPos;


        SetRopeThickness(ropeThickness);
        UpdateRope(Vector3.Distance(grabPos,GetAnchorPos()));
        UpdateValues();
    }

    public void SetAnchorPos(Vector3 value) {joint.connectedAnchor = value;}
    public Vector3 GetAnchorPos() {return joint.connectedAnchor;}

    public void UpdateValues()
    {
        var len = Vector3.Distance(joint.connectedAnchor,transform.position);
        if(len >= ropeLength)
        {
            var force = spearBody.transform.position - transform.position;
            spearBody.AddForce(-force);
            currentLength = ropeLength;
        }

        FixPosition(currentLimit);
        currentLength = currentLimit;

        lineRenderer.SetPosition(0,joint.connectedAnchor);
        for(int i = 1; i < lineRenderer.positionCount; ++i)
        {
            lineRenderer.SetPosition(i,rope.joints[i - 1].position);
        }
    }

    public void FixPosition(float length)
    {
        var dir = transform.position - GetAnchorPos();
        transform.position = GetAnchorPos() + dir.normalized * length;
    }

    public void UpdateRope(float len)
    {
        len = len < 0 ? 0f : (len > ropeLength ? ropeLength : len);
        currentLimit = len;

        var limit = joint.linearLimit;
        limit.limit = len;
        joint.linearLimit = limit;

        rope.SetRopeLength(ropeLength - currentLimit);
        UpdateValues();
    }

    public void DettachRope()
    {

    }

    public void ClimbingRope(float inputValue , float ropeClimbingSpeed, Vector3 right, bool left)
    {
        float targetLen = currentLength + (inputValue * ropeClimbingSpeed * Time.deltaTime);
        targetLen = targetLen < 0 ? 0f : (targetLen > ropeLength ? ropeLength : targetLen);
        currentLimit = targetLen;

        var limit = joint.linearLimit;
        limit.limit = targetLen;
        joint.linearLimit = limit;

        rope.SetRopeLength(ropeLength - currentLimit);
        AddForce((left ? -right : right) * 2f);
        UpdateValues();
    }

    public void AddForce(Vector3 force)
    {
        var rotate = transform.localRotation.eulerAngles;
        rotate.y = 0f;

        force.y = 0f;
        force = Quaternion.Euler(rotate) * force;

        rig.AddForce(force);
    }

    public void DrawDebugLine()
    {
        GizmoHelper.Instance.DrawLine(transform.position,transform.position + rig.velocity, Color.blue);
    }

    public void SetRopeThickness(float t)
    {
        ropeThickness = t;
        lineRenderer.startWidth = ropeThickness;
        lineRenderer.endWidth = ropeThickness;
    }

    public void Disable()
    {
        Transform player = transform.Find("Player_02");
        if(player != null)
        {
            player.transform.parent = null;
        }
        Destroy(this.gameObject);
    }
}
