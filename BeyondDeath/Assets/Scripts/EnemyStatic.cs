using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyStatic : Character
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 4f;
    [SerializeField] private float attackRange = 12f;

    private enum FacingDirection { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }
    private FacingDirection _facingDirection;
    private CharacterState _state;
    
    private Rigidbody2D _rb;
    private Coroutine _attackCoroutine;
    
    private bool _isAttacking;
    private bool _canAttack = true;
    
    //nombre EXACTOS de los estados
    //Estados de izquierda y derecha
    private readonly int _idleAnimState = Animator.StringToHash("EnemyDistance_Idle");
    private readonly int _walkAnimState = Animator.StringToHash("EnemyDistance_Walk");
    private readonly int _attackAnimState = Animator.StringToHash("EnemyDistance_Attack");
    private readonly int _hitAnimState = Animator.StringToHash("EnemyDistance_Hit");
    private readonly int _deathAnimState = Animator.StringToHash("EnemyDistance_Death");
    
    //Estados de front y back (w y s)
    private readonly int _idleFrontAnimState = Animator.StringToHash("EnemyDistance_Idle_Front");
    private readonly int _idleBackAnimState = Animator.StringToHash("EnemyDistance_Idle_Back");
    private readonly int _idleUpRightAnimState = Animator.StringToHash("EnemyDistance_Idle_UpRight");
    private readonly int _idleDownRightAnimState = Animator.StringToHash("EnemyDistance_Idle_DownRight");
    
    private readonly int _walkFrontAnimState = Animator.StringToHash("EnemyDistance_Walk_Front");
    private readonly int _walkBackAnimState = Animator.StringToHash("EnemyDistance_Walk_Back");
    private readonly int _walkUpRightAnimState = Animator.StringToHash("EnemyDistance_Walk_UpRight");
    private readonly int _walkDownRightAnimState = Animator.StringToHash("EnemyDistance_Walk_DownRight");
    
    private readonly int _attackFrontAnimState = Animator.StringToHash("EnemyDistance_Attack_Front");
    private readonly int _attackBackAnimState = Animator.StringToHash("EnemyDistance_Attack_Back");
    private readonly int _attackUpRightAnimState = Animator.StringToHash("EnemyDistance_Attack_UpRight");
    private readonly int _attackDownRightAnimState = Animator.StringToHash("EnemyDistance_Attack_DownRight");
    
    private readonly int _deathFrontAnimState = Animator.StringToHash("EnemyDistance_Death_Front");
    private readonly int _deathBackAnimState = Animator.StringToHash("EnemyDistance_Death_Back");
    private readonly int _deathUpRightAnimState = Animator.StringToHash("EnemyDistance_Death_UpRight");
    private readonly int _deathDownRightAnimState = Animator.StringToHash("EnemyDistance_Death_DownRight");
    
    private readonly int _hitFrontAnimState = Animator.StringToHash("EnemyDistance_Hit_Front");
    private readonly int _hitBackAnimState = Animator.StringToHash("EnemyDistance_Hit_Back");
    private readonly int _hitUpRightAnimState = Animator.StringToHash("EnemyDistance_Hit_UpRight");
    private readonly int _hitDownRightAnimState = Animator.StringToHash("EnemyDistance_Hit_DownRight");
    
    
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

        float distance = Vector2.Distance(transform.position, player.transform.position);
        // Atacar al player si está en rango
        if (distance <= attackRange)
        {
            _rb.linearVelocity = Vector2.zero;
            if (_canAttack && !_isAttacking)
                Attack();
        }
    }
    
    //Rotar enemy en base a la direccion en la que esta el player
    private void UpdateRotation()
         {
             Vector2 direction = (player.transform.position - transform.position).normalized;
             float absX = Mathf.Abs(direction.x);
             float absY = Mathf.Abs(direction.y);
            
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
        const float range = 0.7f;
        Vector2 shootDirection = (player.transform.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + (Vector3)(shootDirection * range);

        // Animación según dirección
        switch (_facingDirection)
        {
            case FacingDirection.Left:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_attackAnimState, 0f);
                break;
            case FacingDirection.Right:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_attackAnimState, 0f);
                break;
            case FacingDirection.Up:
                anim.CrossFadeInFixedTime(_attackFrontAnimState, 0f);
                break;
            case FacingDirection.Down:
                anim.CrossFadeInFixedTime(_attackBackAnimState, 0f);
                break;
            case FacingDirection.UpLeft:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_attackUpRightAnimState, 0f);
                break;
            case FacingDirection.UpRight:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_attackUpRightAnimState, 0f);
                break;
            case FacingDirection.DownLeft:
                sprite.flipX = true;
                anim.CrossFadeInFixedTime(_attackDownRightAnimState, 0f);
                break;
            case FacingDirection.DownRight:
                sprite.flipX = false;
                anim.CrossFadeInFixedTime(_attackDownRightAnimState, 0f);
                break;
        }

        yield return new WaitForSeconds(0.15f); //Delay para permitir la animación antes del disparo

        // Instanciar proyectil
        if (projectilePrefab != null)
        {
            GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Init(shootDirection, projectileSpeed, GetDamage());
            }
            else
            {
                Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
                if (projRb != null)
                    projRb.linearVelocity = shootDirection * projectileSpeed;
            }
        }

        // Fin de animación de ataque
        yield return new WaitForSeconds(GetAttackCooldown());
        _isAttacking = false;
        _canAttack = true;
        _attackCoroutine = null;

        // Animación de idle según dirección
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
        Debug.Log("Vida enemy: "+ GetHealthActual());
        
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
        }
        Debug.Log("Enemy Distance muerto");
        Destroy(gameObject, 2f);    //Tiempo de espera de 2 segundos antes de que se destruya el gameobject
    }
}