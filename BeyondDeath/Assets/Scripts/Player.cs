using UnityEngine;
using System.Collections;

public class Player : Character
{
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashCooldown = 0.25f;
    
    private InputSystem_Actions _inputSystemActions;
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private Vector2 _movement;
    private bool _isDashing = false;
    private bool _canDash = true;
    
    private void Awake()
    {
        _inputSystemActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputSystemActions.Enable();
    }

    private void OnDisable()
    {
        _inputSystemActions.Disable();
    }
    
    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        MovePlayer();
    }
    
    private void MovePlayer()
    {
        _movement = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        
        if (_inputSystemActions.Player.Dash.triggered && _canDash)
        {
            StartCoroutine(Dash());
        }
        else if(!_isDashing)
        {
            _rb2d.linearVelocity = _movement.normalized * moveSpeed;
        }
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _canDash = false;
        _rb2d.linearVelocity = _movement.normalized * dashSpeed;
        yield return new WaitForSeconds(dashCooldown);
        _isDashing = false;
        _canDash = true;
    }
}