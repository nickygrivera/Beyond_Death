using UnityEngine;

/*
Los metodos Die() y TakeDamage() usa los del padre 
*/

public class EnemyStatic : Character
{
    [SerializedField] private float healthMax;
    [SerializedField] private float healthActual;
    [SerializedField] private float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetHealthMax();
        SetDamage();
    }

    override protected void Attack(_enemyHealth)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
