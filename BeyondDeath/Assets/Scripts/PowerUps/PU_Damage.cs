using System.Collections;
using UnityEngine;

public class PU_Damage : MonoBehaviour
{
    [SerializeField] private float damageInc = 1.5f;
    [SerializeField] private float duration = 6f;
    [SerializeField] private float coolDown;
    //poner audio
    [SerializeField] private GameObject auraP;

    // Agregar referencia al jugador
    [SerializeField] private GameObject player;

    private float damageIncial;
    private GameObject gameO = null;
    bool _onCooldown;
    private bool _isrunning;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.WarScreamPerformed += WarScream;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.WarScreamPerformed -= WarScream;
        }
    }

    private void WarScream()
    {
        var ch = player.GetComponent<Character>();

        if (ch == null)
        {
            return;
        }

        StartCoroutine(ApplyDamage(ch));

        //poner audio
    }

    private IEnumerator ApplyDamage(Character ch)
    {

        _isrunning = true;
        damageIncial = ch.GetDamage();
        ch.SetDamage(damageIncial * damageInc);

        if (auraP != null)
        {
            gameO = Instantiate(auraP, ch.transform.position, Quaternion.identity, ch.transform);
        }

        yield return new WaitForSeconds(duration);

        ch.SetDamage(damageIncial);

        if (gameO != null)
        {
            Destroy(gameO);
        }
        _isrunning = false;
        _onCooldown = true;

        yield return new WaitForSeconds(coolDown);
        _onCooldown = false;
    }
}