using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHead : MonoBehaviour
{
    public List<IKFootPointRotator> allParts = new List<IKFootPointRotator>();

    public AnimationCurve twistCurve;
    public AnimationCurve twistUpCurve;

    public float farDistance = 5f;

    private float _targetHeight;
    private float _twistTimer = 0f;
    private bool _twist = false;

    private void Start()
    {
        _targetHeight = allParts[0].baseHeight;
    }

    void Update()
    {
        allParts[0].baseHeight = Mathf.Lerp(allParts[0].baseHeight,_targetHeight,0.1f);

        for(int i = 1; i < allParts.Count; ++i)
        {
            var one = allParts[i - 1].transform.position;
            var two = allParts[i].transform.position;
            var direction = (one - two).normalized;
            var dist = Vector3.Distance(one,two);

            if(dist >= farDistance)
            {
                allParts[i].transform.position = one - direction * farDistance;
            }

            var angle = Vector3.SignedAngle(allParts[i].transform.forward,Vector3.ProjectOnPlane(direction,allParts[i].transform.up),allParts[i].transform.up);
            allParts[i].transform.RotateAround(allParts[i].transform.position,allParts[i].transform.up,angle);
            allParts[i].baseHeight = Mathf.Lerp(allParts[i].baseHeight, allParts[i - 1].baseHeight, 0.05f);
        }

        if(_twist)
        {
            _twistTimer += Time.deltaTime * .5f;
            for(int i = 1; i < allParts.Count; ++i)
            {
                allParts[i].transform.position = MathEx.easeOutCubicVector3(allParts[i].transform.position,
                                                            allParts[i].transform.position + (allParts[i].transform.right * Mathf.Sin(i * 1f)) * twistCurve.Evaluate(_twistTimer),
                                                            _twistTimer);

                allParts[i].baseHeight = _targetHeight + twistUpCurve.Evaluate(_twistTimer * 2.3f - (i * 0.1f));

                allParts[i].transform.LookAt(allParts[i - 1].transform.position,allParts[i].transform.up);
            }

            if(_twistTimer >= 1f)
            {
                _twist = false;
            }
        }
    }

    public void SetHeightInOrder(float height)
    {
        _targetHeight = height;
    }

    public void StartTwist()
    {
        _twist = true;
        _twistTimer = 0f;
    }
}
