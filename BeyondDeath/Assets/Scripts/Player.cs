using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/*
 * En el inspector y en las animaciones de prueba se cambian por clips de animacion
 * Hay 3 tipos de clips de animaciones:
 *
 * Es decir que cada accion como por ejemplo idle puede estar en der(es la misma animacion para izquierda  pero con Flip , 
 * ya lo hace el codigo solo),front y back.
 *
 * Al impotar las animaciones , se deberán cambiar por las que aparece en la zona de Animator.StringtoHash
 * por ejemplo "Player_Idle" , siguiendo esa estructura para que el codigo funcione
 * Estan implementado todas las animaciones segun la direccion del raton y tambien segun la tecla.
 * 
 * En player (root) ,apartado de Animator se le pasa el de playerAesthetics
 * En escena cambiar las Anchor para que coincidan con la del player
 * En la zona de Animator no hay transiciones todo lo hace por crossfadeIn
 * */



[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Player : Character
{

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Animator anim;//_anim arrastrar aqui el animator de playerAesthetics
    [SerializeField] private SpriteRenderer spriteRenderer;//para que rote visualmente al caminar (ARREGLO DEL SALTO)


    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashCooldown = 0.5f;


    //nombre EXACTOS de los estados
    //Estados de izquierda y derecha
    private readonly int IdleAnimState = Animator.StringToHash("Player_Idle");
    private readonly int WalkAnimState = Animator.StringToHash("Player_Walk");
    private readonly int Attack1AnimState = Animator.StringToHash("Player_Attack1");
    private readonly int Attack2AnimState = Animator.StringToHash("Player_Attack2");
    private readonly int HitAnimState = Animator.StringToHash("Player_Hit");
    private readonly int DeathAnimState = Animator.StringToHash("Player_Death");
    private readonly int DashAnimState = Animator.StringToHash("Player_Dash");


    //Estados de front y bacj (w y s)
    private readonly int IdleFrontAnimState = Animator.StringToHash("Player_Idle_Front");
    private readonly int IdleBackAnimState = Animator.StringToHash("Player_Idle_Back");

    private readonly int WalkFrontAnimState = Animator.StringToHash("Player_Walk_Front");
    private readonly int WalkBackAnimState = Animator.StringToHash("Player_Walk_Back");

    private readonly int Attack1FrontAnimState = Animator.StringToHash("Player_Attack1_Front");
    private readonly int Attack1BackAnimState = Animator.StringToHash("Player_Attack1_Back");

    private readonly int Attack2FrontAnimState = Animator.StringToHash("Player_Attack2_Front");
    private readonly int Attack2BackAnimState = Animator.StringToHash("Player_Attack2_Back");

    private readonly int DashFrontAnimState = Animator.StringToHash("Player_Dash_Front");
    private readonly int DashBackAnimState = Animator.StringToHash("Player_Dash_Back");

    private readonly int DeathFrontAnimState = Animator.StringToHash("Player_Death_Front");
    private readonly int DeathBackAnimState = Animator.StringToHash("Player_Death_Back");

    private readonly int HitFrontAnimState = Animator.StringToHash("Player_Hit_Front");
    private readonly int HitBackAnimState = Animator.StringToHash("Player_Hit_Back");

    //variables del codigo
    private Rigidbody2D _rb;
    private Vector2 _movement;//direccion del input
    private Vector2 _animDir = Vector2.right;//direccion del raton
    private int _currentLocomotionHash = -1;//recuerda el clip de animacion que usa para poder cambiar

    private bool _isAttack = true;
    private bool _isDashing = false;
    private bool _canDash = true;

    private CharacterState _state;


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

        //fallback
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }

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


    private void Update()//primero lee la entrada de teclado y luego la direccion del raton
    {
        if (_state == CharacterState.Hurt || _state == CharacterState.Die)
        {
            return;
        }

        //lectura de teclas y normaliza para evitar lo de la diagonal
        _movement = InputManager.Instance != null ? InputManager.Instance.GetMovement() : Vector2.zero;
        if (_movement.sqrMagnitude > 1f)
        {
            _movement.Normalize();//corrige la diagonal
        }

        //dir del ratón para decidir front o back
        if (InputManager.Instance != null)
        {
            Vector3 mouseWorld = InputManager.Instance.GetPointerWorldPosition();

            Vector2 dir = (Vector2)(mouseWorld - transform.position);
            if (dir.sqrMagnitude > 0.0001f)
            {
                _animDir = dir.normalized;
            }
        }


        //cambio flip visualmente no por player(root) , depende de si hay movimiento o no
        if (_state != CharacterState.Attack && _state != CharacterState.Dash)
        {
            float x;
            bool isMoving = !IsNearlyZero(_movement);

            if (isMoving)
            {
                //flip depende de la dirección de movimiento(en movimiento)
                x = _movement.x;
            }
            else
            {
                //flip depende del ratón ( en idle)
                x = _animDir.x;
            }

            if (x < -0.01f)
            {
                spriteRenderer.flipX = true;//izq
            }
            else if (x > 0.01f)
            {
                spriteRenderer.flipX = false;//der
            }
        }

        //elige la direccion y luego reproduce el clip de ese lado 

        if (_state == CharacterState.Idle || _state == CharacterState.Walk)
        {
            bool isMoving = !IsNearlyZero(_movement);

            if (!isMoving)
            {
                //idle se decide por raton
                int idleTarget;
                bool aimVertical = IsVerticalDominant(_animDir);
                if (aimVertical)
                {
                    idleTarget = (_animDir.y >= 0f) ? IdleBackAnimState : IdleFrontAnimState;
                }
                else
                {

                    idleTarget = IdleAnimState; //flipX a la izq
                }

                //si no esta en idle
                if (_state != CharacterState.Idle)
                {
                    _state = CharacterState.Idle;
                    CrossFadeSafe(idleTarget, IdleAnimState, 0.2f);
                    _currentLocomotionHash = idleTarget;
                }
                else
                {
                    // en idle mira la dir de  raton(arriba o abajo)
                    if (_currentLocomotionHash != idleTarget)
                    {
                        CrossFadeSafe(idleTarget, IdleAnimState, 0.2f);
                        _currentLocomotionHash = idleTarget;
                    }
                }
            }
            else
            {
                //walk se decide por movimiento del teclado
                int walkTarget;
                bool moveVertical = IsVerticalDominant(_movement);
                if (moveVertical)
                {
                    walkTarget = (_movement.y >= 0f) ? WalkBackAnimState : WalkFrontAnimState;
                }
                else
                {
                    //
                    walkTarget = WalkAnimState; //flipXva a la izq
                }

                if (_state != CharacterState.Walk)
                {
                    _state = CharacterState.Walk;
                    CrossFadeSafe(walkTarget, WalkAnimState, 0.2f);
                    _currentLocomotionHash = walkTarget;
                }
                else
                {
                    // en walk, cambia de vertical a horizontal o alrevés
                    if (_currentLocomotionHash != walkTarget)
                    {
                        CrossFadeSafe(walkTarget, WalkAnimState, 0.1f);
                        _currentLocomotionHash = walkTarget;
                    }
                }
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
            _rb.linearVelocity = Vector2.zero;//durante el ataque no se mueve el player
        }
        else if (!_isDashing)
        {
            _rb.linearVelocity = _movement * moveSpeed;
        }

    }

    //DASH
    //En la animacion de prueba del dash hace un frontflip
    //Dash del player
    private void OnDashInput()
    {
        if (!_canDash || _state == CharacterState.Attack || _state == CharacterState.Hurt || _state == CharacterState.Die)
        {
            return;
        }

        StartCoroutine(Dash());
    }



    private IEnumerator Dash()
    {
        _isDashing = true;
        _canDash = false;

        //anim del dash segun dir
        _state = CharacterState.Dash;

        bool verticalDominant = Mathf.Abs(_animDir.y) >= Mathf.Abs(_animDir.x);
        int dashTarget;

        if (verticalDominant)
        {
            if (_animDir.y >= 0f)
            {
                dashTarget = DashBackAnimState;
            }

            else
            {
                dashTarget = DashFrontAnimState;
            }

        }
        else
        {
            dashTarget = DashAnimState;
        }

        CrossFadeSafe(dashTarget, DashAnimState, 0f);


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


    //ATTACKS

    public override void Attack()
    {
        Attack1();
    }


    private void Attack1()//ataque meele
    {
        if (!_isAttack || _state == CharacterState.Attack || _state == CharacterState.Die) return;

        _state = CharacterState.Attack;
        _isAttack = false;
        _rb.linearVelocity = Vector2.zero;

        //elige animacion segun dir del raton
        bool verticalDominant = false;
        if (Mathf.Abs(_animDir.y) >= Mathf.Abs(_animDir.x))
        {
            verticalDominant = true;
        }

        int atk1Target = Attack1AnimState;

        if (verticalDominant)
        {
            if (_animDir.y >= 0f)
            {
                atk1Target = Attack1BackAnimState;
            }
            else
            {
                atk1Target = Attack1FrontAnimState;
            }
        }
        else
        {
            atk1Target = Attack1AnimState;////
        }

        CrossFadeSafe(atk1Target, Attack1AnimState, 0f);

        //hitbox melee
        if (hitAnchor)
        {
            Vector2 forward;

            if (_animDir.sqrMagnitude > 0.0001f)
            {
                forward = _animDir;
            }
            else
            {
                if (spriteRenderer.flipX)
                    forward = Vector2.left;
                else
                    forward = Vector2.right;
            }

            float forwardOffset = 0.4f;
            Vector2 center = (Vector2)hitAnchor.position + forward * forwardOffset;
            float angleDeg = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

            Collider2D[] results = Physics2D.OverlapBoxAll(center, hitSize, angleDeg);

            foreach (var col in results)
            {
                col.GetComponent<ITriggerEnter>()?.HitByPlayer(gameObject);
            }


            StartCoroutine(WaitForAnimationToEnd(atk1Target));
        }
    }

    private void Attack2()//ataque distancia
                          //SOLO HACE LA ANIMACION
                          //HAY QUE AÑADIR EL PROYECTIL AQUI
    {
        if (!_isAttack || _state == CharacterState.Attack || _state == CharacterState.Die) return;

        _state = CharacterState.Attack;
        _isAttack = false;
        _rb.linearVelocity = Vector2.zero;

        bool verticalDominant = Mathf.Abs(_animDir.y) >= Mathf.Abs(_animDir.x);
        int atk2Target;

        if (verticalDominant)
        {
            if (_animDir.y >= 0f)
                atk2Target = Attack2BackAnimState;
            else
                atk2Target = Attack2FrontAnimState;
        }
        else
        {
            atk2Target = Attack2AnimState;
        }

        CrossFadeSafe(atk2Target, Attack2AnimState, 0f);
        StartCoroutine(WaitForAnimationToEnd(atk2Target));
    }


    //TAKEDAMAGE O HIT
    //overrides de character
    public override void TakeDamage(float dmg)
    {
        if (_state == CharacterState.Attack)
        {
            return;
        }
        if (_state == CharacterState.Die)
        {
            return;
        }

        SetHealthActual(GetHealthActual() - dmg);

        if (GetHealthActual() <= 0f)
        {
            _state = CharacterState.Die;
            _rb.linearVelocity = Vector2.zero;

            bool verticalDominant = false;
            if (Mathf.Abs(_animDir.y) >= Mathf.Abs(_animDir.x))
            {
                verticalDominant = true;
            }

            int deathTarget = DeathAnimState;

            if (verticalDominant)
            {
                if (_animDir.y >= 0f)
                {
                    deathTarget = DeathBackAnimState;
                }
                else
                {
                    deathTarget = DeathFrontAnimState;
                }
            }
            else
            {
                deathTarget = DeathFrontAnimState;
            }

            CrossFadeSafe(deathTarget, DeathAnimState, 0f);
        }
        else
        {
            _state = CharacterState.Hurt;
            _isAttack = false;
            _rb.linearVelocity = Vector2.zero;

            // === NUEVO: elegir Hit Front / Back según el ratón ===
            bool verticalDominant = false;
            if (Mathf.Abs(_animDir.y) >= Mathf.Abs(_animDir.x))
            {
                verticalDominant = true;
            }

            int hitTarget = HitAnimState; // fallback genérico

            if (verticalDominant)
            {
                if (_animDir.y >= 0f)
                {
                    hitTarget = HitBackAnimState;//arriba
                }
                else
                {
                    hitTarget = HitFrontAnimState;//abajo
                }
            }
            else
            {
                hitTarget = HitAnimState;//horizontal
            }

            CrossFadeSafe(hitTarget, HitAnimState, 0f);

            //espera al hit segun fallback
            StartCoroutine(WaitForAnimationToEnd(hitTarget, HitAnimState));
        }
    }


    //DEATH
    public override void Die()
    {
        Debug.Log("Player muerto");
        //TODO:codigo reiniciar todas las escenas si muere 
        //TODO:lanzar escena o UI de Game over
    }

    //ANIMACIONES
    //espera a que acabe las animaciones (espera al fallback),para estar en idle
    private IEnumerator WaitForAnimationToEnd(int preferredState, int fallbackState)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //espera al fallback
        while (stateInfo.shortNameHash != preferredState && stateInfo.shortNameHash != fallbackState)
        {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        //espera a normalizarla y que la animacion finalice
        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //al acabar , vuelve a idle
        _state = CharacterState.Idle;
        anim.CrossFadeInFixedTime(IdleAnimState, 0f);
        _isAttack = true;
    }

    //ref de 1 parametro cuando no hay fallback
    private IEnumerator WaitForAnimationToEnd(int animState)
    {
        return WaitForAnimationToEnd(animState, animState);
    }


    //cambia la animacion en caso de que no haya clip, para no colapsar (lo hace en caso de que falte o falle algun clip)
    private void CrossFadeSafe(int preferredStateHash, int fallbackStateHash, float fade)
    {
        if (anim && anim.runtimeAnimatorController && anim.HasState(0, preferredStateHash))
        {
            anim.CrossFadeInFixedTime(preferredStateHash, fade);
        }

        else
        {
            anim.CrossFadeInFixedTime(fallbackStateHash, fade);
        }

    }

    //Helpers para la dir del raton
    private bool IsNearlyZero(Vector2 v)
    {
        return v.sqrMagnitude < 0.0001f;//comprueba si el player esta quieto o no (cerca de 0)
    }

    private bool IsVerticalDominant(Vector2 v)//determina si se apunta mas arriba que a la izq/der
    {//para poder decidir entre las animaciones
        return Mathf.Abs(v.y) >= Mathf.Abs(v.x);
    }

}