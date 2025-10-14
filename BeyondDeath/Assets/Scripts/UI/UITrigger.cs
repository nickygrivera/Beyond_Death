using System.Collections;
using UnityEngine;


[RequireComponent (typeof(BoxCollider2D))]
public class UITrigger : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _popUp;
    private Animator _popUpAnimator;
    [SerializeField] private KeyCode _key;
    private bool isInside = false;
    [SerializeField] private bool _exitGame;
    [SerializeField] private string _gameScene;


    //Manual de uso
    // Si es la options o credits, poned el panel en _menu, si es "exit" o "start game" dejad vacío. Si es salir al juego, haced click en el boleano


    private void Start()
    {
        _popUpAnimator = _popUp.GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInside = true;
            ShowPopUp();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInside = false;
            HidePopUp();
        }
    }

    private void Update()
    {
        if (isInside && Input.GetKeyDown(_key))
        {
            if(_menu != null)
            {
                UIManager.Instance.ShowMenu(_popUp, false);
                UIManager.Instance.ShowMenu(_menu, true);
            }
            else if (_exitGame)
            {
                UIManager.Instance.ExitGame();
            }
            else
            {
                UIManager.Instance.LoadGame(_gameScene);
            }
        }
    }

    private void ShowPopUp()
    {
        _popUpAnimator.SetTrigger("show");
    }
    private void HidePopUp()
    {
        
        _popUpAnimator.SetTrigger("hide");
        
    }


}
