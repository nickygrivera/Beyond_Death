using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputActions _inputActions;

    //Aquí ponemos todas las acciones puntuales que vaya a usar nuestra aplicación
    public Action JumpPerformed,
        FirePerformed,
        PausePerformed,
        UnPausePerformed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        _inputActions = new InputActions();
        _inputActions.Player.Enable();
    }

    private void Start()
    {
        _inputActions.Player.Jump.performed += JumpOnPerformed;
        _inputActions.Player.Fire.performed += FireOnPerformed;
        //_inputActions.Player.Pause.performed += PauseOnPerformed;
        //_inputActions.UI.UnPause.performed += UnPauseOnPerformed;
    }

    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {
        PausePerformed?.Invoke();
        SwitchPlayerToUI();
    }

    private void UnPauseOnPerformed(InputAction.CallbackContext obj)
    {
        UnPausePerformed?.Invoke();
        SwitchUIToPlayer();
    }

    private void FireOnPerformed(InputAction.CallbackContext obj)
    {
        FirePerformed?.Invoke();
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        JumpPerformed?.Invoke();
    }

    public Vector2 GetMovement()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }
    
    public Vector2 GetLook()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }

    public void SwitchUIToPlayer()
    {
        _inputActions.UI.Disable();
        _inputActions.Player.Enable();
    }

    private void SwitchPlayerToUI()
    {
        _inputActions.Player.Disable();
        _inputActions.UI.Enable();
    }
}
