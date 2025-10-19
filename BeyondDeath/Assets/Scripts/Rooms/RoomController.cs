using UnityEngine;

public class RoomController : MonoBehaviour
{
 
    [SerializeField] private Transform enemiesRoot;//enemies el go vacio
    [SerializeField] private GameObject[] puertas;//puertas de salida (on)
    [SerializeField] private GameObject[] limites;//praedes sala
    [SerializeField] private Collider2D roomArea;//helper de area de juego

    [Header("Setup")]
    [SerializeField] private bool activarAlStart = false;//Room_1 = true

    private int vivos;
    private bool activa;
    private bool cleared;


    public bool IsActive => activa;
    public bool IsCleared => cleared;

    private void Awake()
    {
        vivos = ContarEnemigosInicial();

        //estado inicial
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

    //llama al enterTrigger de la sala
    public void ActivarSala()
    {
        if (activa)
        {
            return;
        }
        activa = true;

        //en combate cierra la salida y activa limites
        SetPuertasCerradas(true);
        SetLimitesActivos(true);

        if (vivos <= 0)
        {

            AbrirSala();
        }
    }

    //llamda del exitTrigger al cruzar la salida
    public void ApagarLimitesAlSalir()
    {
        SetLimitesActivos(false);
    }

    /// <summary>
    /// /notifica cuando muere un enemy en la sala
    /// </summary>
    /// <param name="who"></param>
    public void NotificarEnemigoMuerto(RoomMember who)
    {
        if (!activa || cleared)
        {
            return;
        }

        if (enemiesRoot != null && who != null && !who.transform.IsChildOf(enemiesRoot))
        {

            return; // ese enemigo no es de esta sala
        }

        if (vivos > 0)
        {
            vivos--;
        }
        if (vivos <= 0)
        {
            AbrirSala();
        }
    }

    private void AbrirSala()
    {
        if (cleared)
        {
            return;
        }
        cleared = true;
        SetPuertasCerradas(false); // abre salida (colliders OFF)
        //los limites se apagan al salir de la sala (RoomTriggerExit)
    }

    private void SetPuertasCerradas(bool cerradas)
    {
        if (puertas == null) return;

        foreach (var p in puertas)
        {
            if (!p)
            {
                continue;
            }

           
            var cols = p.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in cols)
            {
                c.enabled = cerradas;//colliders ON/OFF
            }

            
            var rends = p.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends)
            {
                r.enabled = true;
            }
        }
    }

    private void SetLimitesActivos(bool activos)
    {
        if (limites == null)
        {
            return;
        }
        foreach (var l in limites)
        {
            if (l)
            {
                l.SetActive(activos);
            }
        }
            
    }
}
