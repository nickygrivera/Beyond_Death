using System.Collections;
using UnityEngine;

public class PU_Damage : MonoBehaviour,ITriggerEnter
{
    [SerializeField] private float damageInc = 1.5f;
    [SerializeField] private float duration = 6f;
    //poner audio
    [SerializeField] private GameObject auraP;


    
    private float damageIncial;// = GetDamage();
    private GameObject gameO = null;
    
    public void HitByPlayer(GameObject player)
    {
        var ch=player.GetComponent<Character>();

        if (ch == null)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(ApplyDamage(ch));

        //poner audio
        Destroy(gameObject);    

    }

    
    private IEnumerator ApplyDamage (Character ch)
    {
        damageIncial=ch.GetDamage();
        ch.SetDamage(damageIncial*damageInc);

       if(auraP != null)
       {
            gameO=Instantiate(auraP,ch.transform.position,Quaternion.identity,ch.transform);
       }

        yield return new WaitForSeconds(duration);

        ch.SetDamage(damageIncial);

        if (gameO != null)
        {
            Destroy(gameO);
        }
    }//corrutina para que lo mantenga durante 6 segundos
    
}


//HACERR PICKUPS- Con collider 2d(isTrigger true),rg kinematic