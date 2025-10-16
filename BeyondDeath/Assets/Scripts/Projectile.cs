using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 _direction;
    private float _speed;
    private float _damage;
    private Rigidbody2D _rb;
    [SerializeField] private float lifeTime = 3f;

    //Inicializacion desde el enemigo
    public void Init(Vector2 direction, float speed, float damage)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        if (_rb != null)
        {
            _rb.linearVelocity = _direction * _speed;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Los proyectiles hacen danio al jugador
        if (collision.CompareTag("Player"))
        {
            Character playerChar = collision.GetComponentInParent<Character>();
            if (playerChar != null)
                playerChar.TakeDamage(_damage);
            DestroyProjectile();
        }
        //TODO: Los proyectiles danian a otros enemigos
        //Se destruyen si choca con algo que no sea enemigo o jugador
        else if (!collision.CompareTag("Enemy"))
        {
            DestroyProjectile();
        }
    }

    public void DestroyProjectile()
    {
        gameObject.SetActive(false);
    }
}