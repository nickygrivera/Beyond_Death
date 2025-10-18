using Unity.Cinemachine;
using UnityEngine;

public class CameraBoundsController : MonoBehaviour
{
    public static CameraBoundsController Instance { get; private set; }
    [SerializeField] private CinemachineConfiner2D confiner;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (confiner == null)
            confiner = Object.FindFirstObjectByType<CinemachineConfiner2D>();
    }

    public void SetBounds(Collider2D shape)
    {
        if (confiner == null || shape == null) return;
        confiner.BoundingShape2D = shape;
        confiner.InvalidateBoundingShapeCache();
    }
}

