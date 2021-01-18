using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_BehaviorControll : MonoBehaviour
{
    public enum State
    {
        Idle,
        Step,
        StepIdle,
        MoveEnd,
        Pattern,
        Disturbance,
        Dead,
    };

    [SerializeField]private EventSequencer eventSequencer;
    [SerializeField]private GameObject explosionModel;

    [SerializeField]private EffectGenerator bridgeEffectGenerator;

    [SerializeField]private GameObject footstepEffect;
    [SerializeField]private Transform leftFootPoint;
    [SerializeField]private Transform rightFootPoint;
    [SerializeField]private LevelEdit_RayFixParticle particle;


    [SerializeField]private AnimationCurve speedCurve;
    [SerializeField]private State state;
    [SerializeField]private State prevState;
    [SerializeField]private string startAnimation = "Idle";
    [SerializeField]private float stepSpeed;
    [SerializeField]private float stepCooldown;
    [SerializeField]private float stepProgressTime;
    [SerializeField]private float arriveDistance;

    [SerializeField] private Renderer eyeObjectRenderer;
    [SerializeField] private float eyeEmissionChangeSpeed = 2f;
    [SerializeField] Transform headLookEventTransform;
    private Material eyeMat;
    private Color targetColor = new Color(4.0f, 0f, 0f);
    private Color startColor = new Color(1.465969f, 1.465969f, 1.465969f);

    private List<LevelEdit_MovePoint> movePoints;

    private int movePoint;
    private int prevPoint;

    private LevelEdit_AnimationControll animator;

    private Vector3 moveDirection;

    private float stepIdleTimer = 0f;
    private float stepTimer = 0f;
    private float targetAngle = 0f;

    private float movePointTimer = 0f;
    private float curveLength = 0f;

    private bool stepFoot = false; //0 left 1 right
    private bool arrive = false;

    private int playerLayer;

    public delegate void OnShakeEvent(Vector3 thisPos);
    public OnShakeEvent OnShake;

    private CameraCollision mainCamera;
    private CameraCtrl camRoot;

    private bool firstFootStepDone;

    private void Start()
    {
        animator = GetComponent<LevelEdit_AnimationControll>();
        animator.ChangeAnimation(startAnimation);

        playerLayer = (1 << LayerMask.NameToLayer("Player")) +(1 << LayerMask.NameToLayer("Characters"));

        eyeMat = eyeObjectRenderer.material;
        if(eyeMat != null)
        {
            eyeMat.SetColor("_EmissionColor", startColor);
        }

        mainCamera = Camera.main.GetComponent<CameraCollision>();

        if(Camera.main.transform.parent != null)
            camRoot = Camera.main.transform.parent.GetComponent<CameraCtrl>();
    }

    public void Progress()
    {
        if(state == State.Idle)
        {

        }
        else if(state == State.Step)
        {
            stepTimer += Time.deltaTime;
            var progress = stepTimer / stepProgressTime;
            var speed = stepSpeed * speedCurve.Evaluate(progress);

			movePointTimer += speed * Time.deltaTime / curveLength;

            if(movePointTimer >= 1f)
			{
				arrive = true;
                UpdateMovePoint();
			}

			var targetPos = MathEx.Vector2ToVector3(MathEx.GetPointOnBezierCurve2D(movePoints[prevPoint].transform.position,
                                            movePoints[prevPoint].GetBezierPoint1().position,
                                            movePoints[prevPoint].GetBezierPoint2().position,
                                            movePoints[movePoint].transform.position,movePointTimer));

            var dir = (targetPos - MathEx.DeleteYPos(transform.position)).normalized;
            dir.y = dir.z;
            dir.z = dir.x;
            dir.x = dir.y;

            targetAngle = MathEx.directionToAngle(MathEx.Vector3ToVector2(dir));

            targetPos.y = transform.position.y;
            transform.position = targetPos;

            if(progress >= 1f)
            {
                stepIdleTimer = 0f;
                SetState(State.StepIdle);

                particle.StopParticle();
            }

            // if(!arrive)
            // {
            //     var dist = MathEx.xzDistance(transform.position, movePoint.GetPoint());

            //     if(dist <= arriveDistance)
            //     {
            //         arrive = true;
            //     }
            // }

            transform.eulerAngles = new Vector3(0f,targetAngle,0f);
        }
        else if(state == State.StepIdle)
        {
            stepIdleTimer += Time.deltaTime;
            if(stepIdleTimer >= stepCooldown)
            {
                SetState(arrive ? State.Step : State.Step);
                stepTimer = 0f;
            }
        }
    }

    public void InitializeStepIdleTimer()
    {
        stepIdleTimer = -3f;
    }

    public void UpdateMovePoint()
    {
        prevPoint = movePoint;
        movePoint = movePoint + 1 >= movePoints.Count ? 0 : movePoint + 1;

        movePointTimer -= 1f;
        movePointTimer = movePointTimer * curveLength;

        CalcCurveLength();
        
        movePointTimer = movePointTimer / curveLength;
    }

    public void CalcCurveLength()
    {
        curveLength = MathEx.GetBezierLengthStupidWay2D(movePoints[prevPoint].transform.position,
                                                        movePoints[prevPoint].GetBezierPoint1().position,
                                                        movePoints[prevPoint].GetBezierPoint2().position,
                                                        movePoints[movePoint].transform.position,
                                                        10);
    }

    public void CreateFootstepEffect()
    {
        GameObject obj = Instantiate(footstepEffect);

        if(stepFoot)
        {
            AudioManager.instance.Play("BossWalk",leftFootPoint.position);
            AudioManager.instance.Play("EarthQuake",leftFootPoint.position);

            obj.transform.position = leftFootPoint.position;
            OnShake?.Invoke(leftFootPoint.position);
            GenerateQuake(leftFootPoint.position);
        }
        else
        {
            AudioManager.instance.Play("BossWalk",rightFootPoint.position);
            AudioManager.instance.Play("EarthQuake",rightFootPoint.position);

            obj.transform.position = rightFootPoint.position;
            OnShake?.Invoke(rightFootPoint.position);
            GenerateQuake(rightFootPoint.position);
        }

        Destroy(obj,5f);
    }

    public void BodyChange()
    {
        SetState(State.Dead);
        gameObject.SetActive(false);
        explosionModel.transform.position = transform.position;
        explosionModel.transform.rotation = transform.rotation;
        explosionModel.SetActive(true);
    }

    public void BridgeSmashEvent()
    {
        bridgeEffectGenerator.CreateEffectObject(0);
        AudioManager.instance.Play("BossWalk",bridgeEffectGenerator.transform.position);
        AudioManager.instance.Play("EarthQuake",bridgeEffectGenerator.transform.position);
    }

    public void ExplosionProgress()
    {
        eventSequencer.StartEvent();
    }

    public void PlayAnimation(string aniName)
    {
        animator.PlayAnimation(aniName);
    }

    public void ChangeAnimation(string ani)
    {
        animator.ChangeAnimation(ani);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    public void SetPrevState()
    {
        SetState(prevState);
    }

    public void SetStepFoot(bool side)
    {
        stepFoot = side;
    }

    public void SetStepFootRight()
    {
        SetStepFoot(true);
    }

    public void SetState(State s)
    {

        Debug.Log(s);
        if(state == s)
            return;

        prevState = state;

        state = s;

        if(state == State.Step)
        {
            stepFoot = !stepFoot;
            stepTimer = 0f;
            particle.PlayParticle();
            animator.PlayStepAnimation(stepFoot);

            if(AudioManager.instance != null)
                AudioManager.instance.Play("BossClack", stepFoot ? rightFootPoint.position : leftFootPoint.position);
        }
    }

    public State GetState()
    {
        return state;
    }

    public void SetMoveInfo(List<LevelEdit_MovePoint> points)
    {
        movePoints = points;
        movePoint = 1;
        prevPoint = 0;

        SetState(State.Step);
        arrive = false;
        
        CalcCurveLength();
    }

    private void GenerateQuake(Vector3 generatePosition)
    {
        Collider[] colliders = Physics.OverlapSphere(generatePosition, 15.0f, playerLayer);
        if(colliders.Length != 0)
        {
            for(int i = 0; i<colliders.Length;i++)
            {
                //colliders[i].GetComponent<PlayerCtrl>().Quake();
                colliders[i].GetComponent<PlayerCtrl_State>().Quake();
            }
        }
    }

    private void FlashBossEye()
    {
        if(eyeMat != null)
        StartCoroutine(ChangeEyeEmission());
        //mainCamera.LookingEvent(head, 3f);
        //camRoot.LookingEvent(headLookEventTransform, 3f);
    }

    private void LookHead()
    {
        //Debug.Log((headLookEventTransform.position - GameManager.Instance.GetPlayerObject().transform.position).magnitude);
        if((headLookEventTransform.position - GameManager.Instance.GetPlayerObject().transform.position).magnitude> 25f)
        {
            return;
        }

        mainCamera.ForceZoomOut();
        camRoot.LookingEvent(headLookEventTransform, 6f);
    }

    IEnumerator ChangeEyeEmission()
    {
        Color eyeEmission = eyeMat.GetColor("_EmissionColor");
        while(eyeEmission != targetColor)
        {
            eyeEmission = Color.Lerp(eyeEmission, targetColor, eyeEmissionChangeSpeed * Time.deltaTime);
            eyeMat.SetColor("_EmissionColor",eyeEmission);
            yield return null;
        }
    }

    private void LeftFootStepEvent()
    {
        //Debug.Log("LeftFootStepEvent");
        //if (firstFootStepDone == false)
        //{
        //    GameManager.Instance.RequstCameraShakeByFactor(0.5f, 2.5f);
        //    firstFootStepDone = true;
        //}

        if (leftFootPoint != null)
        {
            Collider[] playerColl = Physics.OverlapSphere(leftFootPoint.position, 4.5f,playerLayer);

            if(playerColl.Length != 0)
            {
                //playerColl[0].GetComponent<PlayerCtrl>().NuckBack(leftFootPoint.position);
                playerColl[0].GetComponent<PlayerCtrl_State>().NuckBack(leftFootPoint.position);
            }
        }
    }

    private void RightFootStepEvent()
    {
        //Debug.Log("RightFootStepEvent");
        if (leftFootPoint != null)
        {
            Collider[] playerColl = Physics.OverlapSphere(rightFootPoint.position, 4.5f, playerLayer);

            if (playerColl.Length != 0)
            {
                //playerColl[0].GetComponent<PlayerCtrl>().NuckBack(rightFootPoint.position);
                playerColl[0].GetComponent<PlayerCtrl_State>().NuckBack(rightFootPoint.position);
            }
        }
    }
}
