using UnityEngine;

/*
Los metodos Die() y TakeDamage() usa los del padre 
*/

/*Depende del sistema de cambio de escenas igual habria que hacer esta clase un singlteon*/

public class Player : Character
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
