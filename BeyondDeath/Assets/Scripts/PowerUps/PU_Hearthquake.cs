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

    [SerializeField] private LayerMask enemyLayer;//para detectar solo enemigos

    private bool _onCooldown;

    //lectura para la UI
    public bool IsOnCooldown => _onCooldown;
    public float Cooldown => coolDown;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");//fallback por si no se ha asignado el player
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
        if (_onCooldown)//si esta en cooldown, salir
        {
            return;
        }

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");//fallback por si no se ha asignado el player
        }
            
        if (player == null)//si sigue siendo null, salir
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

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radio,enemyLayer);

        foreach (var h in hits)
        {
            /*CON EL TAG
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
            }*/

            //CON LAYERMASK ENEMYLAYER
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
        
        _onCooldown = true;//poner en cooldown
        yield return new WaitForSeconds(coolDown);
        _onCooldown = false;//quitar cooldown
    }

    
     private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
     

}
