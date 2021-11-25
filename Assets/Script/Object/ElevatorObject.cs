using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ElevatorObject : ObjectBase
{
    public InputAction interactionAction;
    public LevelEdit_TimelinePlayer timelinePlayer;
    public bool interaction = false;
    public bool complete = false;
    public GameObject canvas;
    public TextMeshProUGUI text;

    public override void Assign()
    {
        base.Assign();

        interactionAction.performed += _ => 
        {
            if(complete == false && interaction == true)
            {
                canvas.SetActive(false);
                timelinePlayer.Play();
                complete = true;
            }
        };
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));
    }

    private void OnEnable()
    {
        interactionAction.Enable();
    }

    private new void OnDisable()
    {
        base.OnDisable();
        interactionAction.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            interaction = true;
            canvas.SetActive(true);
            if (PlayerUnit.GamepadMode == true)
                text.text = "X";
            else
                text.text = "F";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interaction = false;
            canvas.SetActive(false);
        }
    }
}
