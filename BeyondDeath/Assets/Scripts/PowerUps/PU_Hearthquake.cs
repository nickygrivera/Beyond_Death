using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Hearthquake : MonoBehaviour
{

    [SerializeField] private float radio = 2.5f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float force=6f;
    [SerializeField] private float coolDown;


    //poner aqui  el audio
    [SerializeField] private GameObject quakeP;
    [SerializeField] private GameObject player; 

    private bool _onCooldown;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }
    private void OnEnable()//suscripciones
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.HearthquakePerformed += Hearthquake;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.HearthquakePerformed -= Hearthquake;
        }
    }

    private void Hearthquake()
    {
        if (_onCooldown)
        {
            return;
        }

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
            
        if (player == null)
        {
            return;
        }

        StartCoroutine(ApplyHearthquake(player));
    }
    
    private IEnumerator ApplyHearthquake(GameObject player)
    {
        Vector3 center=player.transform.position;

        if (quakeP != null)
        {
            Instantiate(quakeP, center, Quaternion.identity);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radio);

        foreach (var h in hits)
        {
            if (h.CompareTag("Enemy"))
            {
                var ch = h.GetComponentInParent<Character>();

                if (ch != null)
                {
                    ch.TakeDamage(damage);
                }

                var rb = h.attachedRigidbody;

                if (rb != null)
                {
                    Vector2 dir = (h.transform.position - center).normalized;
                    rb.AddForce(dir * force, ForceMode2D.Impulse);
                }
            }
        }
        
        _onCooldown = true;
        yield return new WaitForSeconds(coolDown);
        _onCooldown = false;
    }

    /*prueba visual
     private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
     * */

}
