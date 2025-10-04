using System;
using UnityEngine;

/*
Los metodos Die() y TakeDamage() usa los del padre 
*/

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyMovable : Character
{
    [SerializeField] private float healthMax = 100f;
    [SerializeField] private float healthActual;
    [SerializeField] private float damage = 17f;
    [SerializeField] private float speed = 4f;
    
    private float _distance;
    private void Start()
    {
        SetHealthMax();
        SetDamage();
    }

    private void Update()
    {

    }

    /*override protected void Attack(_enemyHealth)
    {

    }
    */

    private void SetDamage()
    {
        SetDamage(damage);
    }

    private void SetHealthMax()
    {
        SetHealthMax(healthMax);
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
