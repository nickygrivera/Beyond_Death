using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{


    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private GameObject hud;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        


        if (collision.CompareTag("Player"))
        {
            victoryCanvas.SetActive(true);

            hud.SetActive(false);
        }
    }


    public void ReturnMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
