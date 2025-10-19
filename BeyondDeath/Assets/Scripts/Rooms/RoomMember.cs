using UnityEngine;

public class RoomMember : MonoBehaviour
{
    [SerializeField] private RoomController room;

    private void Awake()
    {
        if (room == null) room = GetComponentInParent<RoomController>();
    }

    public void NotifyDeath()
    {
        if (room != null) room.NotificarEnemigoMuerto(this);
    }
}
