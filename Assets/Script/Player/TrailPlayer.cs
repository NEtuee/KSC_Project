using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailPlayer : MonoBehaviour
{
    public Transform container;
    public Transform originRoot;
    public Transform destRoot;

    public List<Renderer> renderers = new List<Renderer>();
    public List<Material> mats = new List<Material>();

    public float fadeSpeed = 2f;

    private void Awake()
    {
        destRoot.SetParent(container);

        for(int i = 0; i<renderers.Count;i++)
        {
            renderers[i].enabled = false;
            mats.Add(renderers[i].material);
        }
    }

    public void Active(Transform originTransform)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].enabled = true;
            mats[i].SetFloat("FadeAmount", 0.0f);
        }

        //transform.SetPositionAndRotation(originTransform.position, originTransform.rotation);
        destRoot.position = originRoot.position;
        destRoot.rotation = originRoot.rotation;
        CopyBoneTransform(originRoot, destRoot);
        StartCoroutine(Fade());
    }

    private void CopyBoneTransform(Transform origin, Transform dest)
    {
        int count = origin.transform.childCount < dest.transform.childCount ? origin.transform.childCount : dest.transform.childCount;
        for (int i = 0; i< count; i++)
        {
            if(count != 0)
            {
                CopyBoneTransform(origin.transform.GetChild(i), dest.transform.GetChild(i));
            }
            dest.transform.GetChild(i).localPosition = origin.transform.GetChild(i).localPosition;
            dest.transform.GetChild(i).localRotation = origin.transform.GetChild(i).localRotation;
        }
    }

    private IEnumerator Fade()
    {
        float amount = 0.0f;

        while(amount < 1.0f)
        {
            amount += fadeSpeed * Time.deltaTime;
            amount = Mathf.Clamp(amount, 0.0f, 1.0f);
            for (int i = 0; i < mats.Count; i++)
            {
                mats[i].SetFloat("FadeAmount", amount);
            }
            yield return null;
        }

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].enabled = false;
        }
    }
}
