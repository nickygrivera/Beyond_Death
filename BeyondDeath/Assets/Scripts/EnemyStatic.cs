using UnityEngine;

/*
Los metodos Die() y TakeDamage() usa los del padre 
*/

public class EnemyStatic : Character
{
    [SerializeField] private float healthMax;
    [SerializeField] private float healthActual;
    [SerializeField] private float damage;

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
