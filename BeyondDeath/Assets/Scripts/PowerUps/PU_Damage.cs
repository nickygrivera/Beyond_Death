using System.Collections;
using UnityEngine;

public class PU_Damage : MonoBehaviour
{
    [SerializeField] private float damageInc = 1.5f;
    [SerializeField] private float duration = 6f;
    [SerializeField] private float coolDown;
    //poner audio
    [SerializeField] private GameObject auraP;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform auraAnchor;//punto desde donde sale el aura ( modificar en la escena)

    private float damageIncial;
    private GameObject gameO = null;
    bool _onCooldown;
    private bool _isrunning;

    //lectura para la UI
    public bool IsOnCooldown => _onCooldown;
    public bool IsRunning => _isrunning;
    public float Cooldown => coolDown;
    public float Duration => duration;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        if (auraAnchor == null)
        {
            var sr = player != null ? player.GetComponentInChildren<SpriteRenderer>() : null;//buscar el sprite renderer del player
            auraAnchor = sr != null ? sr.transform : player?.transform;//fallback al transform del player
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
        if (_onCooldown || _isrunning)//si esta en cooldown o ya se esta ejecutando, salir
        {
            return;
        }
        //fallback por si no se ha asignado el player
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        //si sigue siendo null, salir
        if (player == null)
        {
            return;
        }


        var ch = player.GetComponent<Character>();//coger el componente character del player

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
        SoundManager.Instance.PlayHabilidadGritoPlayer();

        if (auraP != null && auraAnchor != null)
        {
            gameO = Instantiate(auraP, auraAnchor);
            gameO.transform.localPosition = Vector3.zero;
        }

        var srPlayer = ch.GetComponentInChildren<SpriteRenderer>(true);
        if (srPlayer != null)
        {
            foreach (var sr in gameO.GetComponentsInChildren<SpriteRenderer>(true))
            {
                sr.sortingLayerID = srPlayer.sortingLayerID;
                sr.sortingOrder = srPlayer.sortingOrder + 1; // siempre por encima
            }
        }

        yield return new WaitForSeconds(duration);

        ch.SetDamage(damageIncial);

        if (gameO != null)
        {
            Destroy(gameO);
        }
        _isrunning = false;
        _onCooldown = true;//poner en cooldown

        yield return new WaitForSeconds(coolDown);
        SoundManager.Instance.PlayRegeneraciónHabilidad();
        _onCooldown = false;//quitar cooldown
    }
}
//poner en sorting layer para que se vea encima del player(player+1 por ejemplo)