using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Player _player;

    [Header("HUD Components")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Text _healthIndicator;


    private void Update()
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        _healthBar.fillAmount = _player.GetHealthActual() / _player.GetHealthMax();
        _healthIndicator.text = _player.GetHealthActual().ToString()+" / " + _player.GetHealthMax();
    }
}
