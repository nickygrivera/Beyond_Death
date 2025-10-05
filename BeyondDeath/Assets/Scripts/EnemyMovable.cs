using System.Collections;
using UnityEngine;

/*
Los metodos Die() y TakeDamage() usa los del padre
*/

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyMovable : Character
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;
    
    private Rigidbody2D _rb;    //El rb y anim se podrian mover a character ya que lo tienen todos
    private Coroutine _attackCoroutine;
    private Vector2 _hitAnchorBasePos;
    
    private bool _isAttacking;
    private bool _canAttack = true;
    private bool _isDead;
    private CharacterState _state;
    
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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if(anim == null) anim = GetComponent<Animator>();
        if(sprite == null) sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //Esto se puede hacer en el editor
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        
        SetHealthMax(100f);
        SetDamage(17f);
        _state = CharacterState.Idle;
        
        if(hitAnchor != null)
            _hitAnchorBasePos = hitAnchor.localPosition;
    }

    private void Update()
    {
        if(_isDead || player == null) return;

        if (!_isAttacking)
            UpdateRotation();

        float distance = Vector2.Distance(transform.position, player.transform.position);
        
        if (distance <= GetAttackDistance())
        {
            _rb.linearVelocity = Vector2.zero;
            if (_canAttack && !_isAttacking)
                Attack();
        }
        else
            MoveToPlayer();
    }

    private void UpdateRotation()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        
        //Rotacion horizontal
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            bool facingLeft = direction.x < 0;
            transform.localScale = new Vector3(facingLeft ? -1 : 1, 1, 1);
            
        }
        else
            sprite.flipX = false;
        
        //Rotación vertical
        
    }
    
    private void MoveToPlayer()
    {
        if(_isAttacking || _state == CharacterState.Hurt || _state == CharacterState.Die) return;
        
        Vector2 direction = (player.transform.position - transform.position).normalized;
        _rb.linearVelocity = direction * speed;

        if (_state != CharacterState.Walk)
            _state = CharacterState.Walk;
    }


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
        
        anim.CrossFadeInFixedTime(_attackAnimState, 0f);
        
        yield return new WaitForSeconds(0.15f); //Delay para permitir la animación antes del danio

        if (hitAnchor != null)
        {
            Vector2 center = hitAnchor.position;
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, hitSize, 0);
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
            anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
        }
    }

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

        if (GetHealthActual() <= 0f)
            Die();
        else
        {
            _state = CharacterState.Hurt;
            anim.CrossFadeInFixedTime(_hitAnimState, 0.1f);
            _rb.linearVelocity = Vector2.zero;
            StartCoroutine(Recover());
        }
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.4f);
        if (!_isDead)
        {
            _state = CharacterState.Idle;
            anim.CrossFadeInFixedTime(_idleAnimState, 0.1f);
        }
    }

    public override void Die()
    {
        _isDead = true;
        _state = CharacterState.Die;
        _rb.linearVelocity = Vector2.zero;
        anim.CrossFadeInFixedTime(_deathAnimState, 0.1f);
        Debug.Log("Enemy melee muerto");
    }
}