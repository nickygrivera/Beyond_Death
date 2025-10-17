using System;
using System.Collections;
using UnityEngine;

public class PU_Projectile : MonoBehaviour
{


    [SerializeField] private float coolDown;
    [SerializeField] private ProjectileSpawner projectileSpawner;
    [SerializeField] private Transform spawnPoint;

    //poner audio
    [SerializeField] private GameObject fireP;
    [SerializeField] private Player player;

    //Projectil + hearthquake cuando explota

    private bool _onCooldown;

    //lectura para la UI
    public bool IsOnCooldown => _onCooldown;
    public float Cooldown => coolDown;

    private void Awake()
    {
        if (player == null)
        {
            if (player == null)
            {
                var go = GameObject.FindWithTag("Player");

                if (go != null)
                {
                    player = go.GetComponent<Player>();
                }
            }
        }

        /*if (projectileSpawner == null)
        {
            projectileSpawner = GetComponent<ProjectileSpawner>();//fallback por si no se ha asignado el player
        }
        if (spawnPoint == null && projectileSpawner != null)
        {
            spawnPoint = projectileSpawner.transform;//fallback por si no se ha asignado el player
        }*/
    }

    private void OnEnable()//suscripciones
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.FireBallPerformed += FireBall;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.FireBallPerformed -= FireBall;
        }
    }

    private void FireBall()
    {
        /*if (_onCooldown || projectileSpawner == null || spawnPoint == null)//si esta en cooldown o ya se esta ejecutando, salir
        {
            return;
        }

        //se le vuelve a calcular la posicion ya que no depende de un player fijo 
        //y calcula cuando se esta dando a la tecla en el momento ,la ultima posicion que se actualizo de otra clase
        Vector3 mouseWorld =InputManager.Instance.GetPointerWorldPosition();
        Vector2 direction = (mouseWorld - spawnPoint.position);//.normalized;

        if(direction.sqrMagnitude < 0.0001f)
        {
            direction=Vector2.right;
        }
        direction.Normalize();

        if (fireP != null)
        {
            Instantiate(fireP, spawnPoint.position, Quaternion.identity);
        }

        projectileSpawner.SpawnProjectile(direction);//llamar al spawner para que cree el proyectil*/
        StartCoroutine(ApplyProjectile());//iniciar el cooldown


    }

    private IEnumerator ApplyProjectile()
    {
        if (_onCooldown || player == null) yield break;

        // Dirección hacia el puntero
        Vector3 mouseWorld = InputManager.Instance.GetPointerWorldPosition();
        Vector2 dir = mouseWorld - player.transform.position;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        dir.Normalize();

        // Disparo con TERREMOTO
        var spawner = player.GetComponent<ProjectileSpawner>();
        if (spawner != null)
        {
            spawner.SpawnProjectile(dir, true); // <- quake SOLO aquí
            Debug.Log("[PU_Projectile] Disparo CON quake (tecla 3).");
        }


        _onCooldown = true;
        yield return new WaitForSeconds(coolDown);
        _onCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {

        }
    }
}