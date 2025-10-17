using System.Collections;
using UnityEngine;

/*
 Este codigo es el que se encarga de instanciar los proyectiles
 */
public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Transform projectilePool;

    [SerializeField] private Transform activeProjectilePool;

    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float projectileLifeTime, despawnLifetime;

    [SerializeField] private float fireRate;

    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 1f;

    private bool _canSpawn = true;


    //gestiona si necesita crear nuevas instancias o reactivar instancias "durmientes" o si esta en cooldown

    public void SpawnProjectile(Vector2 direction)
    {
        SpawnProjectile(direction, false);  // por defecto SIN terremoto
    }

    public void SpawnProjectile(Vector2 direction, bool withQuake)
    {
        if (!_canSpawn)
        {
            return;
        }
        _canSpawn = false;
        StartCoroutine(SpawnCooldown());

        GameObject projectile;
        if (projectilePool.childCount <= 0)//si no hay proyectiles en el pool, crear uno nuevo
        {
            projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            projectile = projectilePool.GetChild(0).gameObject;//coger el primer hijo del pool
            projectile.transform.position = spawnPoint.position;//colocarlo en la posicion del spawnpoint
            projectile.SetActive(true);//reactivarlo
        }
        projectile.transform.SetParent(activeProjectilePool);

        // activar/desactivar el terremoto en este disparo
        var quake = projectile.GetComponent<QuakeOnImpact>();
        if (quake != null)
        {
            quake.enabled = withQuake;
        }

        Debug.Log($"[Spawner] Spawn withQuake = {withQuake}");

        var projScript = projectile.GetComponent<Projectile>();

        if (projScript != null)
        {
            projScript.Init(direction, projectileSpeed, projectileDamage, gameObject.layer);
            StartCoroutine(DestroyProjectile(projScript));
        }
    }

    //lo que gestiona cada cuanto se puede disparar
    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(fireRate); //fireRate es lo que lo limita, si se quiere cambiar modificar la variable
        _canSpawn = true;
    }

    //gestiona la vida util del proyectil
    private IEnumerator DestroyProjectile(Projectile projectile)
    {
        yield return new WaitForSeconds(projectileLifeTime);

        if (projectile != null && projectile.gameObject.activeSelf)
        {
            projectile.DestroyProjectile();
        }
        yield return new WaitForSeconds(despawnLifetime);

        if (projectile != null)
        {
            var quake = projectile.GetComponent<QuakeOnImpact>();
            if (quake != null)
            {
                quake.enabled = false;
            }

            projectile.gameObject.SetActive(false);
            projectile.transform.SetParent(projectilePool);
        }
    }
}