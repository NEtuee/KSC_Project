using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour
{
    bool bulletTime = false;
    private void Start()
    {
        PlayerCtrl player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
    }

    public void OnBulletTime()
    {
        Time.timeScale = 0.2f;
        bulletTime = true;
    }
    public void OffBulletTime()
    {
        Time.timeScale = 1.0f;
        bulletTime = false;
    }

    public void PauseTime()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeTime()
    {
        if(bulletTime == true)
        {
            Time.timeScale = 0.2f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
