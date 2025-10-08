using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * */
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private InputSystem_Actions _inputSystemActions;//es el input actions pero ya estaba generado con este nombre
    
    //acciones
    public Action AttackPerformed;
    public Action AttackDistancePerformed;
    public Action DashPerformed;
    
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

        _inputSystemActions = new InputSystem_Actions();

        _inputSystemActions.Player.Enable();
    }

    private void Start()
    {
        //suscripciones 
        _inputSystemActions.Player.Attack.performed += AttackOnPerformed;
        _inputSystemActions.Player.AttackDistance.performed += AttackDistanceOnPerformed;
        _inputSystemActions.Player.Dash.performed += DashOnPerformed;

        // en caso de aniadir pause y onpause
        // _input.Player.Pause.performed += PauseOnPerformed;
        // _input.UI.OnPause.performed += UnPauseOnPerformed;
    }
    //hay que desuscribirse con un Destroy para mas escenas
    private void AttackOnPerformed(InputAction.CallbackContext obj)
    {
        AttackPerformed?.Invoke();
    }

    private void AttackDistanceOnPerformed(InputAction.CallbackContext obj)
    {
        AttackDistancePerformed?.Invoke();
    }
    
    private void DashOnPerformed(InputAction.CallbackContext obj)
    {
        DashPerformed?.Invoke();
    }
    
    //lectura de eventos
    public Vector2 GetMovement()
    {
        return _inputSystemActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLook()
    {
        return _inputSystemActions.Player.Look.ReadValue<Vector2>();
    }

    //cambiar de player y ui
    public void SwitchUIToPlayer()
    {
        _inputSystemActions.UI.Disable();
        _inputSystemActions.Player.Enable();
    }
    public void SwitchPlayerToUI()
    {
        _inputSystemActions.Player.Disable();
        _inputSystemActions.UI.Enable();
    }

    public Vector3 GetPointerWorldPosition(Camera cam = null)
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        Vector2 screen = _inputSystemActions.Player.Look.ReadValue<Vector2>();

        float z = Mathf.Abs(cam.transform.position.z);
        return cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, z));
    }

}