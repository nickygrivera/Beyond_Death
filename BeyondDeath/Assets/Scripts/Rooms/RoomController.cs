using UnityEngine;

public class RoomControllerObs : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform enemiesRoot;      // hijos con tag Enemy
    [SerializeField] private GameObject[] puertas;       // collider NO trigger: activas = CERRADAS
    [SerializeField] private GameObject[] limites;       // trigger: activas = ON
    [SerializeField] private Collider2D roomArea;        // Box/Poly isTrigger que cubre la sala (para leash)

    [Header("Estado inicial")]
    [SerializeField] private bool activarAlStart = false;  // Room_1 = true

    private int vivos;
    private bool activa;
    private bool cleared;

    private void Awake()
    {
        // Contar enemigos hijos (aunque estén desactivados)
        vivos = 0;
        if (enemiesRoot != null)
        {
            var childs = enemiesRoot.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < childs.Length; i++)
                if (childs[i].CompareTag("Enemy")) vivos++;
        }

        // Estado inicial de puertas/limites
        if (activarAlStart)
        {
            activa = true;
            SetPuertasCerradas(true);   // arrancamos cerrada la sala de inicio
            SetLimitesActivos(true);
            if (vivos <= 0) AbrirSala();
        }
        else
        {
            SetPuertasCerradas(false);  // salas no activas: abiertas y sin límites
            SetLimitesActivos(false);
        }

        if (roomArea == null)
            Debug.LogWarning($"[Room] Falta roomArea en {name}");
    }

    public void ActivarSala()
    {
        if (activa) return;
        activa = true;

        SetPuertasCerradas(true);
        SetLimitesActivos(true);

        if (vivos <= 0) AbrirSala();
    }

    public void NotificarEnemigoMuerto(RoomMember member)
    {
        if (member == null) return;

        // comprobar que pertenece a ESTA sala
        Transform t = member.transform;
        bool esDeEstaSala = enemiesRoot ? t.IsChildOf(enemiesRoot) : t.IsChildOf(transform);
        if (!esDeEstaSala) return;

        if (vivos > 0) vivos--;

        if (activa && !cleared && vivos <= 0)
            AbrirSala();
    }

    public void ApagarLimitesAlSalir() => SetLimitesActivos(false);

    private void AbrirSala()
    {
        if (cleared) return;
        cleared = true;
        SetPuertasCerradas(false);
        // límites se apagarán al cruzar el ExitTrigger
    }

    private void SetPuertasCerradas(bool cerradas)
    {
        if (puertas == null) return;
        for (int i = 0; i < puertas.Length; i++)
            if (puertas[i] != null) puertas[i].SetActive(cerradas);
    }

    private void SetLimitesActivos(bool activos)
    {
        if (limites == null) return;
        for (int i = 0; i < limites.Length; i++)
            if (limites[i] != null) limites[i].SetActive(activos);
    }
}
