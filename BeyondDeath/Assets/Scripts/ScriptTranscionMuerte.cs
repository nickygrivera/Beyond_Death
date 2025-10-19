using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScriptTranscionMuerte : MonoBehaviour
{

    private AudioSource cosa;
    [SerializeField] GameObject messageDead;
    public float timer = 0;
    [SerializeField] AudioSource activateAudioGhost;
   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cosa = GetComponent<AudioSource>();
        StartCoroutine(ejecutarSonido());
    }

    // Update is called once per frame
    void Update()
    {
       
        cosa.volume -= Time.deltaTime * 0.038f;
       
    }



    IEnumerator ejecutarSonido()
    {
        yield return new WaitForSeconds(timer);
        activateAudioGhost.Play();
        messageDead.SetActive(true);
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("MainMenu");


    }
}
