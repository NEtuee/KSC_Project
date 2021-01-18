// 2020_09_13_이우민

using UnityEngine;


//스피어 케스팅 헬퍼
public class SphereRayEx : RayEx
{
    public float radius;

    public override void Draw(Vector3 position, Color color)
    {
        var origin = position + RayInfo.origin;
        GizmoHelper.Instance.DrawLine(origin, origin + RayInfo.direction * Distance,color);
        GizmoHelper.Instance.DrawCircle(origin,radius,color);
        GizmoHelper.Instance.DrawCircle(origin + RayInfo.direction * Distance,radius,color);
    }

    public override bool Cast(Vector3 position, out RaycastHit hit)
    {
        var origin = position + RayInfo.origin;
        return Physics.SphereCast(origin,radius,RayInfo.direction,out hit,Distance,CheckLayer);
    }

    public SphereRayEx(Ray ray, float dist, float rad, LayerMask layer) : base(ray,dist,layer)
    {
        radius = rad;
    }
}
