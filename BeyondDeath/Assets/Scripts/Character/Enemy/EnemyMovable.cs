using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyMovable : Character
{
    [SerializeField] private GameObject player;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float detectionRange = 5f;
    
    //private enum FacingDirection { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }
    private enum FacingDirection { Left, Right, Up, Down }
    private FacingDirection _facingDirection;
    private CharacterState _state;
    
    private Rigidbody2D _rb;
    private Coroutine _attackCoroutine;
    
    private bool _isAttacking;
    private bool _canAttack = true;
    
    //nombre EXACTOS de los estados
    //Estados de izquierda y derecha
    private readonly int _idleAnimState = Animator.StringToHash("EnemyMelee_Idle");
    private readonly int _walkAnimState = Animator.StringToHash("EnemyMelee_Walk");
    private readonly int _attackAnimState = Animator.StringToHash("EnemyMelee_Attack");
    private readonly int _hitAnimState = Animator.StringToHash("EnemyMelee_Hit");
    private readonly int _deathAnimState = Animator.StringToHash("EnemyMelee_Death");
    
    //Estados de front y back (w y s)
    private readonly int _idleFrontAnimState = Animator.StringToHash("EnemyMelee_Idle_Front");
    private readonly int _idleBackAnimState = Animator.StringToHash("EnemyMelee_Idle_Back");
    //private readonly int _idleUpRightAnimState = Animator.StringToHash("EnemyMelee_Idle_UpRight");
    //private readonly int _idleDownRightAnimState = Animator.StringToHash("EnemyMelee_Idle_DownRight");
    
    private readonly int _walkFrontAnimState = Animator.StringToHash("EnemyMelee_Walk_Front");
    private readonly int _walkBackAnimState = Animator.StringToHash("EnemyMelee_Walk_Back");
    //private readonly int _walkUpRightAnimState = Animator.StringToHash("EnemyMelee_Walk_UpRight");
    //private readonly int _walkDownRightAnimState = Animator.StringToHash("EnemyMelee_Walk_DownRight");
    
    private readonly int _attackFrontAnimState = Animator.StringToHash("EnemyMelee_Attack_Front");
    private readonly int _attackBackAnimState = Animator.StringToHash("EnemyMelee_Attack_Back");
    //private readonly int _attackUpRightAnimState = Animator.StringToHash("EnemyMelee_Attack_UpRight");
    //private readonly int _attackDownRightAnimState = Animator.StringToHash("EnemyMelee_Attack_DownRight");
    
    private readonly int _deathFrontAnimState = Animator.StringToHash("EnemyMelee_Death_Front");
    private readonly int _deathBackAnimState = Animator.StringToHash("EnemyMelee_Death_Back");
    //private readonly int _deathUpRightAnimState = Animator.StringToHash("EnemyMelee_Death_UpRight");
    //private readonly int _deathDownRightAnimState = Animator.StringToHash("EnemyMelee_Death_DownRight");
    
    private readonly int _hitFrontAnimState = Animator.StringToHash("EnemyMelee_Hit_Front");
    private readonly int _hitBackAnimState = Animator.StringToHash("EnemyMelee_Hit_Back");
    //private readonly int _hitUpRightAnimState = Animator.StringToHash("EnemyMelee_Hit_UpRight");
    //private readonly int _hitDownRightAnimState = Animator.StringToHash("EnemyMelee_Hit_DownRight");
    
    
    //Inicializar enemy
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if(anim == null) anim = GetComponent<Animator>();
        if(sprite == null) sprite = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        SetHealthMax(100f);
        SetDamage(17f);
        _state = CharacterState.Idle;
    }

    //Calcular posicion hacia el player y moverse hasta el
    private void Update()
    {
        if(GetHasDied() || player == null) return;

        if (!_isAttacking)
            UpdateRotation();
        
        //Calcular distancia al player
        float distance = Vector2.Distance(transform.position, player.transform.position);
        
        if(distance > detectionRange)
        {
            _rb.linearVelocity = Vector2.zero;
            _state = CharacterState.Idle;
            switch (_facingDirection)
            {
                case FacingDirection.Left:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_idleAnimState, 0f);
                    break;
                case FacingDirection.Right:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_idleAnimState, 0f);
                    break;
                case FacingDirection.Up:
                    anim.CrossFadeInFixedTime(_idleFrontAnimState, 0f);
                    break;
                case FacingDirection.Down:
                    anim.CrossFadeInFixedTime(_idleBackAnimState, 0f);
                    break;
            }
            return;
        }
        
        //Atacar al player
        if (distance <= GetAttackDistance())
        {
            _rb.linearVelocity = Vector2.zero;
            if (_canAttack && !_isAttacking)
                Attack();
        }
        else
            MoveToPlayer();
    }
    
    //Rotar enemy en base a la direccion en la que esta el player
    private void UpdateRotation()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        // --- CÓDIGO ORIGINAL (8 direcciones, ahora comentado) ---
        /*
        //Diagonales
        if (absX > 0.5f && absY > 0.5f)
        {
            if (direction.x > 0 && direction.y > 0)
            {
                _facingDirection = FacingDirection.UpRight;
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_walkUpRightAnimState, 0.1f);
            }
            else if (direction.x < 0 && direction.y > 0)
            {
                _facingDirection = FacingDirection.UpLeft;
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_walkUpRightAnimState, 0.1f);
            }
            else if (direction.x > 0 && direction.y < 0)
            {
                _facingDirection = FacingDirection.DownRight;
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_walkDownRightAnimState, 0.1f);
            }
            else if (direction.x < 0 && direction.y < 0)
            {
                _facingDirection = FacingDirection.DownLeft;
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_walkDownRightAnimState, 0.1f);
            }
        //Horizontales
        } else if (absX > absY)
        {
            _facingDirection = direction.x < 0 ? FacingDirection.Left : FacingDirection.Right;
            sprite.flipX = _facingDirection == FacingDirection.Left;
            anim.CrossFadeInFixedTime(_walkAnimState, 0.1f);
        }
        //verticales
        else
        {
            _facingDirection = direction.y > 0 ? FacingDirection.Up : FacingDirection.Down;
            anim.CrossFadeInFixedTime(_facingDirection == FacingDirection.Up
                ? _walkFrontAnimState : _walkBackAnimState, 0.1f);
        }
        */
        // --- FIN CÓDIGO ORIGINAL ---

        // --- SOLO 4 DIRECCIONES ---
        if (absX > absY)
        {
            // Movimiento horizontal
            _facingDirection = direction.x < 0 ? FacingDirection.Left : FacingDirection.Right;
            sprite.flipX = _facingDirection == FacingDirection.Left;
            anim.CrossFadeInFixedTime(_walkAnimState, 0.1f);
        }
        else
        {
            // Movimiento vertical
            _facingDirection = direction.y > 0 ? FacingDirection.Up : FacingDirection.Down;
            anim.CrossFadeInFixedTime(_facingDirection == FacingDirection.Up
                ? _walkFrontAnimState : _walkBackAnimState, 0.1f);
        }
    }

    //Moverse hacia el player
    private void MoveToPlayer()
    {
        if(_isAttacking || _state == CharacterState.Hurt || _state == CharacterState.Die) return;
        
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        Vector2 moveDir;
        // --- CÓDIGO ORIGINAL (comentado) ---
        // _rb.linearVelocity = direction * speed;
        
        // --- SOLO 4 DIRECCIONES ---
        moveDir = absX > absY ? new Vector2(Mathf.Sign(direction.x), 0) : new Vector2(0, Mathf.Sign(direction.y));
        _rb.linearVelocity = moveDir * speed;
        _state = CharacterState.Walk;
    }


    //Atacar al player
    public override void Attack()
    {
        if(_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
        _attackCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _canAttack = false;
        _rb.linearVelocity = Vector2.zero;
        _state = CharacterState.Attack;
        Vector2 offset = hitAnchor.localPosition;
        const float range = 0.7f;
        
        
        switch (_facingDirection)
        {
            case FacingDirection.Left:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_attackAnimState, 0f);
                offset = new Vector2(-range, 0);
                break;
            case FacingDirection.Right:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_attackAnimState, 0f);
                offset = new Vector2(range, 0);
                break;
            case FacingDirection.Up:
                anim.CrossFadeInFixedTime(_attackFrontAnimState, 0f);
                offset = new Vector2(0, range);
                break;
            case FacingDirection.Down:
                anim.CrossFadeInFixedTime(_attackBackAnimState, 0f);
                offset = new Vector2(0, -range);
                break;
            /*
            case  FacingDirection.UpLeft:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_attackUpRightAnimState, 0f);
                offset = new Vector2(-range, range);
                break;
            case  FacingDirection.UpRight:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_attackUpRightAnimState, 0f);
                offset = new Vector2(range, range);
                break;
            case  FacingDirection.DownLeft:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_attackDownRightAnimState, 0f);
                offset = new Vector2(-range, -range);
                break;
            case  FacingDirection.DownRight:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_attackDownRightAnimState, 0f);
                offset = new Vector2(range, -range);
                break;
            */
        }
        SoundManager.Instance.PlayEnemigoAtaqueMelee();
        yield return new WaitForSeconds(0.15f); //Delay para permitir la animación antes del danio

        //Area del ataque
        if (hitAnchor != null)
        {
            hitAnchor.localPosition = offset;
            
            Vector2 boxSize = hitSize;
            if(_facingDirection == FacingDirection.Up || _facingDirection == FacingDirection.Down)
                boxSize = new Vector2(hitSize.y, hitSize.x);
            
            Collider2D[] hits = Physics2D.OverlapBoxAll(hitAnchor.position, boxSize, 0);
            foreach (Collider2D col in hits)
            {
                if ((col.CompareTag("Player")))
                {
                    Character playerChar = col.GetComponentInParent<Character>();
                    if(playerChar != null)
                        playerChar.TakeDamage(GetDamage());
                }
            }
        }
        
        //Fin de animación de ataque
        yield return new WaitForSeconds(GetAttackCooldown());
        _isAttacking = false;
        _canAttack = true;
        _attackCoroutine = null;
        
        //Hacer que el enemigo pueda volver a moverse
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > GetAttackDistance())
        {
            _state = CharacterState.Walk;
        }
        else
        {
            _state = CharacterState.Idle;
            switch (_facingDirection)
            {
                case FacingDirection.Left:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
                    break;
                case FacingDirection.Right:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
                    break;
                case FacingDirection.Up:
                    anim.CrossFadeInFixedTime(_idleFrontAnimState, 0.1f);
                    break;
                case FacingDirection.Down:
                    anim.CrossFadeInFixedTime(_idleBackAnimState, 0.1f);
                    break;
                /*
                case FacingDirection.UpLeft:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_idleUpRightAnimState, 0.1f);
                    break;
                case FacingDirection.UpRight:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_idleUpRightAnimState, 0.1f);
                    break;
                case FacingDirection.DownLeft:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_idleDownRightAnimState, 0.1f);
                    break;
                case FacingDirection.DownRight:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_idleDownRightAnimState, 0.1f);
                    break;
                */
            }
        }
    }

    //Recibir danio si el player le ataca
    public override void TakeDamage(float dmg)
    {
        if (GetHasDied())
            return;

        if (_isAttacking)
        {
            StopCoroutine(_attackCoroutine);
            _isAttacking = false;
            _canAttack = true;
        }
        
        SetHealthActual(GetHealthActual() - dmg);
        
        if (GetHealthActual() <= 0f)
            Die();
        else
        {
            _state = CharacterState.Hurt;
            switch (_facingDirection)
            {
                case FacingDirection.Left:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_hitAnimState, 0.1f);
                    break;
                case FacingDirection.Right:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_hitAnimState, 0.1f);
                    break;
                case FacingDirection.Up:
                    anim.CrossFadeInFixedTime(_hitFrontAnimState, 0.1f);
                    break;
                case FacingDirection.Down:
                    anim.CrossFadeInFixedTime(_hitBackAnimState, 0.1f);
                    break;
                /*
                case FacingDirection.UpLeft:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_hitUpRightAnimState, 0.1f);
                    break;
                case FacingDirection.UpRight:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_hitUpRightAnimState, 0.1f);
                    break;
                case FacingDirection.DownLeft:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_hitDownRightAnimState, 0.1f);
                    break;
                case FacingDirection.DownRight:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_hitDownRightAnimState, 0.1f);
                    break;
                */
            }
            _rb.linearVelocity = Vector2.zero;
            StartCoroutine(Recover());
        }
    }

    //Tiempo que el enemy tarda en recuperarse del golpe
    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.4f);
            _state = CharacterState.Idle;
            switch (_facingDirection)
            {
                case FacingDirection.Left:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
                    break;
                case FacingDirection.Right:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
                    break;
                case FacingDirection.Up:
                    anim.CrossFadeInFixedTime(_idleFrontAnimState, 0.1f);
                    break;
                case FacingDirection.Down:
                    anim.CrossFadeInFixedTime(_idleBackAnimState, 0.1f);
                    break;
                /*
                case FacingDirection.UpLeft:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_idleUpRightAnimState, 0.1f);
                    break;
                case FacingDirection.UpRight:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_idleUpRightAnimState, 0.1f);
                    break;
                case FacingDirection.DownLeft:
                    sprite.flipX = true;
                    anim.CrossFadeInFixedTime(_idleDownRightAnimState, 0.1f);
                    break;
                case FacingDirection.DownRight:
                    sprite.flipX = false;
                    anim.CrossFadeInFixedTime(_idleDownRightAnimState, 0.1f);
                    break;
                */
            }
    }

    //El enemy muere
    public override void Die()
    {
        SetHasDied(true);
        _state = CharacterState.Die;
        _rb.linearVelocity = Vector2.zero;
        
        switch (_facingDirection)
        {
            case FacingDirection.Left:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_deathAnimState, 0.1f);
                break;
            case FacingDirection.Right:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_deathAnimState, 0.1f);
                break;
            case FacingDirection.Up:
                anim.CrossFadeInFixedTime(_deathFrontAnimState, 0.1f);
                break;
            case FacingDirection.Down:
                anim.CrossFadeInFixedTime(_deathBackAnimState, 0.1f);
                break;
            /*
            case FacingDirection.UpLeft:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_deathUpRightAnimState, 0.1f);
                break;
            case FacingDirection.UpRight:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_deathUpRightAnimState, 0.1f);
                break;
            case FacingDirection.DownLeft:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_deathDownRightAnimState, 0.1f);
                break;
            case FacingDirection.DownRight:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_deathDownRightAnimState, 0.1f);
                break;
            */
        }

        GetComponent<RoomMember>()?.NotifyDeath();


        Destroy(gameObject, 2f);
    }
}
