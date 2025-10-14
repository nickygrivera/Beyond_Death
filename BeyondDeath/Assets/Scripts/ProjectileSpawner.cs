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

    private bool _canSpawn = true;
    
    private void Start()
    {
        InputManager.Instance.AttackDistancePerformed += OnAttackDistance;
    }

    //gestiona si necesita crear nuevas instancias o reactivar instancias "durmientes" o si esta en cooldown
    private void OnAttackDistance()
    {
        if (!_canSpawn) return;

        _canSpawn = false;
        StartCoroutine(SpawnCooldown());

        GameObject projectile;

        if (projectilePool.childCount <= 0)
        {
            projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            projectile = projectilePool.GetChild(0).gameObject;
            projectile.transform.position = spawnPoint.position;
            projectile.SetActive(true);
        }
        projectile.transform.SetParent(activeProjectilePool);
        StartCoroutine(DestroyProjectile(projectile.GetComponent<Projectile>()));
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
        projectile.DestroyProjectile();
        yield return new WaitForSeconds(despawnLifetime);
        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(projectilePool);
    }
}