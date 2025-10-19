using UnityEngine;

public class RoomTriggerEnter : MonoBehaviour
{
    [SerializeField] private RoomController room;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        room?.ActivarSala();
    }
}
