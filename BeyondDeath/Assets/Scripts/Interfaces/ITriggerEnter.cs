using UnityEngine;

public interface ITriggerEnter
{
    /*
     * para reaccionar con cualquier cosa que toque al player 
    se pueden implementar mas metodos aqui para el triggerEnter
    */
    public void HitByPlayer(GameObject player);
}

