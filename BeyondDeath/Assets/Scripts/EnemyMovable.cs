using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyMovable : Character
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;

    private enum FacingDirection { Left, Right, Up, Down}
    private FacingDirection _facingDirection;
    private CharacterState _state;
    
    private Rigidbody2D _rb;
    private Coroutine _attackCoroutine;
    
    private bool _isAttacking;
    private bool _canAttack = true;
    private bool _isDead;
    
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

    private readonly int _walkFrontAnimState = Animator.StringToHash("EnemyMelee_Walk_Front");
    private readonly int _walkBackAnimState = Animator.StringToHash("EnemyMelee_Walk_Back");

    private readonly int _attackFrontAnimState = Animator.StringToHash("EnemyMelee_Attack_Front");
    private readonly int _attackBackAnimState = Animator.StringToHash("EnemyMelee_Attack_Back");

    private readonly int _deathFrontAnimState = Animator.StringToHash("EnemyMelee_Death_Front");
    private readonly int _deathBackAnimState = Animator.StringToHash("EnemyMelee_Death_Back");

    private readonly int _hitFrontAnimState = Animator.StringToHash("EnemyMelee_Hit_Front");
    private readonly int _hitBackAnimState = Animator.StringToHash("EnemyMelee_Hit_Back");

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
        if(_isDead || player == null) return;

        if (!_isAttacking)
            UpdateRotation();

        float distance = Vector2.Distance(transform.position, player.transform.position);
        
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
             
             //Rotacion horizontal
             if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
             {
                 _facingDirection = direction.x < 0 ? FacingDirection.Left : FacingDirection.Right;
                 if (_facingDirection == FacingDirection.Left)
                 {
                     anim.CrossFadeInFixedTime(_walkAnimState, 0.1f);
                 }
                 else
                 {
                     anim.CrossFadeInFixedTime(_walkAnimState, 0.1f);
                 }
             }
             //Rotacion vertical
             else
             {
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
        _rb.linearVelocity = direction * speed;

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

        switch (_facingDirection)
        {
            case FacingDirection.Left:
                //TODO: Animacion ataque izquierda
            case FacingDirection.Right:
                anim.CrossFadeInFixedTime(_attackAnimState, 0f);
                break;
            case FacingDirection.Up:
                anim.CrossFadeInFixedTime(_attackFrontAnimState, 0f);
                break;
            case FacingDirection.Down:
                anim.CrossFadeInFixedTime(_attackBackAnimState, 0f);
                break;
        }
        
        yield return new WaitForSeconds(0.15f); //Delay para permitir la animación antes del danio

        if (hitAnchor != null)
        {
            Vector2 offset = hitAnchor.localPosition;
            const float range = 0.7f;
            
            switch (_facingDirection)
            {
                case FacingDirection.Left:
                    offset = new Vector2(-range, 0);
                    break;
                case FacingDirection.Right:
                    offset = new Vector2(range, 0);
                    break;
                case FacingDirection.Up:
                    offset = new Vector2(0, range);
                    break;
                case FacingDirection.Down:
                    offset = new Vector2(0, -range);
                    break;
            }
            
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
                    //TODO: Animacion Idle izquierda
                    break;
                case FacingDirection.Right:
                    anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
                    break;
                case FacingDirection.Up:
                    anim.CrossFadeInFixedTime(_idleFrontAnimState, 0.1f);
                    break;
                case FacingDirection.Down:
                    anim.CrossFadeInFixedTime(_idleBackAnimState, 0.1f);
                    break;
            }
        }
    }

    //Recibir danio si el player le ataca
    public override void TakeDamage(float dmg)
    {
        if (_isDead)
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
                    //TODO: Animacion hit izquierda
                    break;
                case FacingDirection.Right:
                    anim.CrossFadeInFixedTime(_hitAnimState, 0.1f);
                    break;
                case FacingDirection.Up:
                    anim.CrossFadeInFixedTime(_hitFrontAnimState, 0.1f);
                    break;
                case FacingDirection.Down:
                    anim.CrossFadeInFixedTime(_hitBackAnimState, 0.1f);
                    break;
            }
            anim.CrossFadeInFixedTime(_hitAnimState, 0.1f);
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
                    //TODO: Animacion Idle izquierda
                    break;
                case FacingDirection.Right:
                    anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
                    break;
                case FacingDirection.Up:
                    anim.CrossFadeInFixedTime(_idleFrontAnimState, 0.1f);
                    break;
                case FacingDirection.Down:
                    anim.CrossFadeInFixedTime(_idleBackAnimState, 0.1f);
                    break;
            }
    }

    //El enemy muere
    public override void Die()
    {
        _isDead = true;
        _state = CharacterState.Die;
        _rb.linearVelocity = Vector2.zero;
        switch (_facingDirection)
        {
            case FacingDirection.Left:
                //TODO: Animacion muerte izquierda
                break;
            case FacingDirection.Right:
                anim.CrossFadeInFixedTime(_deathAnimState, 0.1f);
                break;
            case FacingDirection.Up:
                anim.CrossFadeInFixedTime(_deathFrontAnimState, 0.1f);
                break;
            case FacingDirection.Down:
                anim.CrossFadeInFixedTime(_deathBackAnimState, 0.1f);
                break;
        }
        Debug.Log("Enemy melee muerto");
    }
}