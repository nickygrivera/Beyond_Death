using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 _direction;
    private float _speed;
    private float _damage;
    private Rigidbody2D _rb;
    [SerializeField] private float lifeTime = 3f;
    private int _ownerLayer;

    //Inicializacion desde el enemigo o player
    public void Init(Vector2 direction, float speed, float damage, int ownerLayer)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        _ownerLayer = ownerLayer;

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

        var quake = GetComponent<QuakeOnImpact>();

        if (quake != null && quake.enabled && collision.gameObject.layer == LayerMask.NameToLayer("EnemyLayer"))
        {
            return; //hará el daño, VFX y desactivará el proyectil
        }


        if (collision.gameObject.layer == _ownerLayer)
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerLayer") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyLayer"))
        {
            Character character = collision.GetComponentInParent<Character>();
            if (character != null)
            {
                character.TakeDamage(_damage);
                Debug.Log($"[Projectile] Hit {character.name} por {_damage} dmg");
            }
            DestroyProjectile();
        }
        else
        {
            DestroyProjectile();
        }
    }

    public void DestroyProjectile()
    {
        if (gameObject.activeSelf)
        {

            gameObject.SetActive(false);
        }
    }
}