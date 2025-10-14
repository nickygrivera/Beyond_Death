using UnityEngine;

/*
La clase Character es la clase base para Player y Enemigos, solo tiene los metodos comunes a TODOS sus hijos.
IMPORTANTE NO cambiar la visibilidad de las variables de esta clase sin consultarlo (puede dar problemas luego).
Si se quiere acceder a los valores de las variables privadas se hace con GetVariable() y eso lo devuelve.
 */

/*
 * IMPORTANTE 
 * ============
ANOTACIONES DE ESCRITURA EN EL CODIGO
- Variables locales camelCase
- Variables como parametro _camelCase
- Metodos CamelCase()
 */

public abstract class Character : MonoBehaviour
{
    [SerializeField] private float healthMax;
    [SerializeField] private float healthActual;
    [SerializeField] private float damage;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    
    [SerializeField] public Transform hitAnchor;//punto desde donde sale el ataque ( modificar en la escena)
    [SerializeField] public Transform bottomAnchor;//punto en los pies (amarillo) 
    [SerializeField] public Vector2 hitSize;//area del golpe

    private bool _hasDied;


    //Getters y Setters
    public float GetHealthActual()
    {
        return healthActual;
    }

    public void SetHealthActual(float _healthActual)
    {
        healthActual = _healthActual;//se puede hacer lo del clamp para que no baje de 0

        if (healthActual <= 0f)
        {
            Die();
        }
    }
    public float GetDamage()//damage  base
    {
        return damage;
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }
    
    public float GetHealthMax()
    {
        return healthMax;
    }
    
    public void SetHealthMax(float _healtMax)//Cambiar para implementar la bar de vida y su logica
    {
        healthMax = _healtMax;
        healthActual = healthMax;
    }

    public float GetAttackDistance()
    {
        return attackDistance;
    }

    public void SetAttackDistance(float newAttackDistance)
    {
        attackDistance = newAttackDistance;
    }
    
    public float GetAttackCooldown()
    {
        return attackCooldown;
    }

    public void SetAttackCooldown(float newAttackCooldown)
    {
        attackCooldown = newAttackCooldown;
    }
    
    public virtual void Die()//para el override en los hijos
    {
        //si hay animacion de muerte habria que hacer play antes
        if (gameObject.CompareTag("Player"))
        {
            //para que reviva una vez
            if (!_hasDied)
            {
                SetHealthMax(healthMax*0.8f); //por lo que se reestablece la vida
                _hasDied = true;
            }
            
            //sacar pantalla de game over o cargar escena de game over
        }

        Destroy(this.gameObject);
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
    
    public abstract void Attack();//player y enemigos los implementan
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitAnchor.position, hitSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(bottomAnchor.position, new Vector2(0.1f, 0.1f));
    }
}   
