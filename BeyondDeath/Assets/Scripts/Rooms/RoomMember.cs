using UnityEngine;

public class RoomMember : MonoBehaviour
{
    [SerializeField] private RoomControllerObs room;

    private void Awake()
    {
        if (room == null)
            room = GetComponentInParent<RoomControllerObs>();
    }

    public void NotifyDeath()
    {
        if (room != null) room.NotificarEnemigoMuerto(this);
    }
}
