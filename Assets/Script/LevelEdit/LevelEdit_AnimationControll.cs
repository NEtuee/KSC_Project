using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEdit_AnimationControll : MonoBehaviour
{
    [System.Serializable]
    public class AnimationItem
    {
        public string animation;
        public int layer = 0;
        public bool isEnd = false;
        public UnityEvent beginEvent;
        public UnityEvent endEvent;
    };

    [SerializeField]private List<AnimationItem> animationList;
    private Animator animator;
    private AnimationItem currentAnimation;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        //animator.
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && 
            animator.GetCurrentAnimatorStateInfo(0).IsName(currentAnimation.animation) && !currentAnimation.isEnd)
        {
            currentAnimation.endEvent.Invoke();
            currentAnimation.isEnd = true;
        }
    }

    public void PlayAnimation(string aniName)
    {
        animator.Play(aniName,1);
    }

    public void ChangeAnimation(string ani)
    {
        int pos = FindAnimation(ani);
        if(pos == -1)
        {
            Debug.Log("ani is null");
            return;
        }

        currentAnimation = animationList[pos];
        currentAnimation.beginEvent.Invoke();

        currentAnimation.isEnd = false;

        animator.SetInteger("Target",pos);
        animator.SetTrigger("Changed");
    }

    public void SetTrigger(string s)
    {
        animator.SetTrigger(s);
    }

    public void PlayStepAnimation(bool leg)
    {
        ChangeAnimation(leg ? "LeftStep" : "RightStep");
    }

    public int FindAnimation(string n)
    {
        return animationList.FindIndex(x=> x.animation == n);
    }
}
