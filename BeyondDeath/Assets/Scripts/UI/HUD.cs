using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private UIManager _uiM;

    [Header("HUD Components")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Text _healthIndicator;
    [SerializeField] private GameObject _barraVidaFantasmal;

    [SerializeField] private Image _dashCooldown;
    [SerializeField] private Image _terremotoCooldown;
    [SerializeField] private Image _iraCooldown;
    [SerializeField] private Image _bolaFuegoCooldown;





    private void Start()
    {
        _uiM = UIManager.Instance;
        _barraVidaFantasmal.SetActive(false);
    }

    private void Update()
    {
        _uiM.UpdateHealth(_healthBar, _healthIndicator);
        _dashCooldown.enabled = !_uiM.UpdateCooldown(2);
        _terremotoCooldown.enabled=_uiM.UpdateCooldown(0);
        _iraCooldown.enabled = _uiM.UpdateCooldown(3);
        _bolaFuegoCooldown.enabled = _uiM.UpdateCooldown(1);



    }
}
