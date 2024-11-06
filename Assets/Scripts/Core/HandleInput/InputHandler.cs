using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [HideInInspector]
    public Vector2 inputDirection = Vector2.zero;

    // Events
    public event EventHandler OnJumpPreformed;
    public event EventHandler OnJumpCanceled;
    public event EventHandler OnRollPreformed;
    public event EventHandler OnInteractPreformed;
    public event EventHandler OnAttackPreformed;

    InputMap inputMap;
    
    void Start()
    {
        inputMap = new InputMap();
        inputMap.Player.Enable();

        inputMap.Player.Jump.performed += Jump_performed;
        inputMap.Player.Jump.canceled += Jump_canceled;
        inputMap.Player.Roll.performed += Roll_performed;
        inputMap.Player.Interact.performed += Interact_performed;
        inputMap.Player.Attack.performed += Attack_performed;
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackPreformed?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPreformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Roll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRollPreformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpPreformed?.Invoke(this, EventArgs.Empty);
    }

    void Update()
    {
        inputDirection = inputMap.Player.Movement.ReadValue<Vector2>();
    }
}