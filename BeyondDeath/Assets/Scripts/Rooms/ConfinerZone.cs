using UnityEngine;

public class ConfinerZone : MonoBehaviour
{
    [SerializeField] private Collider2D bounds; // arrastra aquí el collider que quieres usar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        CameraBoundsController.Instance?.SetBounds(bounds);
    }
}
