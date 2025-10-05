using UnityEngine;

public class Proyectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;

    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject explosionPrefab; // Prefab de la explosi�n

    private Rigidbody2D _rb;
    private Animator _anim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        ActivateProjectile();
    }

    private void OnEnable()
    {
        ActivateProjectile();
    }

    public void DestroyProjectile()
    {
        _rb.linearVelocity = Vector2.zero;
        projectile.SetActive(false);
    }

    private void ActivateProjectile()
    {
        projectile.SetActive(true);
        _rb.linearVelocity = transform.up * speed;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.GetComponentInParent<Character>()) //para evitar que haga daño al ser disparado
        {
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            DestroyProjectile();
        }
    }
}
