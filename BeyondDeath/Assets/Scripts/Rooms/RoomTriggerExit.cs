using UnityEngine;

public class RoomExitTrigger : MonoBehaviour
{
    [SerializeField] private RoomControllerObs room;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        room?.ApagarLimitesAlSalir();
    }
}
