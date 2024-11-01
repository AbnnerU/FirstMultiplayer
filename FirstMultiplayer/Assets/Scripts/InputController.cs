using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private bool active = true;

    private Controls controls;

    //Gameplay
    public Action<Vector2> OnMoveInputUpdate;
    public Action<Vector2> OnMouseDeltaUpdate;
    public Action<Vector2> OnMousePositionUpdate;
    public Action OnJump;
    public Action OnWeaponChange;

    //public Action OnPauseEvent;

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();

        //inputActived = InputMapActive.GAMEPLAY;

        //---------------Gameplay---------------
        //Perfomed
        controls.Gameplay.Move.performed += Move;
        controls.Gameplay.Mouse.performed += Mouse;
        controls.Gameplay.Jump.performed += Jump;
        controls.Gameplay.WeaponChange.performed += WeaponChange_performed;
        controls.Gameplay.MousePosition.performed += MousePosition_performed;

        //Cancel
        controls.Gameplay.Move.canceled += Move;

    }

    private void MousePosition_performed(InputAction.CallbackContext context)
    {
        OnMousePositionUpdate?.Invoke(context.ReadValue<Vector2>());
    }

    private void WeaponChange_performed(InputAction.CallbackContext context)
    {
        OnWeaponChange?.Invoke();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void Mouse(InputAction.CallbackContext context)
    {
     
       OnMouseDeltaUpdate?.Invoke(context.ReadValue<Vector2>());    
    }

    private void Move(InputAction.CallbackContext context)
    {
        //print(context.ReadValue<Vector2>());
        OnMoveInputUpdate?.Invoke(context.ReadValue<Vector2>());
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

}
