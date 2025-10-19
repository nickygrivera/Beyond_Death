using UnityEngine;


[RequireComponent (typeof(BoxCollider2D))]
public class Abismo : MonoBehaviour
{
    [SerializeField] private Transform respawn;
    [SerializeField] private float damage;

    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger=true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(collision.GetComponent<Player>().isDashing == false)
            {
                collision.GetComponent<Player>().SetHealthActual(collision.GetComponent<Player>().GetHealthActual()-damage);
                collision.transform.position = respawn.position;
            } 
        }
    }
}
