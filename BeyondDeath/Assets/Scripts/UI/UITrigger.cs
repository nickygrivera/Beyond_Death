using System.Collections;
using UnityEngine;


[RequireComponent (typeof(BoxCollider2D))]
public class UITrigger : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _popUp;
    [SerializeField] private PlayerMenu pMenu;
    private Animator _popUpAnimator;
    private bool isInside = false;
    [SerializeField] private bool _exitGame;
    [SerializeField] private string _gameScene;
    [SerializeField] private int optionsOrCredits;


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
        Debug.Log(isInside);
    }

    public void ShowPopUp()
    {
        _popUpAnimator.SetTrigger("show");
        SoundManager.Instance.PlayPopUpOpen();
    }
    private void HidePopUp()
    {
        
        _popUpAnimator.SetTrigger("hide");
        SoundManager.Instance.PlayPopUpClose();
        
    }

    public void InteractWithUI()
    {
        if (isInside)
        {
            if (_menu != null)
            {
                if (optionsOrCredits == 0)
                {
                    SoundManager.Instance.PlayOpenBook();
                }
                else
                {
                    SoundManager.Instance.PlayCama();
                }
                UIManager.Instance.ShowMenuAndStopPlayer(_menu, true, pMenu);
                _popUpAnimator.SetTrigger("hide");
            }
            else if (_exitGame)
            {
                SoundManager.Instance.PlayAgujero();
                UIManager.Instance.ExitGame();
            }
            else
            {
                StartCoroutine(changeScene());
            }
        }

    }
    private void OnEnable()//suscripciones
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractPerformed += InteractWithUI;
        }
    }

    private void OnDisable()//desuscripciones
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractPerformed -= InteractWithUI;
        }
    }



    IEnumerator changeScene()
    {
        SoundManager.Instance.PlayCelda();
        yield return new WaitForSeconds(2);
        UIManager.Instance.LoadGame(_gameScene);
    }

}
