using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMenu : MonoBehaviour,ITriggerEnter
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;


    [SerializeField] public Transform hitAnchor;//punto desde donde sale el ataque ( modificar en la escena)
    [SerializeField] public Transform bottomAnchor;//punto en los pies (amarillo) 
    [SerializeField] public Vector2 hitSize;//area del golpe

    private readonly int IdleSide = Animator.StringToHash("Player_Idle");
    private readonly int IdleFront = Animator.StringToHash("Player_Idle_Front");
    private readonly int IdleBack = Animator.StringToHash("Player_Idle_Back");

    private readonly int WalkSide = Animator.StringToHash("Player_Walk");
    private readonly int WalkFront = Animator.StringToHash("Player_Walk_Front");
    private readonly int WalkBack = Animator.StringToHash("Player_Walk_Back");


    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Vector2 _animDir = Vector2.right;//dir del raton
    private int _currentHash = -1;//para los cross

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (!anim)
        {
            anim = GetComponentInChildren<Animator>(true);
        }
        if (!sprite)
        {
            sprite = GetComponentInChildren<SpriteRenderer>(true);
        }

        
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }
    
    private void OnEnable()//suscripciones
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractPerformed += Interact;
        }
    }
    
    private void OnDisable()//desuscripciones
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractPerformed -= Interact;
        }
    }

    private void Interact()
    {
        //TODO: Implentar el funcionamiento de la interaccion

    }

    private void Update()
    {
    //dir por teclado
        Vector2 raw = Vector2.zero;

        if (InputManager.Instance != null)
        {
            raw = InputManager.Instance.GetMovement();
        }
            
        _movement = FourD(raw);

        //dir del raton
        if (InputManager.Instance != null)
        {
            Vector2 mouseWorld = (Vector2)InputManager.Instance.GetPointerWorldPosition();
            Vector2 dir = mouseWorld - (Vector2)transform.position;
            if (dir.sqrMagnitude > 0.0001f) _animDir = dir.normalized;
        }

        //flipX
        float xForFlip = IsNearlyZero(_movement) ? _animDir.x : _movement.x;

        if (xForFlip < -0.01f)
        {
            sprite.flipX = true;
        }
        else if (xForFlip > 0.01f)
        {
            sprite.flipX = false;
        }

        //elige la anim y la orientacion
        if (IsNearlyZero(_movement))
        {
            int target = IsVerticalDominant(_animDir)? (_animDir.y >= 0f ? IdleBack : IdleFront):IdleSide;
            PlayIfChanged(target, IdleSide, 0.15f);
        }
        else
        {
            int target = IsVerticalDominant(_movement)? (_movement.y >= 0f ? WalkBack : WalkFront): WalkSide;
            PlayIfChanged(target, WalkSide, 0.10f);
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _movement * moveSpeed;
    }

    //solo 4 dir y bloquea diagonales
    private Vector2 FourD(Vector2 raw)
    {
        if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y)) 
        { 
            return new Vector2(Mathf.Sign(raw.x), 0f);
        }
        if (Mathf.Abs(raw.y) > 0f)
        {
            return new Vector2(0f, Mathf.Sign(raw.y));
        }

        return Vector2.zero;
    }

    private static bool IsNearlyZero(Vector2 v)
    {
        return v.sqrMagnitude < 0.0001f;
    }

    private static bool IsVerticalDominant(Vector2 v)
    {
        return Mathf.Abs(v.y) >= Mathf.Abs(v.x);
    }


    //fallback(cross)
    private void PlayIfChanged(int targetHash, int fallbackHash, float fade)
    {
        if (_currentHash == targetHash)
        {
            return;
        }

        if (anim && anim.runtimeAnimatorController && anim.HasState(0, targetHash))
        {
            anim.CrossFadeInFixedTime(targetHash, fade);
        }
        else
        {
            anim.CrossFadeInFixedTime(fallbackHash, fade);
        }

        _currentHash = targetHash;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitAnchor.position, hitSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(bottomAnchor.position, new Vector2(0.1f, 0.1f));
    }
    public void HitByPlayer(GameObject player)
    {
        //Implementar aqui las interracciones con el trigger
    }
}
