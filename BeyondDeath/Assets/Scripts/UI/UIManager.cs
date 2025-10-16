using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] public Player _player;

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

    public void ShowMenu(GameObject menu, bool show)
    {
        menu.SetActive(show);
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
    /*public float UpdateCooldown(int habilidadType)
    {

        switch (habilidadType)
        {
            case 0: //Terremoto
                return _player.terremotoCooldown;

            case 1: //Bola de Fuego
                return _player.bolaFuegoCooldown;
            case 2: //Dash
                return _player.dashCooldown;
            case 3: //Grito
                return _player.gritoCooldown;
            default: return 0;
        }
    }*/

}
