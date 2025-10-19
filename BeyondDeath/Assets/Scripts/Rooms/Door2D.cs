using UnityEngine;

public class Door2D : MonoBehaviour
{
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private Renderer[] visuals;

    private void Reset()
    {
        colliders = GetComponentsInChildren<Collider2D>(true);
        visuals = GetComponentsInChildren<Renderer>(true);
    }

    public void SetClosed(bool closed)
    {
        if (colliders != null)
            foreach (var c in colliders) if (c) c.enabled = closed;

        // Las visuales las dejamos siempre visibles. Si quieres ocultarlas al abrir:
        // foreach (var r in visuals) if (r) r.enabled = closed;
    }
}
