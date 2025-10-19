using UnityEngine;


public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonBack;
    [SerializeField] private GameObject menuOptions;

    private bool isPaused = false;

    public void Fullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void ChangeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
    public void OpenMenu()
    {
        menuOptions.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void CloseMenu()
    {
        menuOptions.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}