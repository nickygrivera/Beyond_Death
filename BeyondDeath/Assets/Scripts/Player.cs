using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/*
Los metodos Die() y TakeDamage() usa los del padre 
*/

/*Depende del sistema de cambio de escenas igual habria que hacer esta clase un singlteon*/

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Player : Character
{
    [SerializeField] private float moveSpeed = 5f;

   // [SerializeField] private bool clampDiagonal = true;//normaliza
    [SerializeField] private Animator anim;//_anim

    private readonly int IdleAnimState = Animator.StringToHash("Player_Idle");
    private readonly int WalkAnimState = Animator.StringToHash("Player_Walk");
    private readonly int Attack1AnimState = Animator.StringToHash("Player_Attack1");
    private readonly int Attack2AnimState = Animator.StringToHash("Player_Attack2");
    private readonly int HitAnimState = Animator.StringToHash("Player_Hit");
    private readonly int DeathAnimState = Animator.StringToHash("Player_Death");

 
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private bool _isAttack = true;

    private CharacterState _state;
    //velocity sale obsoleto y en esta versión solo me deja linearVelocity

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        SetHealthMax(GetHealthMax());
        SetDamage(GetDamage());

        _state = CharacterState.Idle;
    }

    //codigo samurai 2�
    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.AttackPerformed += Attack1;
            InputManager.Instance.AttackDistancePerformed += Attack2;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.AttackPerformed -= Attack1;
            InputManager.Instance.AttackDistancePerformed -= Attack2;
        }
    }

    private void Update()
    {
        if (_state == CharacterState.Hurt || _state == CharacterState.Die) return;

        _movement = InputManager.Instance != null ? InputManager.Instance.GetMovement() : Vector2.zero;
        if (_movement.sqrMagnitude > 1f) _movement.Normalize();

        // Flip por rotaci�n en Y
        if (_state != CharacterState.Attack)
        {
            if (_movement.x < -0.01f) transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (_movement.x > 0.01f) transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Animaciones Idle / Walk
        if (_state == CharacterState.Idle || _state == CharacterState.Walk)
        {
            if (_movement == Vector2.zero && _state != CharacterState.Idle)
            {
                _state = CharacterState.Idle;
                anim.CrossFadeInFixedTime(IdleAnimState, 0.2f);
            }
            else if (_movement != Vector2.zero && _state != CharacterState.Walk)
            {
                _state = CharacterState.Walk;
                anim.CrossFadeInFixedTime(WalkAnimState, 0.2f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_state == CharacterState.Hurt || _state == CharacterState.Die) return;

        if (_state == CharacterState.Attack)
            _rb.linearVelocity = Vector2.zero;
        else
            _rb.linearVelocity = _movement * moveSpeed;
    }

    private void Attack1()
    {
        if (!_isAttack || _state == CharacterState.Attack || _state == CharacterState.Die) return;

        _state = CharacterState.Attack;
        _isAttack = false;
        _rb.linearVelocity = Vector2.zero;

        anim.CrossFadeInFixedTime(Attack1AnimState, 0f);

        //hitbox melee
        if (hitAnchor)
        {
            Vector2 forward = transform.right;
            float forwardOffset = 0.4f;
            Vector2 center = (Vector2)hitAnchor.position + forward * forwardOffset;

            Collider2D[] results = Physics2D.OverlapBoxAll(center, hitSize, 0);
            foreach (var col in results)
                col.GetComponent<ITriggerEnter>()?.HitByPlayer(gameObject);

            StartCoroutine(WaitForAnimationToEnd(Attack1AnimState));
        }
    }

    private void Attack2()
    {
        if (!_isAttack || _state == CharacterState.Attack || _state == CharacterState.Die) return;

        _state = CharacterState.Attack;
        _isAttack = false;
        _rb.linearVelocity = Vector2.zero;

        anim.CrossFadeInFixedTime(Attack2AnimState, 0f);
        StartCoroutine(WaitForAnimationToEnd(Attack2AnimState));
    }

    private IEnumerator WaitForAnimationToEnd(int animState)
    {
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.shortNameHash != animState)
        {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        _state = CharacterState.Idle;
        anim.CrossFadeInFixedTime(IdleAnimState, 0f);
        _isAttack = true;
    }

    // Overrides de Character
    public override void TakeDamage(float dmg)
    {
        if (_state == CharacterState.Attack || _state == CharacterState.Die) return;

        SetHealthActual(GetHealthActual() - dmg);

        if (GetHealthActual() <= 0f)
        {
            _state = CharacterState.Die;
            _rb.linearVelocity = Vector2.zero;
            anim.CrossFadeInFixedTime(DeathAnimState, 0f);
        }
        else
        {
            _state = CharacterState.Hurt;
            _isAttack = false;
            _rb.linearVelocity = Vector2.zero;
            anim.CrossFadeInFixedTime(HitAnimState, 0f);
            StartCoroutine(WaitForAnimationToEnd(HitAnimState));
        }
    }

    protected override void Die()
    {
        Debug.Log("Player muerto");
        //codigo reiniciar todas las escenas si muere 
    }
}
