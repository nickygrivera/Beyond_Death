using System.Collections;
using UnityEngine;

public class PU_Projectile : MonoBehaviour
{
    [SerializeField] private float coolDown;
    /*Solo hacer onda expansiva, y se le mete la componente proyectil para que lo instacie con esta habilidad*/
    //Pasarle el spawner
    //Hay una animacion de fireSword  o fireBallpara probar

    //Projectil + hearthquake cuando explota
    
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
        ApplyProjectile();
    }

    private IEnumerator ApplyProjectile()
    {
        
        yield return new WaitForSeconds(coolDown);
    }
}
