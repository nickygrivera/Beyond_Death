using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyRoomBounds : MonoBehaviour
{
    [SerializeField] private Collider2D roomArea;   // Collider de la sala (IsTrigger = ON)
    [SerializeField] private float correctionSpeed = 10f; // Velocidad de correcci�n suave
    [SerializeField] private float snapDistance = 1.5f;   // Si est� m�s lejos, teleporta
    [SerializeField] private float margin = 0.05f;        // Peque�o margen hacia dentro

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (roomArea == null)
            Debug.LogWarning($"[EnemyRoomBounds] Asigna roomArea en {name}");
    }

    private void FixedUpdate()
    {
        if (roomArea == null) return;

        Vector2 center = _rb.position;

        // Si el centro est� dentro del RoomArea, no corrijo.
        if (roomArea.OverlapPoint(center)) return;

        // Punto m�s cercano dentro del RoomArea
        Vector2 closest = roomArea.ClosestPoint(center);
        Vector2 toInside = closest - center;
        float dist = toInside.magnitude;
        if (dist < 0.0001f) return;

        Vector2 dir = toInside / dist;
        Vector2 target = closest - dir * margin;

        // Corto cualquier inercia para evitar oscilaci�n
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        if (dist > snapDistance)
        {
            // Teleport peque�o si est� muy fuera
            _rb.position = target;
        }
        else
        {
            // Correcci�n suave sin empujones
            Vector2 newPos = Vector2.MoveTowards(center, target, correctionSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPos);
        }
    }
}
