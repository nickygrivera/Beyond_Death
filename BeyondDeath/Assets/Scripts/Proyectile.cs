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
    
    //para guardar el personaje que dispara
    private Character _owner;
    private LayerMask mask;

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
        //elimina el owner para poder reutilizarlo
        _owner = null;
    }

    //activa los proyectiles cuando se demandan
    private void ActivateProjectile()
    {
        projectile.SetActive(true);
        _owner = transform.root.GetComponent<Character>();
        //si lo instancia el player solo interactua con layer enemy
        if (_owner.CompareTag("Player"))
        {
            mask = LayerMask.GetMask("Enemy");
        }
        //si lo instancia el enemy solo interactua con layer player
        else if (_owner.CompareTag("Enemy"))
        {
            mask = LayerMask.GetMask("Player");
        }
        
        _rb.linearVelocity = transform.up * speed;
    }

    //la funcion que gestiona las colisiones
    public void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponent<Character>().TakeDamage(damage);
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        DestroyProjectile();
    }
}
