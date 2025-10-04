using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/*
 * En el inspector y en las animaciones de prueba se cambian por las buenas
 * En player (root) ,apartado de Animator se le pasa el de playerAesthetics
 * En escena cambiar las Anchor para que coincidan con la del player
 * 
 * Al impotar las animaciones , se deberán cambiar por las que aparece en la zona de Animator.StringtoHash
 * por ejemplo "Player_Idle" , siguiendo esa estructura para que el codigo funcione
 * Solo esta implementado Walk por prueba
 * En la zona de Animator no hay transiciones todo lo hace por crossfadeIn
 * */

/*Depende del sistema de cambio de escenas igual habria que hacer esta clase un singlteon*/

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Player : Character
{

    [SerializeField] private float moveSpeed = 5f;
    
   // [SerializeField] private bool clampDiagonal = true;//normaliza
    [SerializeField] private Animator anim;//_anim arrastrar aqui el animator de playerAesthetics
    [SerializeField] private SpriteRenderer spriteRenderer;//para que rote visualmente al caminar (ARREGLO DEL SALTO)

    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashCooldown = 0.5f;


    //nombre EXACTOS de los estados
    private readonly int IdleAnimState = Animator.StringToHash("Player_Idle");
    private readonly int WalkAnimState = Animator.StringToHash("Player_Walk");
    private readonly int Attack1AnimState = Animator.StringToHash("Player_Attack1");
    private readonly int Attack2AnimState = Animator.StringToHash("Player_Attack2");
    private readonly int HitAnimState = Animator.StringToHash("Player_Hit");
    private readonly int DeathAnimState = Animator.StringToHash("Player_Death");
    private readonly int DashAnimState = Animator.StringToHash("Player_Dash");


    private Rigidbody2D _rb;
    private Vector2 _movement;//direccion del input
    private bool _isAttack = true;
    private bool _isDashing = false;
    private bool _canDash = true;
    
    private CharacterState _state;
    //velocity sale obsoleto y en esta versión solo deja linearVelocity

   
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        //inicializamos la vida y el daño
        SetHealthMax(GetHealthMax());
        SetDamage(GetDamage());

        _state = CharacterState.Idle;
    }



    //codigo de la plantilla del character (samurai)
    private void OnEnable()//suscripciones
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.AttackPerformed += Attack1;
            InputManager.Instance.AttackDistancePerformed += Attack2;
            InputManager.Instance.DashPerformed += OnDashInput;
        }
    }



    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.AttackPerformed -= Attack1;
            InputManager.Instance.AttackDistancePerformed -= Attack2;
            InputManager.Instance.DashPerformed -= OnDashInput;
        }
    }



    private void Update()
    {
        if (_state == CharacterState.Hurt || _state == CharacterState.Die)
        {
            return;
        }

        //lectura de teclas y normaliza para evitar lo de la diagonal
        _movement = InputManager.Instance != null ? InputManager.Instance.GetMovement() : Vector2.zero;
        if (_movement.sqrMagnitude > 1f) _movement.Normalize();

        /*
         * 
        //flippor rotacion en Y
        if (_state != CharacterState.Attack)
        {
            if (_movement.x < -0.01f) transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (_movement.x > 0.01f) transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        */

        //cambio flip visualmente no por player(root)
        if (_state != CharacterState.Attack && _state != CharacterState.Dash)
        {
            if (_movement.x < -0.01f) spriteRenderer.flipX = true;
            else if (_movement.x > 0.01f) spriteRenderer.flipX = false;
        }


        //change anim de idle o walk por  rossfade
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
        if (_state == CharacterState.Hurt || _state == CharacterState.Die)
        {
            return;
        }
        if (_state == CharacterState.Attack)
        {
            _rb.linearVelocity = Vector2.zero;
        }
        else if (!_isDashing)
        {
            _rb.linearVelocity = _movement * moveSpeed;
        }

    }


    //En la animacion de prueba del dash hace un frontflip
    //Dash del player
    private void OnDashInput()
    {
        Debug.Log("Dash");
        if (!_canDash || _state == CharacterState.Attack || _state == CharacterState.Hurt || _state == CharacterState.Die)
        {
            return;
        }

        StartCoroutine(Dash());
    }
    
    private IEnumerator Dash()
    {
        Debug.Log("Dashing");
        _isDashing = true;
        _canDash = false;

        //anim del dash
        _state = CharacterState.Dash;
        anim.CrossFadeInFixedTime(DashAnimState, 0f);


        _rb.linearVelocity = _movement * dashSpeed;//dash
        yield return new WaitForSeconds(dashCooldown);
        _isDashing = false;

        //vuelve a idle o walk segun teclado
        if (_movement == Vector2.zero)
        {
            _state = CharacterState.Idle;
            anim.CrossFadeInFixedTime(IdleAnimState, 0.1f);
            _rb.linearVelocity = Vector2.zero;
        }
        else
        {
            _state = CharacterState.Walk;
            anim.CrossFadeInFixedTime(WalkAnimState, 0.1f);
            _rb.linearVelocity = _movement * moveSpeed;
        }


        _canDash = true;
    }
    
    

    public override void Attack(){
        Attack1();
    }


    private void Attack1()//ataque meele
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
            {
                col.GetComponent<ITriggerEnter>()?.HitByPlayer(gameObject);
            }

            StartCoroutine(WaitForAnimationToEnd(Attack1AnimState));
        }
    }

    private void Attack2()//ataque distancia
    {
        if (!_isAttack || _state == CharacterState.Attack || _state == CharacterState.Die) return;

        _state = CharacterState.Attack;
        _isAttack = false;
        _rb.linearVelocity = Vector2.zero;

        anim.CrossFadeInFixedTime(Attack2AnimState, 0f);
        StartCoroutine(WaitForAnimationToEnd(Attack2AnimState));
    }

    //espera a que acabe las animaciones para estar en idle
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



    //overrides de character
    public override void TakeDamage(float dmg)
    {
        if (_state == CharacterState.Attack || _state == CharacterState.Die)
        {
            return;
        }

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

    public override void Die()
    {
        Debug.Log("Player muerto");
        //TODO:codigo reiniciar todas las escenas si muere 
        //TODO:lanzar escena o UI de Game over
    }
}
