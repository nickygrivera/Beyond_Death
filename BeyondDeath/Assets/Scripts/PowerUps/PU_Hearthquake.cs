using UnityEngine;

public class PU_Hearthquake : MonoBehaviour, ITriggerEnter
{

    [SerializeField] private float radio = 2.5f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float force=6f;


    //poner aqui  el audio
    [SerializeField] private GameObject quakeP;

    public void HitByPlayer(GameObject player)
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
        Destroy(gameObject);
    }

    /*prueba visual
     private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
     * */

}
