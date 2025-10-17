using UnityEngine;
 

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance {get; private set;}

    [Header("Gameplay")]
    [SerializeField] public AudioSource[] _playerHitSound;
    [SerializeField] public AudioSource[] _BolaPlasma;
    [SerializeField] public AudioSource _sonidoRevivir;
    [SerializeField] public AudioSource[] _enemigoAtaqueDistancia;
    [SerializeField] public AudioSource[] _enemigoAtaqueMelee;
    [SerializeField] public AudioSource[] _gritoMelee;
    [SerializeField] public AudioSource[] _BolaFuego;
    [SerializeField] public AudioSource _Dash;
    [SerializeField] public AudioSource _gritoPlayer;
    [SerializeField] public AudioSource _habilidadTerremoto;
    [SerializeField] public AudioSource _desbloqueoHabilidad;

    [Header("UI")]
    [SerializeField] public AudioSource _usoHabilidad;
    [SerializeField] public AudioSource _regeneracionHabilidad;

    [Header("MainMenu")]
    [SerializeField] public AudioSource _agujero;
    [SerializeField] public AudioSource _cama;
    [SerializeField] public AudioSource _popUpOpen;
    [SerializeField] public AudioSource _popUpClose;
    [SerializeField] public AudioSource _openBook;
    [SerializeField] public AudioSource _closeBook;
    [SerializeField] public AudioSource _celda;

    [Header("Banda Sonora")]
    [SerializeField] public AudioSource _soundBand1;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlayPlayerHitSound()
    {
        int sel=Random.Range(0,_playerHitSound.Length);
        _playerHitSound[sel].Play();
    }
    public void PlayBolaPlasma()
    {
        int sel = Random.Range(0, _BolaPlasma.Length);
        _BolaPlasma[sel].Play();
    }
    public void PlaySonidoRevivir()
    {
        _sonidoRevivir.Play();
    }
    public void PlayEnemigoAtaqueDistancia()
    {
        int sel = Random.Range(0, _enemigoAtaqueDistancia.Length);
        _enemigoAtaqueDistancia[sel].Play();
    }

    public void PlayEnemigoAtaqueMelee()
    {
        int sel = Random.Range(0, _enemigoAtaqueMelee.Length);
        _enemigoAtaqueMelee[sel].Play();
    }
    public void PlayGritoMelee()
    {
        int sel = Random.Range(0, _gritoMelee.Length);
        _gritoMelee[sel].Play();
    }
    public void PlayBolaFuego()
    {
        int sel = Random.Range(0, _BolaFuego.Length);
        _BolaFuego[sel].Play();
    }
    public void PlayDashSound()
    {
        _Dash.Play();
    }
    public void PlayHabilidadGritoPlayer()
    {
        _gritoPlayer.Play();
    }
    public void PlayHabilidadTerremotoPlayer()
    {
        _habilidadTerremoto.Play();
    }
    public void PlayDesbloqueoHabilidad()
    {
        _desbloqueoHabilidad.Play();
    }
    public void PlayUsoHabilidad()
    {
        _usoHabilidad.Play();
    }
    public void PlayRegeneraciónHabilidad()
    {
        _regeneracionHabilidad.Play();
    }
    public void PlayAgujero()
    {
        _agujero.Play();
    }
    public void PlayCama()
    {
        _cama.Play();
    }
    public void PlayPopUpOpen()
    {
        _popUpOpen.Play();
    }
    public void PlayPopUpClose()
    {
        _popUpClose.Play();
    }
    public void PlayOpenBook()
    {
        _openBook.Play();
    }
    public void PlayCloseBook()
    {
        _closeBook.Play();
    }
    public void PlayCelda()
    {
        _celda.Play();
    }




}
