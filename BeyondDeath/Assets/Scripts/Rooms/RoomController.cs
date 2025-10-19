using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform enemiesRoot;   // contenedor de enemigos de esta sala
    [SerializeField] private GameObject[] puertas;    // SOLO puertas de SALIDA (colliders ON = cerrada)
    [SerializeField] private GameObject[] limites;    // paredes de la sala (SetActive true/false)
    [SerializeField] private Collider2D roomArea;     // opcional (para gizmos/ayudas)

    [Header("Setup")]
    [SerializeField] private bool activarAlStart = false;   // Room_1 = true

    private int vivos;
    private bool activa;
    private bool cleared;


    public bool IsActive => activa;
    public bool IsCleared => cleared;

    private void Awake()
    {
        vivos = ContarEnemigosInicial();

        // Estado inicial (sala abierta y sin límites)
        SetPuertasCerradas(false);
        SetLimitesActivos(false);
    }

    private int ContarEnemigosInicial()
    {
        Transform root = enemiesRoot != null ? enemiesRoot : transform;
        var miembros = root.GetComponentsInChildren<RoomMember>(true);
        return miembros.Length;
    }

    private void Start()
    {
        if (activarAlStart) ActivarSala();
    }

    // Llamada por el EnterTrigger de la sala
    public void ActivarSala()
    {
        if (activa) return;
        activa = true;

        // En combate: cierra puerta de salida y enciende paredes
        SetPuertasCerradas(true);
        SetLimitesActivos(true);

        if (vivos <= 0)
            AbrirSala();
    }

    // Llamada por el ExitTrigger al cruzar la salida
    public void ApagarLimitesAlSalir()
    {
        SetLimitesActivos(false);
    }

    // Llamada por RoomMember.NotifyDeath() cuando muere un enemigo de esta sala
    public void NotificarEnemigoMuerto(RoomMember who)
    {
        if (!activa || cleared) return;

        if (enemiesRoot != null && who != null && !who.transform.IsChildOf(enemiesRoot))
            return; // ese enemigo no es de esta sala

        if (vivos > 0) vivos--;
        if (vivos <= 0) AbrirSala();
    }

    private void AbrirSala()
    {
        if (cleared) return;
        cleared = true;
        SetPuertasCerradas(false); // abre salida (colliders OFF)
        // Los límites siguen activos hasta que el ExitTrigger los apague
    }

    private void SetPuertasCerradas(bool cerradas)
    {
        if (puertas == null) return;

        foreach (var p in puertas)
        {
            if (!p) continue;

            // 1) SOLO colisiones (NO desactivar el GameObject)
            var cols = p.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in cols) c.enabled = cerradas;

            // 2) Mantener visible el sprite siempre (para feedback visual)
            var rends = p.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends) r.enabled = true;
        }
    }

    private void SetLimitesActivos(bool activos)
    {
        if (limites == null) return;
        foreach (var l in limites)
            if (l) l.SetActive(activos);
    }
}
