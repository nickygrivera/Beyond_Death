using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private UIManager _uiM;

    [Header("HUD Components")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Text _healthIndicator;
    [SerializeField] private GameObject _barraVidaFantasmal;

    private void Start()
    {
        _uiM = UIManager.Instance;
        _barraVidaFantasmal.SetActive(false);
    }

    private void Update()
    {
        _uiM.UpdateHealth(_healthBar, _healthIndicator);
        
        //_uiM.UpdateCooldown();

        /*if (_uiM._player.hasRevived)
        {
            _barraVidaFantasmal.SetActive(true);
        }*/

    }
}
