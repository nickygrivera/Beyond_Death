using UnityEngine;

/*
La clase Character es la clase base para Player y Enemigos, solo tiene los metodos comunes a TODOS sus hijos.
IMPORTANTE NO cambiar la visibilidad de las variables de esta clase sin consultarlo (puede dar problemas luego).
Si se quiere acceder a los valores de las variables privadas se hace con GetVariable() y eso lo devuelve.
 */

/*
ANOTACIONES DE ESCRITURA EN EL CODIGO
Variables locales camelCase
Variables como parametro _camelCase
Metodos CamelCase()
 */

public class Character : MonoBehaviour
{
    private float healthMax;
    private float healthActual;
    private float damage;

    [SerializeField] protected Transform hitAnchor, bottomAnchor;
    [SerializeField] protected Vector2 hitSize;

    //inicilaizamos vida

    //Getters y Setters
    //protected porque nos interesa que lo usen player y enemigos pero solo ellos
    protected float GetHealthActual()
    {
        if (healthActual <= 0)
        {
            Die();
        }
        return healthActual;
    }

    protected void SetHealthActual(float _healthActual)
    {
        healthActual = _healthActual;//se puede hacer lo del clamp para que no baje de 0

        if (healthActual <= 0)
        {
            Die();
        }
    }
    protected float GetDamage()
    {
        return damage;
    }

    protected void SetDamage(float _damage)
    {
        damage = _damage;
    }
    
    protected void SetHealthMax(float _healtMax)
    {
        healthMax = _healtMax;
        healthActual = healthMax;
    }
    protected float GetHealthMax()
    {
        return healthMax;
    }
    protected virtual void Die()//para el override
    {

    }

    public virtual void TakeDamage(float _damage)
    {
        healthActual -= _damage;
        if (healthActual <= 0f)
        {
            healthActual = 0f;
            Die();
        }
    }

    public void Attack()
    {

    }//player y enemigos los implementan

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitAnchor.position, hitSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(bottomAnchor.position, new Vector2(0.1f, 0.1f));
    }
}   
