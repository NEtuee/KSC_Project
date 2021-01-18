
using UnityEngine;

public class BoxRayEx : RayEx
{
    public Vector3 Size;

    public override void Draw(Vector3 position, Color color)
    {
        var origin = position + RayInfo.origin;
        // GizmoHelper.Instance.DrawLine(origin, origin + RayInfo.direction * Distance,color);
        // GizmoHelper.Instance.DrawRectangle(origin,Size,0f,color);
        // GizmoHelper.Instance.DrawRectangle(origin + RayInfo.direction * Distance,Size,0f,color);
    }

    public override bool Cast(Vector3 position, out RaycastHit hit)
    {
        var origin = position + RayInfo.origin;
        return Physics.BoxCast(origin,Size,RayInfo.direction,out hit,Quaternion.identity,
                                Distance,CheckLayer);
    }

    public BoxRayEx(Ray ray, float dist, Vector3 size, LayerMask layer) : base(ray,dist,layer)
    {
        Size = size;
    }
}
