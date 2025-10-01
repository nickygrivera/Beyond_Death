using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int attack;
    
    private InputSystem_Actions _inputSystemActions;
    private Rigidbody2D _rb2d;
    private Animator _animator;
    
    private Vector2 _movement;


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
        currentHealth = maxHealth;
        
    }
    
    private void Update()
    {
        MovePlayer();
    }
    
    private void MovePlayer()
    {
        _movement = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        _rb2d.linearVelocity = _movement.normalized * moveSpeed;
    }
}