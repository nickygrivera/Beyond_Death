using UnityEngine;

/*
Los metodos Die() y TakeDamage() usa los del padre 
*/

public class EnemyStatic : Character
{
    [SerializeField] private float healthMax = 70f;
    [SerializeField] private float healthActual;
    [SerializeField] private float damage = 12f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetHealthMax();
        SetDamage();
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
        
    }
}
