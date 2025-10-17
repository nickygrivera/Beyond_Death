using UnityEngine;

public class QuakeOnImpact : MonoBehaviour
{

    [SerializeField] private float radio = 2.5f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float force = 6f;
    [SerializeField] private LayerMask enemyLayer;//para detectar solo enemigos
    [SerializeField] private GameObject quakeVFX;
    [SerializeField] private float durationVfx = 1.2f;

    bool _done;


    //lectura para la UI
    public bool IsDone => _done;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled)
        {
            return;
        }//guard extra

        if (_done)
        {
            return;
        }
        /* Codigo Obtenido por IA para entender las capas
        //
        int capaDelOtro = other.gameObject.layer;//obtener la capa del otro
        int bitDeEsaCapa = 1 << capaDelOtro;//crear el bit correspondiente a esa capa (Bitmaps a la izq)
        int mascaraEnemigos = enemyLayer.value;//obtener la mascara de enemigos como un int
        bool esEnemigo = (bitDeEsaCapa & mascaraEnemigos) != 0;

        if (!esEnemigo) return;*/

        if (((1 << other.gameObject.layer) & enemyLayer.value) == 0) return;
        //

        Vector2 center = transform.position;
        DoQuake(center);

        _done = true;

        var proj = GetComponent<Projectile>();

        if (proj != null)
        {
            proj.DestroyProjectile();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void DoQuake(Vector2 center)//aplica el terremoto
    {
        if (quakeVFX != null)
        {
            var vfx = Instantiate(quakeVFX, center, Quaternion.identity);//instancia el vfx en la posicion del terremoto
            Destroy(vfx, durationVfx);//destruye el vfx tras su duracion
        }

        var hits = Physics2D.OverlapCircleAll(center, radio, enemyLayer);

        foreach (var h in hits)
        {
            var ch = h.GetComponentInParent<Character>();

            if (ch != null)
            {
                ch.TakeDamage(damage);//aplicar el daño
                Debug.Log($"[Quake] Hit {ch.name} por {damage} dmg");
            }

            var rb = h.attachedRigidbody;

            if (rb != null)
            {
                Vector2 dir = ((Vector2)h.transform.position - center).normalized;//calcular la direccion desde el centro al enemigo
                rb.AddForce(dir * force, ForceMode2D.Impulse);//aplicar la fuerza
            }
        }
    }
}