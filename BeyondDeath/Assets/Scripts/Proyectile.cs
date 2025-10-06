using UnityEngine;

/*
 El proyectil como tal lo que lo dispara y lo gestiona es el Spawner
 IMPORTANTE los proyectiles NO se destruyen, se desactivan
 */

public class Proyectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;

    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject explosionPrefab; // Prefab de la explosion

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

    //se llama destroy pero lo que hace es desactivarlo para reusarlo cuando se necesite
    public void DestroyProjectile()
    {
        _rb.linearVelocity = Vector2.zero; //detiene el movimiento
        projectile.SetActive(false);
    }

    //activa los proyectiles cuando se demandan
    private void ActivateProjectile()
    {
        projectile.SetActive(true);
        _rb.linearVelocity = transform.up * speed;
    }

    //la funcion que gestiona las colisiones
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
