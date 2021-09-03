using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImmortalJirungE_V2_master : ObjectBase
{
    public List<ImmortalJirungE_V2_AI> aIs;
    public BezierLightning lightning;
    public Transform lightningPoint;
    public LayerMask wallLayer;

    public UnityEvent whenAllShieldDestroy;

    public float explosionRadius = 3f;
    
    private bool allShieldBroke = false;

    private int shieldCount = 0;

    private TimeCounterEx _timeCounterEx = new TimeCounterEx();
    private RayEx _wallRay;

    private PlayerUnit _player;

    public override void Assign()
    {
        base.Assign();
        
        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
        });
    }

    public override void Initialize()
    {
        //Recovery();
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,GetSavedNumber("PlayerManager"),null);

        _timeCounterEx.InitTimer("time",0f,Random.Range(1f,2f));
        _wallRay = new RayEx(new Ray(),0f,wallLayer);
    }

    public override void Progress(float deltaTime)
    {
        _timeCounterEx.IncreaseTimerSelf("time",out var limit,deltaTime);
        if(limit)
        {
            for(int i = 0; i < aIs.Count + 0; ++i)
            {
                if(!aIs[i].shield.isOver)
                    lightning.Active(aIs[i].transform,lightningPoint,3,0.1f,4f,0.03f);
            }

            _timeCounterEx.InitTimer("time",0f,Random.Range(0.1f,0.15f));
            
        }

        bool whip = false;
        shieldCount = 0;
        foreach(var jirung in aIs)
        {
            if(!jirung.shield.isOver)
            {
                allShieldBroke = false;
                ++shieldCount;
            }

            // if(jirung.currentState == ImmortalJirungE_V2_AI.State.WallMove)
            // {
            //     _wallRay.SetDirection((jirung.transform.position - lightningPoint.position).normalized);
            //     _wallRay.Distance = Vector3.Distance(jirung.transform.position,lightningPoint.position);

            //     if(_wallRay.Cast(lightningPoint.position,out var hit))
            //     {
            //         jirung.Dead();
            //     }
            // }
            
        }
        
        foreach(var jirung in aIs)
        {
            if (jirung.isDead)
            {
                continue;
            }
            
            if(jirung.currentState == ImmortalJirungE_V2_AI.State.FloorWhip)
            {
                whip = true;
                break;
            }
        }

        if(shieldCount == 0)
        {
            if(!allShieldBroke)
            {
                foreach(var ai in aIs)
                {
                    if(ai.isDead)
                        continue;
                    
                    ai.ChangeState(ImmortalJirungE_V2_AI.State.Stun);
                }

                

                MD.SetTimeScaleMsg data = MessageDataPooling.GetMessageData<MD.SetTimeScaleMsg>();
                    data.timeScale = 0.2f;
                    data.lerpTime = 3f;
                    data.stopTime = 0f;
                    data.startTime = 0f;
                    SendMessageEx(MessageTitles.timemanager_settimescale, GetSavedNumber("TimeManager"), data);

                whenAllShieldDestroy?.Invoke();
                allShieldBroke = true;
            }
        }

        if(!whip)
        {
            foreach(var jirung in aIs)
            {
                if (jirung.isDead)
                    continue;
                
                if(jirung.canFloorWhip && jirung.currentState == ImmortalJirungE_V2_AI.State.WallMove)
                {
                    jirung.ChangeState(ImmortalJirungE_V2_AI.State.FloorWhip);
                    break;
                }
            }
        }

        
    }

    public void Explosion(int target)
    {
        Explosion(aIs[target].lastPosition, explosionRadius,aIs[target].explosionDamage);
    }
    
    public void Explosion(Vector3 position, float radius,float damage)
    {
        
        foreach (var jirung in aIs)
        {
            var dist = Vector3.Distance(position, jirung.transform.position);

            if (dist <= radius)
            {
                if (jirung.shield.isOver)
                {
                    jirung.Dead();
                }
                else
                {
                    jirung.shield.Destroy();
                }
            }
        }

        // if (Vector3.Distance(_player.transform.position, position) <= radius)
        // {
        //     _player.TakeDamage(damage);
        //     var ragdoll = _player.GetComponent<PlayerRagdoll>();
        //     ragdoll.ExplosionRagdoll(340f,(_player.transform.position - position).normalized);
        // }
        
    }
    
    
    public void Launch()
    {
        foreach(var jirung in aIs)
        {
            jirung.ChangeState(ImmortalJirungE_V2_AI.State.Launch);
        }
    }

    public void Recovery()
    {
        // foreach (var ai in aIs)
        // {
        //     if(!ai.isDead)
        //         shieldCount++;
        // }
    }

    public void AddShieldCount()
    {
        //shieldCount++;
    }
    
    public void DecreaseShieldCount()
    {
        // --shieldCount;
        // if(shieldCount == 0)
        // {
        //     foreach(var ai in aIs)
        //     {
        //         if(ai.isDead)
        //             continue;
                
        //         ai.ChangeState(ImmortalJirungE_V2_AI.State.Stun);

        //         Recovery();
        //     }

        //     whenAllShieldDestroy?.Invoke();
        // }
    }
}
