using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public AudioSource _audiosourceBG;
    public AudioClip bgmenu;
    void Start()
    {
        _audiosourceBG.clip = bgmenu;
        _audiosourceBG.Play();
    }

    public GameObject Howtoplay;

    public void OpenHowtoplay()
    {
        Howtoplay.SetActive(true);
    }

    public void CloseHowtoplay()
    {
        Howtoplay.SetActive(false);
    }

    public void Playnow()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }

}
