using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMove : MonoBehaviour
{
    [SerializeField] private float floatingSpeed;
    private Vector3 startPosition;
    [SerializeField]private float range;
    [SerializeField] private float rangeRatio = 1f;

    private void Start()
    {
        startPosition = transform.localPosition;
    }
    void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        float adjust = Mathf.Sin(Time.time * floatingSpeed);
        transform.localPosition = startPosition + Vector3.up *range*rangeRatio* adjust;
    }

    public void SetRangeRatio(float ratio)
    {
        rangeRatio = ratio;
    }
    
}
