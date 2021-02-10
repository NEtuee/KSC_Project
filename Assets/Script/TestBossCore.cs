using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossCore : MonoBehaviour
{
    public DesignTestBoss boss;
    private ExplosionTest _explosion;

    private void Start()
    {
        _explosion = GetComponent<ExplosionTest>();
    }

    public void OnTriggerStay(Collider coll)
    {
        if(coll.tag == "Player" && !boss._progress)
        {
            if(InputManager.Instance.GetAction(KeybindingActions.Interaction))
            {
                _explosion.Exlposion(transform.position);
                boss.Explosion();
            }
        }
    }
}
