using System;
using UnityEngine;

/*
 El proyectil como tal lo que lo dispara y lo gestiona es el Spawner
 IMPORTANTE los proyectiles NO se destruyen, se desactivan 
 */

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject explosionPrefab;
    
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
    
    void Start()
    {
        ActivateProjectile();
    }

    private void OnEnable()
    {
        ActivateProjectile();
    }

    //se llama destroy pero lo que hace es desactivarlo para revisarlo cuando se necesite
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
        
        /*poner owner y layerMap*/
        
        _rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponent<Character>().TakeDamage(damage);
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        DestroyProjectile();
    }
}
