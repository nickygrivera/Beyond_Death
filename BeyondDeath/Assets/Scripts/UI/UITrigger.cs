using UnityEngine;


[RequireComponent (typeof(BoxCollider2D))]
public class UITrigger : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _popUp;
    [SerializeField] private KeyCode _key;
    private bool isInside = false;
    [SerializeField] private bool _exitGame;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        isInside = true;
        UIManager.Instance.ShowMenu(_popUp, true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        isInside = false;
        UIManager.Instance.ShowMenu(_popUp, false);
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
                UIManager.Instance.LoadGame();
            }
        }
    }
}
