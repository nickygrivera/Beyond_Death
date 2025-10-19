using UnityEngine;

public class RoomTriggerExit : MonoBehaviour
{
    [SerializeField] private RoomController room;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        if (room == null)
        {
            return;
        }

        // Solo apaga límites si la sala ya está limpia
        if (room.IsCleared)
        {
            room.ApagarLimitesAlSalir();
        }

    }
}
