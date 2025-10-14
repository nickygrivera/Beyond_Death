using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
 
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

    
}
