using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] public Player _player;
    [SerializeField] public PlayerMenu _pmenu;

    [Header("Habilidades")]
    [SerializeField] private PU_Damage _ira;
    [SerializeField] private PU_Hearthquake terremoto;
    [SerializeField] private PU_Projectile _bolaFuego;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadGame(string _gameScene)
    {
        SceneManager.LoadScene(_gameScene);
    }
    public void UpdateHealth(Image healthBar, Text healthBarText)
    {
        healthBar.fillAmount = _player.GetHealthActual() / _player.GetHealthMax();
        healthBarText.text = _player.GetHealthActual().ToString() + " / " + _player.GetHealthMax();
    }

    public void ShowMenuAndStopPlayer(GameObject menu, bool show, PlayerMenu player)
    {
        menu.SetActive(show);
        player.enabled = !show;
    }
    public void ShowMenuAndPlayPlayer(GameObject menu)
    {
        menu.SetActive(false);
        _pmenu.enabled = true;
    }
    public bool UpdateCooldown(int habilidadType)
    {

        switch (habilidadType)
        {
            case 0: //Terremoto
                return terremoto.IsOnCooldown;

            case 1: //Bola de Fuego
                return _bolaFuego.IsOnCooldown;
            case 2: //Dash
                return _player.canDash;
            case 3: //Grito
                return _ira.IsOnCooldown;
            default: return false;
        }
    }

}
