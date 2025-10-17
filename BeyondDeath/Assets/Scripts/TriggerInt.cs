using UnityEngine;

public class TriggerInt : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var pickup = other.GetComponent<ITriggerEnter>();

        if (pickup != null)
        {
            pickup.HitByPlayer(gameObject);

        }
            
    }
}

//PARA EL PROYECTIL , EL PICKUP DEBE TENER ISTRIGGER Y COLLIDER2D