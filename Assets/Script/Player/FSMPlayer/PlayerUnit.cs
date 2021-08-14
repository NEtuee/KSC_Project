using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnit : UnTransfromObjectBase
{
    public string currnetStateName;

    private PlayerState _currentState;
    private PlayerState _prevState;

    public static PlayerState_Default defaultState;
    public static PlayerState_Jump jumpState;

    public float InputVertical { get => _inputVertical; }
    public float InputHorizontal { get => _inputHorizontal; }

    public Transform Transform { get => _transform; }

    /// Input
    private float _inputVertical;
    private float _inputHorizontal;

    private Transform _transform;


    public override void Initialize()
    {
        base.Initialize();

        _transform = GetComponent<Transform>();

        ChangeState(defaultState);
    }

    public void ChangeState(PlayerState state)
    {
        _prevState = _currentState;
        _prevState.Exit(this);

        _currentState = state;
        _currentState.Enter(this);
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        Vector2 inputVector = value.ReadValue<Vector2>();
        _inputVertical = inputVector.y;
        _inputHorizontal = inputVector.x;
    }

    public void OnJump(InputAction.CallbackContext value)
    {

    }
}
