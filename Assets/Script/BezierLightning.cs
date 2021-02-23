using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLightning : MonoBehaviour
{
    private class LightningPack
    {
        public List<LineRenderer> usingLineRenderers = new List<LineRenderer>();
        public float timer = 0f;

        private float _time = 0f;
        public void Set(float t)
        {
            _time = t;
            timer = 0f;
        }

        public bool Progress(float deltaTime)
        {
            _time -= deltaTime;

            foreach(var line in usingLineRenderers)
            {

            }

            if(_time <= 0f)
            {
                return true;
            }

            return false;
        }
    }

    public LineRenderer reference;
    public Material lightningMat;
    
    private List<LightningPack> _progressPack = new List<LightningPack>();

    private Queue<LineRenderer> _lightningCache = new Queue<LineRenderer>();
    private Queue<LightningPack> _packCache = new Queue<LightningPack>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            Active(Vector3.zero,new Vector3(10f,10f,10f),5,1f,10f);
        }

        for(int i = 0; i < _progressPack.Count;)
        {
            if(_progressPack[i].Progress(Time.deltaTime))
            {
                ReturnCache(_progressPack[i]);
                _progressPack.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }

    public void Active(Vector3 start,Vector3 end, int accur, float time, float randomFactor)
    {
        var pack = CreateLightning(start,end,accur,randomFactor);
        pack.Set(time);
        _progressPack.Add(pack);
    }

    private void ReturnCache(LightningPack pack)
    {
        ReturnCache(ref pack.usingLineRenderers);
        pack.Set(0);
        _packCache.Enqueue(pack);
    }

    private void ReturnCache(ref List<LineRenderer> lines)
    {
        foreach(var line in lines)
        {

            line.gameObject.SetActive(false);

            _lightningCache.Enqueue(line);
        }

        lines.Clear();
    }

    private LightningPack CreateLightning(Vector3 start, Vector3 end,int accur, float randomFactor)
    {
        var line = GetLineRenderer();
        var bezier_0 = Vector3.Lerp(start,end,0.3f) + MathEx.RandomCircle(randomFactor);
        var bezier_1 = Vector3.Lerp(start,end,0.6f) + MathEx.RandomCircle(randomFactor);

        line.positionCount = accur + 1;
        for(int i = 0; i <= accur; ++i)
        {
            line.SetPosition(i,MathEx.GetPointOnBezierCurve(start,bezier_0,bezier_1,end,(float)i / (float)accur));
        }

        var pack = GetPack();
        pack.usingLineRenderers.Add(line);

        return pack;
    }

    private LightningPack GetPack()
    {
        if(_packCache.Count == 0)
            CreatePackItems(1);

        return _packCache.Dequeue();
    }

    private LineRenderer GetLineRenderer()
    {
        if(_lightningCache.Count == 0)
            CreateLineRendererItems(1);

        var item = _lightningCache.Dequeue();
        item.gameObject.SetActive(true);
        
        return item;
    }

    private void CreatePackItems(int count)
    {
        for(int i = 0; i < count; ++i)
        {
            LightningPack pack = new LightningPack();
            _packCache.Enqueue(pack);
        }
    }

    private void CreateLineRendererItems(int count)
    {
        for(int i = 0; i < count; ++i)
        {
            var line = new GameObject("LineCache").AddComponent<LineRenderer>();
            line.startWidth = reference.startWidth;
            line.endWidth = reference.endWidth;
            line.material = lightningMat;
            line.useWorldSpace = reference.useWorldSpace;

            line.gameObject.SetActive(false);

            _lightningCache.Enqueue(line);
        }
    }
}
