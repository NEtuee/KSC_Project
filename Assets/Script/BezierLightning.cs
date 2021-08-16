using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLightning : MonoBehaviour
{
    private class LightningPack
    {
        public List<LineRenderer> usingLineRenderers = new List<LineRenderer>();
        public float timer = 0f;
        public float updateTimer = 0f;

        private float _time = 0f;
        private float _updateTime = 0f;

        public bool keepUpdate = false;

        public int accuracy;
        public float randomFactor;

        public Transform startPoint;
        public Transform endPoint;

        public void Set(float t,float updateTime = 0f,bool update = false)
        {
            _time = t;
            timer = 0f;

            _updateTime = updateTime;
            updateTimer = 0f;
            keepUpdate = update;
        }

        public void Set(Transform start, Transform end,float time, float updateTime,bool update = true)
        {
            startPoint = start;
            endPoint = end;
            _time = time;
            _updateTime = updateTime;
            timer = 0f;
            updateTimer = 0f;

            keepUpdate = update;
        }

        public bool UpdateTimeProgress(float deltaTime)
        {
            updateTimer += deltaTime;
            if(_updateTime <= updateTimer)
            {
                updateTimer = 0f;
                return true;
            }

            return false;
        }

        public bool Progress(float deltaTime)
        {
            _time -= deltaTime;

            // foreach(var line in usingLineRenderers)
            // {

            // }

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
        for(int i = 0; i < _progressPack.Count;)
        {
            var progress = _progressPack[i];

            if(progress.keepUpdate)
            {
                if(progress.UpdateTimeProgress(Time.deltaTime))
                {
                    foreach(var line in progress.usingLineRenderers)
                    {
                        CreateLightning(line,progress.startPoint.position,progress.endPoint.position,progress.accuracy,progress.randomFactor);
                    }
                }
            }

            if(progress.Progress(Time.deltaTime))
            {
                ReturnCache(progress);
                _progressPack.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }

    public void Active(Transform start, Transform end, int accur, float time, float randomFactor, float updateTime, bool update = true)
    {
        var pack = CreateLightningPack(start.position,end.position,accur,randomFactor);
        pack.Set(start,end,time,updateTime,update);
        pack.accuracy = accur;
        pack.randomFactor = randomFactor;
        _progressPack.Add(pack);
    }

    public void Active(Vector3 start,Vector3 end, int accur, float time, float randomFactor)
    {
        var pack = CreateLightningPack(start,end,accur,randomFactor);
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

    private LineRenderer CreateLightning(LineRenderer line, Vector3 start, Vector3 end,int accur, float randomFactor)
    {
        var bezier_0 = Vector3.Lerp(start,end,0.3f) + MathEx.RandomCircle(randomFactor);
        var bezier_1 = Vector3.Lerp(start,end,0.6f) + MathEx.RandomCircle(randomFactor);

        line.positionCount = accur + 1;
        for(int i = 0; i <= accur; ++i)
        {
            line.SetPosition(i,MathEx.GetPointOnBezierCurve(start,bezier_0,bezier_1,end,(float)i / (float)accur));
        }

        return line;
    }

    private LightningPack CreateLightningPack(Vector3 start, Vector3 end,int accur, float randomFactor)
    {
        var line = GetLineRenderer();
        CreateLightning(line, start,end,accur,randomFactor);

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
