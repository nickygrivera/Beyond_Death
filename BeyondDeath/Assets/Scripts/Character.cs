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

    public: //habria que revisar si esto es public o no porque no interesa que a esto acceda todo
        
        //Getters y Setters
        float GetHealthActual()
        {
            if (healthActual <= 0)
            {
                Die();
            }
            return healthActual;
        }
        void SetHealthActual(float _healthActual)
        {
            healthActual = _healthActual;
        }
        float GetDamage()
        {
            return damage;
        }
        void SetDamage(float _damage)
        {
            damage = _damage;
        }
        void SetHealthMax(float _healtMax)
        {
            healthMax = _healtMax;
            healthActual = healthMax;
        }
        float GetHealthMax()
        {
            return healthMax;
        }
    protected:
        void Die()
        {
            
        }
        void TakeDamage(float _damage)
        {
            healthActual -= damage;
        }

        abstract void Attack(float _oponentHealth); 
}   
